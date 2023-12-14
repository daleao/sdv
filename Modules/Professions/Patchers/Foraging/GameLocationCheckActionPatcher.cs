namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Network;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatcher"/> class.</summary>
    internal GameLocationCheckActionPatcher()
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

        // From: if (who.professions.Contains(<botanist_id>) && objects[key].isForage()) objects[key].Quality = 4
        // To: if (who.professions.Contains(<ecologist_id>) && objects[key].isForage() && !IsForagedMineral(objects[key]) objects[key].Quality = Game1.player.GetEcologistForageQuality()
        CodeInstruction[] copy;
        try
        {
            helper
                .MatchProfessionCheck(Farmer.botanist, ILHelper.SearchOption.First) // find index of botanist check
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) }) // start of objects[key].isForage() check
                .CountUntil(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(OverlaidDictionary).RequirePropertyGetter("Item")),
                    },
                    out var steps)
                .Copy(out copy, steps) // copy objects[key]
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }) // end of check
                .GetOperand(out var shouldntSetCustomQuality) // copy failed check branch destination
                .Move()
                .Insert(copy) // insert objects[key]
                .Insert(
                    new[]
                    {
                        // check if is foraged mineral and branch if true
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(DaLion.Shared.Extensions.Stardew.SObjectExtensions).RequireMethod(
                                nameof(DaLion.Shared.Extensions.Stardew.SObjectExtensions.IsForagedMineral))),
                        new CodeInstruction(OpCodes.Brtrue_S, shouldntSetCustomQuality),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) }) // end of objects[key].Quality = 4
                .ReplaceWith(
                    // replace with custom quality
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Extensions.FarmerExtensions).RequireMethod(nameof(Extensions.FarmerExtensions
                            .GetEcologistForageQuality))))
                .Insert(new[]
                {
                    new CodeInstruction(OpCodes.Call, typeof(Game1)
                        .RequirePropertyGetter(nameof(Game1.player))),
                });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Ecologist forage quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) objects[key].Quality = GetMineralQualityForGemologist()
        try
        {
            var gemologistCheck = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.botanist, ILHelper.SearchOption.First) // return to botanist check
                .Move(-1) // retreat to start of check
                .CountUntil(new[] { new CodeInstruction(OpCodes.Br_S) }, out var count)
                .Copy(// copy entire section until done setting quality
                    out copy,
                    count,
                    true)
                .Match(new[]
                {
                    // change previous section branch destinations to injected section
                    new CodeInstruction(OpCodes.Brfalse_S),
                })
                .SetOperand(gemologistCheck)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .SetOperand(gemologistCheck)
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue_S) })
                .SetOperand(gemologistCheck)
                .Match(new[] { new CodeInstruction(OpCodes.Br_S) })
                .Move()
                .Insert(
                    copy,
                    new[] { gemologistCheck }) // insert copy with destination label for branches from previous section
                .Return()
                .Match(new[]
                {
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.botanist),
                }) // find repeated botanist check
                .SetOperand(VanillaProfession.Gemologist.Value) // replace with gemologist check
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .GetOperand(out var shouldntSetCustomQuality) // copy next section branch destination
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldarg_0) },
                    ILHelper.SearchOption.Previous) // start of call to isForage()

                .CountUntil(
                    new[]
                    {
                        // right before call to IsForagedMineral()
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(OverlaidDictionary).RequirePropertyGetter("Item")),
                    },
                    out count)
                .Remove(count)
                .Move()
                .ReplaceWith(
                    // remove 'not' and set correct branch destination
                    new CodeInstruction(OpCodes.Brfalse_S, (Label)shouldntSetCustomQuality))
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Extensions.FarmerExtensions)
                                .RequireMethod(nameof(Extensions.FarmerExtensions.GetEcologistForageQuality))),
                    })
                .SetOperand(
                    typeof(Extensions.FarmerExtensions)
                        .RequireMethod(nameof(Extensions.FarmerExtensions
                            .GetGemologistMineralQuality))); // set correct custom quality method call
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Gemologist foraged mineral quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: CheckActionSubroutine(objects[key], this, who)
        // After: Game1.stats.ItemsForaged++
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Stats).RequirePropertySetter(nameof(Stats.ItemsForaged))),
                    })
                .Move()
                .Insert(copy.SubArray(5, 4)) // SObject objects[key]
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldarg_3),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationCheckActionPatcher).RequireMethod(nameof(CheckActionSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Ecologist and Gemologist counter increment.\nHelper returned {ex}");
            return null;
        }

        // From: if (random.NextDouble() < 0.2)
        // To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(VanillaProfession.Forager.Value)
                .Move(-1)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, out var steps)
                .Copy(out copy, steps, true, true)
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 0.2) })
                .AddLabels(isNotPrestiged)
                .Insert(copy)
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_S, VanillaProfession.Forager.Value) },
                    ILHelper.SearchOption.Previous)
                .SetOperand(VanillaProfession.Forager.Value + 100)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .SetOperand(isNotPrestiged)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Foraged double forage bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CheckActionSubroutine(SObject obj, GameLocation location, Farmer who)
    {
        if (who.HasProfession(VanillaProfession.Ecologist) && obj.isForage(location) && !obj.IsForagedMineral())
        {
            who.Increment(DataKeys.EcologistItemsForaged);
        }
        else if (who.HasProfession(VanillaProfession.Gemologist) && obj.IsForagedMineral())
        {
            who.Increment(DataKeys.GemologistMineralsCollected);
            var collected = who.Read<int>(DataKeys.GemologistMineralsCollected);
            if (!ProfessionsModule.Config.CrystalariumUpgradesWithGemologist)
            {
                return;
            }

            if (collected == ProfessionsModule.Config.MineralsNeededForBestQuality / 2)
            {
                Game1.game1.GlobalUpgradeCrystalariums(SObject.highQuality, who);
            }
            else if (collected == ProfessionsModule.Config.MineralsNeededForBestQuality)
            {
                Game1.game1.GlobalUpgradeCrystalariums(SObject.bestQuality, who);
            }
        }
    }

    #endregion injected subroutines
}
