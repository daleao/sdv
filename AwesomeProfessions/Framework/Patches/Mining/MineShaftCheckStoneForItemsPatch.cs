namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class MineShaftCheckStoneForItemsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MineShaftCheckStoneForItemsPatch()
    {
        Original = RequireMethod<MineShaft>(nameof(MineShaft.checkStoneForItems));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double
    ///     geode chance + remove Prospetor double coal chance.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> MineShaftCheckStoneForItemsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<spelunker_id>) chanceForLadderDown += ModEntry.State.Value.SpelunkerLadderStreak * 0.005;
        /// After: if (EnemyCount == 0) chanceForLadderDown += 0.04;

        var resumeExecution = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst( // find ladder spawn segment
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(MineShaft).Field("ladderHasSpawned"))
                )
                .Retreat()
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(resumeExecution) // branch here to resume execution
                .Insert(
                    // restore backed-up labels
                    labels,
                    // check for local player
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // prepare profession check
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 4) // arg 4 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Spelunker, resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_3), // local 3 = chanceForLadderDown
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SpelunkerLadderStreak))),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 0.005),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stloc_3)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Spelunker bonus ladder down chance.\nHelper returned {ex}");
            return null;
        }

        /// Skipped: if (who.professions.Contains(<geologist_id>)) ...

        var i = 0;
        repeat1:
        try
        {
            helper // find index of geologist check
                .FindProfessionCheck(Farmer.geologist, i != 0)
                .Retreat()
                .StripLabels(out var labels) // backup and remove branch labels
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
                )
                .GetOperand(out var isNotGeologist) // copy destination
                .Return()
                .Insert( // insert uncoditional branch to skip this check
                    new CodeInstruction(OpCodes.Br_S, (Label) isNotGeologist)
                )
                .Retreat()
                .AddLabels(labels); // restore backed-up labels to inserted branch
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Geologist paired gem chance.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat1;

        /// From: random.NextDouble() < <value> * (1.0 + chanceModifier) * (double)(!who.professions.Contains(<excavator_id>) ? 1 : 2)
        /// To: random.NextDouble() < <value> * (1.0 + chanceModifier)

        i = 0;
        repeat2:
        try
        {
            helper // find index of excavator check
                .FindProfessionCheck(Farmer.excavator, i != 0)
                .Retreat()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Mul) // remove this check
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Excavator double geode chance.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat2;

        /// From: if (random.NextDouble() < 0.25 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2))
        /// To: if (random.NextDouble() < 0.25)

        try
        {
            helper
                .FindProfessionCheck(Farmer.burrower, true) // find index of prospector check
                .Retreat()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Mul) // remove this check
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}