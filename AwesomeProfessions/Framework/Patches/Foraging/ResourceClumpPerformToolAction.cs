namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

using Stardew.Common.Harmony;

using Professions = Utility.Professions;

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
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: numChunks = 10;
        /// To: numChunks = (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>)) ? 11 : 10;
        /// -- and also
        /// Injected: if (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>) && Game1.NextDouble() < 0.5) numChunks++;
        /// Before: numChunks++;

        var notPrestigedLumberjack = iLGenerator.DefineLabel();
        var resumeExecution1 = iLGenerator.DefineLabel();
        var resumeExecution2 = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Professions.IndexOf("Lumberjack"))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10)
                )
                .AddLabels(notPrestigedLumberjack)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).MethodNamed(nameof(Tool.getLastFarmerToUse)))
                )
                .InsertProfessionCheckForPlayerOnStack(100 + Professions.IndexOf("Lumberjack"),
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
                .InsertProfessionCheckForPlayerOnStack(100 + Professions.IndexOf("Lumberjack"),
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
            ModEntry.Log($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}