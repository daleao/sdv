namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationBreakStonePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationBreakStonePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationBreakStonePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>("breakStone");
    }

    #region harmony patches

    /// <summary>Patch to remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationBreakStoneTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who.professions.Contains(100 + <miner_id>) addedOres++;
        // After: int addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0);
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_1)])
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_S, (byte)4)]) // arg 4 = Farmer who
                .InsertProfessionCheck(Farmer.miner + 100, forLocalPlayer: false)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Miner extra ores.\nHelper returned {ex}");
            return null;
        }

        // From: if (who.professions.Contains(<geologist_id>) ...
        // To: if (who.professions.Contains(<gemologist_id>) ...
        try
        {
            var notPrestiged = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.geologist)
                .Move()
                .SetOperand(Farmer.gemologist)
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[8])])
                .StripLabels(out var labels)
                .AddLabels(notPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_S, (byte)4)], labels)
                .InsertProfessionCheck(Farmer.gemologist + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing vanilla Geologist paired gems with Gemologist.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if (who.professions.Contains(<burrower_id>)) ...
        try
        {
            helper
                .MatchProfessionCheck(Farmer.burrower) // find index of prospector check
                .Move(-1)
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)]) // the false case branch
                .GetOperand(out var isNotProspector) // copy destination
                .Return()
                .Insert(
                    [
                        // insert uncoditional branch to skip this check
                        new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector),
                    ]);
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
