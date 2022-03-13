namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatch()
    {
        Original = RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches
    
    /// <summary>Patch to remove Acrobat cooldown reduction.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(
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
                .Insert( // insert unconditional branch to skip this check
                    new CodeInstruction(OpCodes.Br_S, (Label) isNotAcrobat)
                )
                .Retreat()
                .AddLabels(labels) // restore bakced-up labels to inserted branch
                .Advance(3);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}