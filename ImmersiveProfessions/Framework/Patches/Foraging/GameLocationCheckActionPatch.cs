namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Network;

using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

internal class GameLocationCheckActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationCheckActionPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.checkAction));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective
    ///     mod data fields.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationCheckActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.professions.Contains(<botanist_id>) && objects[key].isForage()) objects[key].Quality = 4
        /// To: if (who.professions.Contains(<ecologist_id>) && objects[key].isForage() && !IsForagedMineral(objects[key]) objects[key].Quality = Game1.player.GetEcologistForageQuality()

        CodeInstruction[] got;
        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0) // start of objects[key].isForage() check
                )
                .GetInstructionsUntil(out got, pattern: new CodeInstruction(OpCodes.Callvirt, // copy objects[key]
                        typeof(OverlaidDictionary).RequirePropertyGetter("Item"))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // end of check
                )
                .GetOperand(out var shouldntSetCustomQuality) // copy failed check branch destination
                .Advance()
                .Insert(got) // insert objects[key]
                .Insert( // check if is foraged mineral and branch if true
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.IsForagedMineral))),
                    new CodeInstruction(OpCodes.Brtrue_S, shouldntSetCustomQuality)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // end of objects[key].Quality = 4
                )
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist forage quality.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) objects[key].Quality = GetMineralQualityForGemologist()

        var gemologistCheck = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // return to botanist check
                .Retreat() // retreat to start of check
                .GetInstructionsUntil(out got, true, false, // copy entire section until done setting quality
                    new CodeInstruction(OpCodes.Br_S)
                )
                .AdvanceUntil( // change previous section branch destinations to injected section
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(gemologistCheck)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(gemologistCheck)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .SetOperand(gemologistCheck)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Br_S)
                )
                .Advance()
                .InsertWithLabels(new[] {gemologistCheck}, got) // insert copy with destination label for branches from previous section
                .Return()
                .AdvanceUntil( // find repeated botanist check
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.botanist)
                )
                .SetOperand((int) Profession.Gemologist) // replace with gemologist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var shouldntSetCustomQuality) // copy next section branch destination
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0) // start of call to isForage()
                )
                .RemoveUntil( // right before call to IsForagedMineral()
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(OverlaidDictionary).RequirePropertyGetter("Item"))
                )
                .Advance()
                .ReplaceWith( // remove 'not' and set correct branch destination
                    new(OpCodes.Brfalse_S, (Label) shouldntSetCustomQuality)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .SetOperand(
                    typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions
                        .GetGemologistMineralQuality))
                ); // set correct custom quality method call
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Gemologist foraged mineral quality.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: CheckActionSubroutine(objects[key], this, who)
        /// After: Game1.stats.ItemsForaged++

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Stats).RequirePropertySetter(nameof(Stats.ItemsForaged)))
                )
                .Advance()
                .Insert(got.SubArray(5, 4)) // SObject objects[key]
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_3)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationCheckActionPatch).RequireMethod(nameof(CheckActionSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist and Gemologist counter increment.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// From: if (random.NextDouble() < 0.2)
        /// To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2

        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Forager)
                .Retreat()
                .GetInstructionsUntil(out got, true, true,
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.2)
                )
                .AddLabels(isNotPrestiged)
                .Insert(got)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int) Profession.Forager)
                )
                .SetOperand((int) Profession.Forager + 100)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(isNotPrestiged)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Foraged double forage bonus.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void CheckActionSubroutine(SObject obj, GameLocation location, Farmer who)
    {
        if (who.HasProfession(Profession.Ecologist) && obj.isForage(location) && !obj.IsForagedMineral())
            who.IncrementData<uint>(DataField.EcologistItemsForaged);
        else if (who.HasProfession(Profession.Gemologist) && obj.IsForagedMineral())
            who.IncrementData<uint>(DataField.GemologistMineralsCollected);
    }

    #endregion private methods
}