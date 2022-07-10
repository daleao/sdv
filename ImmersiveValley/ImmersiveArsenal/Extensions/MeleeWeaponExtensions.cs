namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
public static class MeleeWeaponExtensions
{
    /// <summary>Whether this weapon is an Infinity weapon.</summary>
    public static bool IsInfinityWeapon(this MeleeWeapon weapon) =>
        weapon.InitialParentTileIndex is Constants.INFINITY_BLADE_INDEX_I or Constants.INFINITY_DAGGER_INDEX_I
            or Constants.INFINITY_CLUB_INDEX_I;
}