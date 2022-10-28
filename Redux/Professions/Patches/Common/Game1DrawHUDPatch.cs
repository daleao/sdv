namespace DaLion.Redux.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawHUDPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawHUDPatch"/> class.</summary>
    internal Game1DrawHUDPatch()
    {
        this.Target = this.RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1DrawHUDTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Removed:
        //     From: if (!player.professions.Contains(<scavenger_id>)
        //     Until: end ...
        try
        {
            helper
                .FindProfessionCheck(Farmer.tracker) // find index of tracker check
                .Retreat()
                .GetLabels(out var leave) // the exception block leave opcode destination
                .GoTo(helper.LastIndex)
                .GetLabels(out var labels) // get the labels of the final return instruction
                .Return()
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Ret)) // remove everything after the profession check
                .AddWithLabels(
                    // add back a new return statement
                    labels.Take(2).Concat(leave).ToArray(), // exclude the labels defined after the profession check
                    new CodeInstruction(OpCodes.Ret));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Tracker behavior.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
