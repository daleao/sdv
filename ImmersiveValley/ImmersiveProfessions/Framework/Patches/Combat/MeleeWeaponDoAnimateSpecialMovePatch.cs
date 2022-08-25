namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatch()
    {
        Target = RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    /// <summary>Patch to remove Acrobat cooldown reduction.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

        var i = 0;
    repeat:
        try
        {
            helper // find index of acrobat check
                .FindProfessionCheck(Farmer.acrobat, i != 0)
                .Retreat(2)
                .StripLabels(out var labels) // backup and remove branch labels
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
                )
                .GetOperand(out var isNotAcrobat) // copy destination
                .Return()
                .InsertInstructions( // insert unconditional branch to skip this check
                    new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat)
                )
                .Retreat()
                .AddLabels(labels) // restore bakced-up labels to inserted branch
                .Advance(3);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}");
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}