namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreePerformBushDestroyPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreePerformBushDestroyPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal TreePerformBushDestroyPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tree>("performBushDestroy");
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreePerformBushDestroyTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        // To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0
        try
        {
            var isPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.forester)
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Dup),
                        new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.forester + 100),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetIntHashSet).RequireMethod(nameof(NetIntHashSet.Contains))),
                        new CodeInstruction(OpCodes.Brtrue_S, isPrestiged),
                    ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R8, 1.25)])
                .Move()
                .AddLabels(resumeExecution)
                .Insert([new CodeInstruction(OpCodes.Br_S, resumeExecution)])
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_R8, 1.5),
                    ],
                    [isPrestiged]);
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
