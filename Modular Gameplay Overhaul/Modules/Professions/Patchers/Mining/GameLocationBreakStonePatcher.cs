namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationBreakStonePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationBreakStonePatcher"/> class.</summary>
    internal GameLocationBreakStonePatcher()
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
                .FindProfessionCheck(Profession.Miner.Value)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_1) })
                .AddLabels(isNotPrestiged)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)4) }) // arg 4 = Farmer who
                .InsertProfessionCheck(Profession.Miner.Value + 100, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_1), new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Miner extra ores.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if (who.professions.Contains(<geologist_id> && r.NextDouble() < 0.5) switch(indexOfStone) ...
        try
        {
            helper
                .FindProfessionCheck(Farmer.geologist) // find index of geologist check
                .Move(-1)
                .StripLabels(out var labels) // backup and remove branch labels
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) }) // the false case branch
                .GetOperand(out var isNotGeologist) // copy destination
                .Return()
                .Insert(
                    new[]
                    {
                        // insert unconditional branch to skip this check and restore backed-up labels to this branch
                        new CodeInstruction(OpCodes.Br, (Label)isNotGeologist),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Geologist paired gems.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if (who.professions.Contains(<prospector_id>)) ...
        try
        {
            helper
                .FindProfessionCheck(Farmer.burrower) // find index of prospector check
                .Move(-1)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }) // the false case branch
                .GetOperand(out var isNotProspector) // copy destination
                .Return()
                .Insert(
                    new[]
                    {
                        // insert uncoditional branch to skip this check
                        new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector),
                    });
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
