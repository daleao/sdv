namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ResourceClumpDestroyAction : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ResourceClumpDestroyAction"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ResourceClumpDestroyAction(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ResourceClump>(nameof(ResourceClump.destroy));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ResourceClumpDestroyTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: numChunks = 10;
        // To: numChunks = (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>)) ? 11 : 10;
        // -- and also
        // Injected: if (t.getLastFarmerToUse().professions.Contains(100 + <lumberjack_id>) && Game1.NextDouble() < 0.5) numChunks++;
        // Before: numChunks++;
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution1 = generator.DefineLabel();
            var resumeExecution2 = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.forester) // vanilla forester is modded lumberjack
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_S, 10)])
                .AddLabels(isNotPrestiged)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.getLastFarmerToUse))),
                    ])
                .InsertProfessionCheck(Farmer.forester + 100, forLocalPlayer: false)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 11), // why 11? because its 50% more than the increase from 8 to 10
                        new CodeInstruction(OpCodes.Br_S, resumeExecution1),
                    ])
                .Move()
                .AddLabels(resumeExecution1)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                    ])
                .Move()
                .AddLabels(resumeExecution2)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.getLastFarmerToUse))),
                    ])
                .InsertProfessionCheck(Farmer.forester + 100, forLocalPlayer: false)
                .Insert([new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2)])
                .InsertDiceRoll(0.5)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Bgt_S, resumeExecution2),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
