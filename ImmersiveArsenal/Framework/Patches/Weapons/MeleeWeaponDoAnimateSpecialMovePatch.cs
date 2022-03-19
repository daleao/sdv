namespace DaLion.Stardew.Arsenal.Framework.Patches.Weapons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

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

    /// <summary>Adds stamina cost to dagger and club special moves.</summary
    [HarmonyPostfix]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        if (!ModEntry.Config.WeaponsCostStamina || !__instance.isOnSpecial ||
            __instance.type.Value is MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword) return;

#pragma warning disable CS8509
        var multiplier = __instance.type.Value switch
#pragma warning restore CS8509
        {
            MeleeWeapon.dagger => 1f,
            MeleeWeapon.club => 4f,
        };

        var who = __instance.getLastFarmerToUse();
        who.Stamina -= (4 - who.CombatLevel * 0.1f) * multiplier;
    }

    #endregion harmony patches
}