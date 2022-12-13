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
internal sealed class GameLocationOnStoneDestroyedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationOnStoneDestroyedPatcher"/> class.</summary>
    internal GameLocationOnStoneDestroyedPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.OnStoneDestroyed));
    }

    #region harmony patches

    /// <summary>Patch to remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationOnStoneDestroyedTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: random.NextDouble() < 0.035 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2)
        // To: random.NextDouble() < 0.035
        try
        {
            helper
                .FindProfessionCheck(Farmer.burrower) // find index of prospector check
                .Move(-1)
                .Match(new[] { new CodeInstruction(OpCodes.Mul) }, out var count)
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
