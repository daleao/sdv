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
internal sealed class TreeTickUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeTickUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal TreeTickUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.tickUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add bonus wood for prestiged Lumberjack.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? TreeTickUpdateTranspiler(
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
                    _ =>
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
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Lumberjack bonus wood.\nHelper returned {ex}");
            return null;
        }

        // From: numHardwood++;
        // To: numHardwood += Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 2 : 1;
        // -- and also
        // From: numHardwood += (int) (numHardwood * 0.25f + 0.9f);
        // To: numHardwood += (int) (numHardwood * (Game1.getFarmer(lastPlayerToHit).professions.Contains(100 + <arborist_id>) ? 0.5f : 0.25f) + 0.9f);
        helper.GoTo(0);
        try
        {
            var notPrestigedArborist1 = generator.DefineLabel();
            var notPrestigedArborist2 = generator.DefineLabel();
            var resumeExecution1 = generator.DefineLabel();
            var resumeExecution2 = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.lumberjack)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                    ],
                    ILHelper.SearchOption.Previous)
                .AddLabels(notPrestigedArborist1)
                .Insert([new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9])])
                .InsertProfessionCheck(Farmer.lumberjack + 100, forLocalPlayer: false)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist1),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution1),
                    ])
                .Move()
                .AddLabels(resumeExecution1)
                .MatchProfessionCheck(Farmer.lumberjack)
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldc_R4, 0.25f),
                        new CodeInstruction(OpCodes.Mul),
                    ])
                .AddLabels(notPrestigedArborist2)
                .Insert([new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[9])])
                .InsertProfessionCheck(Farmer.lumberjack + 100, forLocalPlayer: false)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, notPrestigedArborist2),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution2),
                    ])
                .Move()
                .AddLabels(resumeExecution2);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Arborist bonus hardwood.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
