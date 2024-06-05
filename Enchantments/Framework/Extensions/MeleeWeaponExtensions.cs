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
}
