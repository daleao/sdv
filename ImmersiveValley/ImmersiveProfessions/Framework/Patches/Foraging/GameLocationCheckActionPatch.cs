namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using StardewValley.Network;
using FarmerExtensions = DaLion.Stardew.Professions.Extensions.FarmerExtensions;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using SObjectExtensions = DaLion.Stardew.Professions.Extensions.SObjectExtensions;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckActionPatch"/> class.</summary>
    internal GameLocationCheckActionPatch()
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
        var helper = new IlHelper(original, instructions);

        // From: if (who.professions.Contains(<botanist_id>) && objects[key].isForage()) objects[key].Quality = 4
        // To: if (who.professions.Contains(<ecologist_id>) && objects[key].isForage() && !IsForagedMineral(objects[key]) objects[key].Quality = Game1.player.GetEcologistForageQuality()
        CodeInstruction[] got;
        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldarg_0)) // start of objects[key].isForage() check
                .GetInstructionsUntil(
                    out got,
                    pattern: new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(OverlaidDictionary).RequirePropertyGetter("Item"))) // copy objects[key]
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S)) // end of check
                .GetOperand(out var shouldntSetCustomQuality) // copy failed check branch destination
                .Advance()
                .InsertInstructions(got) // insert objects[key]
                .InsertInstructions(
                    // check if is foraged mineral and branch if true
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.IsForagedMineral))),
                    new CodeInstruction(OpCodes.Brtrue_S, shouldntSetCustomQuality))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_I4_4)) // end of objects[key].Quality = 4
                .ReplaceInstructionWith(
                    // replace with custom quality
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .InsertInstructions(new CodeInstruction(OpCodes.Call, typeof(Game1)
                    .RequirePropertyGetter(nameof(Game1.player))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist forage quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) objects[key].Quality = GetMineralQualityForGemologist()
        var gemologistCheck = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // return to botanist check
                .Retreat() // retreat to start of check
                .GetInstructionsUntil(
                    out got,
                    true,
                    false, // copy entire section until done setting quality
                    new CodeInstruction(OpCodes.Br_S))
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S)) // change previous section branch destinations to injected section
                .SetOperand(gemologistCheck)
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .SetOperand(gemologistCheck)
                .AdvanceUntil(new CodeInstruction(OpCodes.Brtrue_S))
                .SetOperand(gemologistCheck)
                .AdvanceUntil(new CodeInstruction(OpCodes.Br_S))
                .Advance()
                .InsertWithLabels(new[] { gemologistCheck }, got) // insert copy with destination label for branches from previous section
                .Return()
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.botanist)) // find repeated botanist check
                .SetOperand(Profession.Gemologist.Value) // replace with gemologist check
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldarg_0))
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var shouldntSetCustomQuality) // copy next section branch destination
                .RetreatUntil(new CodeInstruction(OpCodes.Ldarg_0)) // start of call to isForage()
                .RemoveInstructionsUntil(
                    // right before call to IsForagedMineral()
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(OverlaidDictionary).RequirePropertyGetter("Item")))
                .Advance()
                .ReplaceInstructionWith(
                    // remove 'not' and set correct branch destination
                    new CodeInstruction(OpCodes.Brfalse_S, (Label)shouldntSetCustomQuality))
                .AdvanceUntil(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions)
                            .RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .SetOperand(
                    typeof(FarmerExtensions)
                        .RequireMethod(nameof(FarmerExtensions.GetGemologistMineralQuality))); // set correct custom quality method call
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Gemologist foraged mineral quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: CheckActionSubroutine(objects[key], this, who)
        // After: Game1.stats.ItemsForaged++
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Stats).RequirePropertySetter(nameof(Stats.ItemsForaged))))
                .Advance()
                .InsertInstructions(got.SubArray(5, 4)) // SObject objects[key]
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldarg_3))
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationCheckActionPatch).RequireMethod(nameof(CheckActionSubroutine))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist and Gemologist counter increment.\nHelper returned {ex}");
            return null;
        }

        // From: if (random.NextDouble() < 0.2)
        // To: if (random.NextDouble() < who.professions.Contains(100 + <forager_id>) ? 0.4 : 0.2
        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Profession.Forager.Value)
                .Retreat()
                .GetInstructionsUntil(out got, true, true, new CodeInstruction(OpCodes.Brfalse_S))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_R8, 0.2))
                .AddLabels(isNotPrestiged)
                .InsertInstructions(got)
                .RetreatUntil(new CodeInstruction(OpCodes.Ldc_I4_S, Profession.Forager.Value))
                .SetOperand(Profession.Forager.Value + 100)
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .SetOperand(isNotPrestiged)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.4),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Foraged double forage bonus.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CheckActionSubroutine(SObject obj, GameLocation location, Farmer who)
    {
        if (who.HasProfession(Profession.Ecologist) && obj.isForage(location) && !obj.IsForagedMineral())
        {
            who.Increment("EcologistItemsForaged");
        }
        else if (who.HasProfession(Profession.Gemologist) && obj.IsForagedMineral())
        {
            who.Increment("GemologistMineralsCollected");
        }
    }

    #endregion injected subroutines
}
