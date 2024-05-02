namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftCheckStoneForItemsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftCheckStoneForItemsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MineShaftCheckStoneForItemsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.checkStoneForItems));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double
    ///     geode chance + remove Prospector double coal chance.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftCheckStoneForItemsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: chanceForLadderDown += GetSpelunkerBonusLadderChance(who);
        // Before: if (EnemyCount == 0) chanceForLadderDown += 0.04;
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldarg_0), // start of `if (EnemyCount == 0)`
                    new CodeInstruction(OpCodes.Call, typeof(MineShaft).RequirePropertyGetter(nameof(MineShaft.EnemyCount))),
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[6]),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MineShaftCheckStoneForItemsPatcher).RequireMethod(nameof(GetSpelunkerBonusLadderChance))),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Spelunker bonus ladder down chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[12])]) // excavatorMultiplier
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Excavator double geode chance.\nHelper returned {ex}");
            return null;
        }

        helper.GoTo(0);

        // From: if (who.professions.Contains(<geologist_id>) ...
        // To: if (who.professions.Contains(<gemologist_id>) ...
        // + duplicate whole section for prestiged
        // x2
        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .MatchProfessionCheck(Farmer.geologist) // find index of geologist check
                            .Move()
                            .SetOperand(Farmer.gemologist)
                            .PatternMatch(
                                [
                                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                                    new CodeInstruction(OpCodes.Brfalse_S),
                                ],
                                ILHelper.SearchOption.Previous)
                            .CopyUntil(
                                [
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(Game1).RequireMethod(
                                            nameof(Game1.createObjectDebris),
                                            [
                                                typeof(string), typeof(int), typeof(int), typeof(long),
                                                typeof(GameLocation),
                                            ])),
                                ],
                                out var copy,
                                moveInstructionPointer: true,
                                removeLabels: true)
                            .Move()
                            .StripLabels(out var labels)
                            .AddLabels(resumeExecution)
                            .Insert(copy, labels)
                            .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                            .SetOperand(resumeExecution)
                            .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                            .SetOperand(resumeExecution)
                            .MatchProfessionCheck(Farmer.gemologist, ILHelper.SearchOption.Previous)
                            .Move()
                            .SetOperand(Farmer.gemologist + 100)
                            .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                            .SetOperand(resumeExecution);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing vanilla Geologist paired gem chance with Gemologist.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[15])]) // burrowerMultiplier
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static double GetSpelunkerBonusLadderChance(Farmer who)
    {
        return who.IsLocalPlayer && who.HasProfession(Profession.Spelunker) ? State.SpelunkerLadderStreak * 0.005 : 0d;
    }

    #endregion injections
}
