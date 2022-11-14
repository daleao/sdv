namespace DaLion.Ligo.Modules.Professions.Patches.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ResourceClumpPerformToolAction : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ResourceClumpPerformToolAction"/> class.</summary>
    internal ResourceClumpPerformToolAction()
    {
        this.Target = this.RequireMethod<ResourceClump>(nameof(ResourceClump.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ResourceClumpPerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

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
                .FindProfessionCheck(Profession.Lumberjack.Value)
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldc_I4_S, 10))
                .AddLabels(isNotPrestiged)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.getLastFarmerToUse))))
                .InsertProfessionCheck(Profession.Lumberjack.Value + 100, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 11),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution1))
                .Advance()
                .AddLabels(resumeExecution1)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add))
                .Advance()
                .AddLabels(resumeExecution2)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.getLastFarmerToUse))))
                .InsertProfessionCheck(Profession.Lumberjack.Value + 100, forLocalPlayer: false)
                .InsertInstructions(new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2))
                .InsertDiceRoll(0.5)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Bgt_S, resumeExecution2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
