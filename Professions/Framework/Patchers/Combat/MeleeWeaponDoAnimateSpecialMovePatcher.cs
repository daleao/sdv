namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MeleeWeaponDoAnimateSpecialMovePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    /// <summary>Patch to null special move cooldown for prestiged Piper.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        if (__instance.getLastFarmerToUse() is not { } lastUser || !lastUser.HasProfession(Profession.Piper, true) ||
            !lastUser.hasBuff("13"))
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = 0;
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = 0;
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = 0;
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = 0;
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
