namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using DaLion.Common.Exceptions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Arsenal.Framework;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
public static class MeleeWeaponExtensions
{
    private static readonly Lazy<Action<MeleeWeapon, bool>> SetAnotherClick = new(() =>
        typeof(MeleeWeapon)
            .RequireField("anotherClick")
            .CompileUnboundFieldSetterDelegate<MeleeWeapon, bool>());

    private static readonly Lazy<Action<MeleeWeapon, bool>> SetHasBegunWeaponEndPause = new(() =>
        typeof(MeleeWeapon)
            .RequireField("hasBegunWeaponEndPause")
            .CompileUnboundFieldSetterDelegate<MeleeWeapon, bool>());

    private static readonly Lazy<Func<MeleeWeapon, int>> GetSwipeSpeed = new(() =>
        typeof(MeleeWeapon)
            .RequireField("swipeSpeed")
            .CompileUnboundFieldGetterDelegate<MeleeWeapon, int>());

    /// <summary>Determines whether the <paramref name="weapon"/> is an Infinity weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/>'s index correspond to one of the Infinity weapon, otherwise <see langword="false"/>.</returns>
    public static bool IsInfinityWeapon(this MeleeWeapon weapon)
    {
        return weapon.InitialParentTileIndex is Constants.InfinityBladeIndex or Constants.InfinityDaggerIndex
            or Constants.InfinityClubIndex;
    }

    /// <summary>Gets the maximum number of hits in a combo for this <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The final <see cref="ComboHitStep"/> for <paramref name="weapon"/>.</returns>
    public static int GetMaxComboHitCount(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => weapon.hasEnchantmentOfType<InfinityEnchantment>()
                ? 4
                : 3,
            MeleeWeapon.club => 2,
            MeleeWeapon.dagger => 1,
            _ => 0,
        };
    }
}
