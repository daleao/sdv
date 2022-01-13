using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Events;
using DaLion.Stardew.Common.Harmony;

namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

[UsedImplicitly]
internal class QuestionEventSetUpPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal QuestionEventSetUpPatch()
    {
        Original = RequireMethod<QuestionEvent>(nameof(QuestionEvent.setUp));
    }

    #region harmony patches

    /// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> QuestionEventSetUpTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (0.0055 * 3)
        /// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

        var isNotBreeder = iLGenerator.DefineLabel();
        var isNotPrestiged = iLGenerator.DefineLabel();
        var resumeExecution = iLGenerator.DefineLabel();
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
                .InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Breeder"), isNotBreeder)
                .InsertProfessionCheckForLocalPlayer(100 + Utility.Professions.IndexOf("Breeder"), isNotPrestiged)
                .Insert( // if player is breeder load adjusted pregancy chance
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0275), // x5 for prestiged
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Insert(
                    new[] {isNotPrestiged},
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0055 * 3), // x3 for regular
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}