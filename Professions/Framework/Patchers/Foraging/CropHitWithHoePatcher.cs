namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHitWithHoePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropHitWithHoePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CropHitWithHoePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.hitWithHoe));
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to wild ginger.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? CropHitWithHoeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (game1.player.professions.Contains(Farmer.botanist)) obj.Quality = Game1.player.GetEcologistForageQuality;
        // Between: obj = new SObject(829, 1);
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_0)])
                .Move()
                .AddLabels(resumeExecution)
                .InsertProfessionCheck(Farmer.botanist)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Call, typeof(ProfessionsMod).RequirePropertyGetter(nameof(Data))),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Item).RequirePropertyGetter(nameof(Item.ItemId))),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModDataManagerExtensions).RequireMethod(
                            nameof(ModDataManagerExtensions
                                .AppendToEcologistItemsForaged))),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed apply Ecologist perk to hoed ginger.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
