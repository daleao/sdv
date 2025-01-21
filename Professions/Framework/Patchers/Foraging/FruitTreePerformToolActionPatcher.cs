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
internal sealed class FruitTreePerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreePerformToolActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FruitTreePerformToolActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FruitTreePerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.getFarmer(lastPlayerToHit).professions.Contains(<lumberjack_id>) ? 1.25 : 1.0
        // To: Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <lumberjack_id>) ? 1.4 : Game1.getFarmer(lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0
        try
        {
            helper
                .Repeat(
                    2,
                    i =>
                    {
                        var isPrestiged = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .MatchProfessionCheck(Farmer.forester)
                            .Move()
                            .Insert([
                                new CodeInstruction(OpCodes.Dup),
                                new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.forester + 100),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(NetIntHashSet).RequireMethod(nameof(NetIntHashSet.Contains))),
                                new CodeInstruction(OpCodes.Brtrue_S, isPrestiged),
                            ])
                            .PatternMatch(
                                i > 0
                                    ? new[] { new CodeInstruction(OpCodes.Ldc_R8, 1.25) }
                                    : [new CodeInstruction(OpCodes.Ldc_I4_5)])
                            .Move()
                            .AddLabels(resumeExecution)
                            .Insert([new CodeInstruction(OpCodes.Br_S, resumeExecution)])
                            .Insert(
                                [
                                    new CodeInstruction(OpCodes.Pop),
                                    i > 0
                                        ? new CodeInstruction(OpCodes.Ldc_R8, 1.5)
                                        : new CodeInstruction(OpCodes.Ldc_I4_6),
                                ],
                                [isPrestiged]);
                    });
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
