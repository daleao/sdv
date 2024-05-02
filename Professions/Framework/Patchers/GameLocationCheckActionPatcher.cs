namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using FarmerExtensions = DaLion.Professions.Framework.Extensions.FarmerExtensions;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationCheckActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.checkAction));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective
    ///     mod data fields.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationCheckActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.professions.Contains(<botanist_id>) && obj.isForage()) obj.Quality = 4
        // To: if (who.professions.Contains(<ecologist_id>) && obj.isForage() && !IsForagedMineral(obj) obj.Quality = who.GetEcologistForageQuality()
        Label resumeExecution;
        try
        {
            helper
                .MatchProfessionCheck(Farmer.botanist) // find index of botanist check
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)]) // end of check
                .GetOperand(out var notQualifiedForEcologistQuality) // copy failed check branch destination
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_4)]) // start of obj.Quality = 4
                .Move(-1)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ItemExtensions).RequireMethod(
                            nameof(ItemExtensions.IsForagedMineral))),
                    new CodeInstruction(OpCodes.Brtrue_S, notQualifiedForEcologistQuality),
                ])
                .Move()
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertyGetter(nameof(Item.ItemId))),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModDataManagerExtensions).RequireMethod(
                            nameof(ModDataManagerExtensions
                                .AppendToEcologistItemsForaged))),
                ])
                .Move(2)
                .GetOperand(out var temp);

            resumeExecution = (Label)temp;
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Ecologist forage quality and counter increment.\nHelper returned {ex}");
            return null;
        }

        // Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) obj.Quality = who.GetMineralQualityForGemologist()
        try
        {
            var notQualifiedForGemologistQuality = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_3)])
                .StripLabels(out var labels)
                .AddLabels(notQualifiedForGemologistQuality)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)], labels)
                .InsertProfessionCheck(Farmer.gemologist, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notQualifiedForGemologistQuality),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ItemExtensions).RequireMethod(
                            nameof(ItemExtensions.IsForagedMineral))),
                    new CodeInstruction(OpCodes.Brfalse_S, notQualifiedForGemologistQuality),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertyGetter(nameof(Item.ItemId))),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModDataManagerExtensions).RequireMethod(
                            nameof(ModDataManagerExtensions
                                .AppendToGemologistMineralsCollected))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetGemologistMineralQuality))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertySetter(nameof(Item.Quality))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Gemologist foraged mineral quality.\nHelper returned {ex}");
            return null;
        }

        // From: if (random.NextDouble() < 0.2)
        // To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.gatherer)
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R8, 0.2)])
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)])
                .InsertProfessionCheck(Farmer.gatherer + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Forager double forage bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
