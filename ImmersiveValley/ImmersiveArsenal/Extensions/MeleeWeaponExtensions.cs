namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using DaLion.Common.Enums;
using DaLion.Common.Exceptions;
using DaLion.Stardew.Arsenal.Framework;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
public static class MeleeWeaponExtensions
{
    /// <summary>Determines whether the <paramref name="weapon"/> is an Infinity weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/>'s index correspond to one of the Infinity weapon, otherwise <see langword="false"/>.</returns>
    public static bool IsInfinityWeapon(this MeleeWeapon weapon)
    {
        return weapon.InitialParentTileIndex is Constants.InfinityBladeIndex or Constants.InfinityDaggerIndex
            or Constants.InfinityClubIndex;
    }

    /// <summary>Gets the final <see cref="ComboHitStep"/> for this <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The final <see cref="ComboHitStep"/> for <paramref name="weapon"/>.</returns>
    public static ComboHitStep GetFinalComboHitStep(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => weapon.hasEnchantmentOfType<InfinityEnchantment>()
                ? ComboHitStep.FourthHit
                : ComboHitStep.ThirdHit,
            MeleeWeapon.club => ComboHitStep.SecondHit,
            MeleeWeapon.dagger => ComboHitStep.Idle,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<ComboHitStep, ComboHitStep>(weapon.type.Value),
        };
    }

    /// <summary>Analogous to <see cref="MeleeWeapon.doSwipe"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="type">The <see cref="MeleeWeapon"/> type.</param>
    /// <param name="position">The <see cref="Farmer"/>'s position.</param>
    /// <param name="facingDirection">The <see cref="Farmer"/>'s <see cref="FacingDirection"/>.</param>
    /// <param name="swipeSpeed">The <paramref name="weapon"/>'s swing speed.</param>
    /// <param name="f">The <see cref="Farmer"/>.</param>
    public static void DoSecondSwipe(
        this MeleeWeapon weapon, int type, Vector2 position, int facingDirection, float swipeSpeed, Farmer f)
    {

    }
}
