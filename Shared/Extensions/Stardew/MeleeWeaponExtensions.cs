namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
public static class MeleeWeaponExtensions
{
    /// <summary>Checks whether the <paramref name="weapon"/> is a sword.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a sword, otherwise <see langword="false"/>.</returns>
    public static bool IsSword(this MeleeWeapon weapon)
    {
        return weapon.type.Value is MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword;
    }

    /// <summary>Checks whether the <paramref name="weapon"/> is a dagger.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a dagger, otherwise <see langword="false"/>.</returns>
    public static bool IsDagger(this MeleeWeapon weapon)
    {
        return weapon.type.Value == MeleeWeapon.dagger;
    }

    /// <summary>Checks whether the <paramref name="weapon"/> is a club.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a club, otherwise <see langword="false"/>.</returns>
    public static bool IsClub(this MeleeWeapon weapon)
    {
        return weapon.type.Value == MeleeWeapon.club;
    }
}
