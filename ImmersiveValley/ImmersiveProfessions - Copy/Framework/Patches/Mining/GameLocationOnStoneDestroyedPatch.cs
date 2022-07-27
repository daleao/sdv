namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationOnStoneDestroyedPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationOnStoneDestroyedPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.OnStoneDestroyed));
    }

    #region harmony patches

    /// <summary>Patch to remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationOnStoneDestroyedTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: random.NextDouble() < 0.035 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2)
        /// To: random.NextDouble() < 0.035

        try
        {
            helper
                .FindProfessionCheck(Farmer.burrower) // find index of prospector check
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