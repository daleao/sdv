namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
        this.Postfix!.after = new[] { LigoModule.Arsenal.Namespace };
    }

    #region harmony patches

    /// <summary>Implement Garnet ring CDR.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("Ligo.Modules.Arsenal")]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var cdr = 10f / (10f + __instance.getLastFarmerToUse().Read<float>(DataFields.RingCooldownReduction));
        if (cdr <= 0f)
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * cdr);
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * cdr);
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * cdr);
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * cdr);
        }
    }

    #endregion harmony patches
}
