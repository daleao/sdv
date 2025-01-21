﻿namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class QuestionEventSetUpPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="QuestionEventSetUpPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal QuestionEventSetUpPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(QuestionEvent).GetInnerMethodsContaining("<setUp>b__0").FirstOrDefault();
    }

    #region harmony patches

    /// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? QuestionEventSetUpTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (0.0055)
        // To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)
        try
        {
            var isNotBreeder = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([
                    // find index of loading base pregnancy chance
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0055),
                ])
                .AddLabels(isNotBreeder) // branch here if player is not breeder
                .Move()
                .AddLabels(resumeExecution) // branch here to resume execution
                .Move(-1)
                .InsertProfessionCheck(Farmer.butcher)
                .Insert([new CodeInstruction(OpCodes.Brfalse_S, isNotBreeder)])
                .Insert([
                    new CodeInstruction(OpCodes.Ldc_R8, 0.0055 * 3), // x3 for regular
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
