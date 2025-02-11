namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
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

    /// <summary>Implement Garnet enchantment CDR.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        if (__instance.getLastFarmerToUse() is not { } lastUser)
        {
            return;
        }

        var cooldownReductionMultiplier = 2f - ((1f + __instance.Get_CooldownReduction().Value) * (1f + lastUser.Get_CooldownReduction().Value));
        if (cooldownReductionMultiplier > 0.99f)
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * cooldownReductionMultiplier);
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * cooldownReductionMultiplier);
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * cooldownReductionMultiplier);
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * cooldownReductionMultiplier);
        }
    }

    #endregion harmony patches
}
