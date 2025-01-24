﻿namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDoAnimateSpecialMovePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void MeleeWeaponDoAnimateSpecialMovePrefix(MeleeWeapon __instance)
    {
        if (__instance.lastUser is { IsLocalPlayer: true } user && user.CurrentTool == __instance &&
            State.LimitBreak is PoacherAmbush { IsActive: true } ambush)
        {
            ambush.Deactivate();
        }
    }

    /// <summary>Patch to remove Acrobat cooldown reduction.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2
        try
        {
            helper
                .Repeat(
                    3,
                    i =>
                {
                    helper
                        // find index of acrobat check
                        .MatchProfessionCheck(
                            Farmer.acrobat,
                            i == 0 ? ILHelper.SearchOption.First : ILHelper.SearchOption.Next)
                        .Move(-2)
                        .StripLabels(out var labels) // backup and remove branch labels
                        .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)]) // the false case branch
                        .GetOperand(out var isNotAcrobat) // copy destination
                        .Return()
                        .Insert(
                            [
                                // insert unconditional branch to skip this check
                                new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat),
                            ])
                        .Move(-1)
                        .AddLabels(labels) // restore bakced-up labels to inserted branch
                        .Move(3);
                });
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
