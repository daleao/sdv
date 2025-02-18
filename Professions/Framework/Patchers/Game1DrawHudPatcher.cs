﻿namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawHudPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawHudPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Game1DrawHudPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? Game1DrawHUDTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed:
        //     From: if (!player.professions.Contains(<scavenger_id>)
        //     Until: end ...
        try
        {
            helper
                .MatchProfessionCheck(Farmer.tracker) // find index of tracker check
                .Move(-1)
                .GetLabels(out var leave) // the exception block leave opcode destination
                .GoTo(helper.LastIndex)
                .GetLabels(out var labels) // get the labels of the final return instruction
                .Return()
                .CountUntil(
                    [new CodeInstruction(OpCodes.Ret)],
                    out var count) // remove everything after the profession check up until the return instruction
                .Remove(count - 1)
                .AddLabels(
                    labels.Take(2).Concat(leave).ToArray()); // exclude the labels defined after the profession check
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Tracker behavior.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
