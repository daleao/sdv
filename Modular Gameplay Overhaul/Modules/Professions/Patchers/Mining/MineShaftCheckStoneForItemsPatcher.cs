namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftCheckStoneForItemsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftCheckStoneForItemsPatcher"/> class.</summary>
    internal MineShaftCheckStoneForItemsPatcher()
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

        // Injected: if (who.IsLocalPlayer && who.professions.Contains(<spelunker_id>) chanceForLadderDown += ModEntry.PlayerState.SpelunkerLadderStreak * 0.005;
        // After: if (EnemyCount == 0) chanceForLadderDown += 0.04;
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        // find ladder spawn segment
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(MineShaft).RequireField("ladderHasSpawned")),
                    })
                .Move(-1)
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(resumeExecution) // branch here to resume execution
                .Insert(
                    new[]
                    {
                        // check for local player
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        // prepare profession check
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
                    },
                    // restore backed-up labels
                    labels)
                .InsertProfessionCheck(Profession.Spelunker.Value, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldloc_3), // local 3 = chanceForLadderDown
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModState).RequirePropertyGetter(nameof(ModState.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(State).RequirePropertyGetter(nameof(State.SpelunkerLadderStreak))),
                        new CodeInstruction(OpCodes.Conv_R8),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.005),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stloc_3),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Spelunker bonus ladder down chance.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if (who.professions.Contains(<geologist_id>)) ...
        try
        {
            helper
                .Repeat(
                    2,
                    i =>
                    {
                        helper // find index of geologist check
                            .MatchProfessionCheck(
                                Farmer.geologist,
                                i == 0 ? ILHelper.SearchOption.First : ILHelper.SearchOption.Next)
                            .Move(-1)
                            .StripLabels(out var labels) // backup and remove branch labels
                            .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }) // the false case branch
                            .GetOperand(out var isNotGeologist) // copy destination
                            .Return()
                            .Insert(
                                new[]
                                {
                                    // insert unconditional branch to skip this check and restore backed-up labels to this branch
                                    new CodeInstruction(OpCodes.Br_S, (Label)isNotGeologist),
                                },
                                labels);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Geologist paired gem chance.\nHelper returned {ex}");
            return null;
        }

        // From: random.NextDouble() < <value> * (1.0 + chanceModifier) * (double)(!who.professions.Contains(<excavator_id>) ? 1 : 2)
        // To: random.NextDouble() < <value> * (1.0 + chanceModifier)
        try
        {
            helper
                .Repeat(
                    2,
                    i =>
                    {
                        helper // find index of excavator check
                            .MatchProfessionCheck(
                                Farmer.excavator,
                                i == 0 ? ILHelper.SearchOption.First : ILHelper.SearchOption.Next)
                            .Move(-1)
                            .Count(new[] { new CodeInstruction(OpCodes.Mul) }, out var count)
                            .Remove(count); // remove this check
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Excavator double geode chance.\nHelper returned {ex}");
            return null;
        }

        // From: if (random.NextDouble() < 0.25 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2))
        // To: if (random.NextDouble() < 0.25)
        try
        {
            helper
                .MatchProfessionCheck(Farmer.burrower) // find index of prospector check
                .Move(-1)
                .Count(new[] { new CodeInstruction(OpCodes.Mul) }, out var count)
                .Remove(count); // remove this check
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
