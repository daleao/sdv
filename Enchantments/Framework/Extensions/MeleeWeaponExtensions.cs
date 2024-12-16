namespace DaLion.Enchantments.Framework.Extensions;

#region using directives

using System.Diagnostics;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class MeleeWeaponExtensions
{
    internal const int ATTACK_SWORD_COOLDOWN_TIME = 2000;

    [Conditional("RELEASE")]
    internal static void DoStabbingSpecialCooldown(this MeleeWeapon weapon)
    {
        var user = weapon.lastUser;
        MeleeWeapon.attackSwordCooldown = ATTACK_SWORD_COOLDOWN_TIME;
        if (!ModHelper.ModRegistry.IsLoaded("DaLion.Professions") && user.professions.Contains(Farmer.acrobat))
        {
            MeleeWeapon.attackSwordCooldown /= 2;
        }
    }

    /// <summary>Gets the name of the sound cue that should play when this weapon is swung.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The name of a sound cue to be played.</returns>
    internal static string GetSwipeSound(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => "swordswipe",
            MeleeWeapon.club => "clubswipe",
            MeleeWeapon.dagger => "daggerswipe",
        };
    }
}
