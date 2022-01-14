namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

using Stardew.Common.Harmony;
using Extensions;

using Professions = Utility.Professions;

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

    /// <summary>Patch to reduce special move cooldown for prestiged Brute and Poacher.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var who = __instance.getLastFarmerToUse();
        if (!who.IsLocalPlayer) return;

        switch (__instance.type.Value)
        {
            case MeleeWeapon.dagger:
                if (who.HasPrestigedProfession("Brute"))
                    MeleeWeapon.daggerCooldown =
                        (int) (MeleeWeapon.daggerCooldown *
                               Professions.GetPrestigedBruteCooldownReduction(who));
                else if (who.HasPrestigedProfession("Poacher"))
                    MeleeWeapon.daggerCooldown =
                        (int) (MeleeWeapon.daggerCooldown *
                               Professions.GetPrestigedPoacherCooldownReduction(who));
                break;

            case MeleeWeapon.club:
                if (who.HasPrestigedProfession("Brute"))
                    MeleeWeapon.clubCooldown =
                        (int) (MeleeWeapon.clubCooldown *
                               Professions.GetPrestigedBruteCooldownReduction(who));
                else if (who.HasPrestigedProfession("Poacher"))
                    MeleeWeapon.clubCooldown =
                        (int) (MeleeWeapon.clubCooldown *
                               Professions.GetPrestigedPoacherCooldownReduction(who));
                break;

            case MeleeWeapon.defenseSword:
                if (who.HasPrestigedProfession("Brute"))
                    MeleeWeapon.defenseCooldown =
                        (int) (MeleeWeapon.defenseCooldown *
                               Professions.GetPrestigedBruteCooldownReduction(who));
                else if (who.HasPrestigedProfession("Poacher"))
                    MeleeWeapon.defenseCooldown =
                        (int) (MeleeWeapon.defenseCooldown *
                               Professions.GetPrestigedPoacherCooldownReduction(who));
                break;
        }
    }

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
            ModEntry.Log($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}