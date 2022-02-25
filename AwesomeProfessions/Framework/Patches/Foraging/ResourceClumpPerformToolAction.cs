﻿namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class ResourceClumpPerformToolAction : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ResourceClumpPerformToolAction()
    {
        Original = RequireMethod<ResourceClump>(nameof(ResourceClump.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ResourceClumpPerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: numChunks = 10;
        /// To: numChunks = (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>)) ? 11 : 10;
        /// -- and also
        /// Injected: if (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>) && Game1.NextDouble() < 0.5) numChunks++;
        /// Before: numChunks++;

        var notPrestigedLumberjack = generator.DefineLabel();
        var resumeExecution1 = generator.DefineLabel();
        var resumeExecution2 = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck((int) Profession.Lumberjack)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10)
                )
                .AddLabels(notPrestigedLumberjack)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).MethodNamed(nameof(Tool.getLastFarmerToUse)))
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Lumberjack + 100,
                    notPrestigedLumberjack)
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 11),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1)
                )
                .Advance()
                .AddLabels(resumeExecution1)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add)
                )
                .Advance()
                .AddLabels(resumeExecution2)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).MethodNamed(nameof(Tool.getLastFarmerToUse)))
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Lumberjack + 100,
                    resumeExecution2)
                .InsertDiceRoll()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R8, 0.5),
                    new CodeInstruction(OpCodes.Bgt_S, resumeExecution2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}