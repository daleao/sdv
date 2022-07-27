namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestionEventSetUpPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal QuestionEventSetUpPatch()
    {
        Target = RequireMethod<QuestionEvent>(nameof(QuestionEvent.setUp));
    }

    #region harmony patches

    /// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? QuestionEventSetUpTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (0.0055 * 3)
        /// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

        var isNotBreeder = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst( // find index of loading base pregnancy chance
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0055)
                )
                .AddLabels(isNotBreeder) // branch here if player is not breeder
                .Advance()
                .AddLabels(resumeExecution) // branch here to resume execution
                .Retreat()
                .InsertProfessionCheck(Profession.Breeder.Value)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotBreeder)
                )
                .InsertProfessionCheck(Profession.Breeder.Value + 100)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    // if player is breeder load adjusted pregnancy chance
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0275), // x5 for prestiged
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] { isNotPrestiged },
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0055 * 3), // x3 for regular
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}