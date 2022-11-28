namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using DaLion.Shared.Enums;
using NetEscapades.EnumGenerators;
using StardewValley.Tools;

#endregion using directives

/// <summary>The type of a <see cref="MeleeWeapon"/>.</summary>
[EnumExtensions]
public enum WeaponType
{
    /// <summary>The <see cref="MeleeWeapon.stabbingSword"/>.</summary>
    StabbingSword,

    /// <summary>The <see cref="MeleeWeapon.dagger"/>.</summary>
    Dagger,

    /// <summary>The <see cref="MeleeWeapon.club"/>.</summary>
    Club,

    /// <summary>The <see cref="MeleeWeapon.defenseSword"/>.</summary>
    DefenseSword,
}

/// <summary>Extensions for the <see cref="FacingDirection"/> enum.</summary>
public static partial class WeaponTypeExtensions
{
    /// <summary>Gets the final combo hit of the <see cref="WeaponType"/>.</summary>
    /// <param name="type">The <see cref="WeaponType"/>.</param>
    /// <returns>The number of final hit for the <see cref="WeaponType"/>, as <see cref="ComboHitStep"/>.</returns>
    public static ComboHitStep GetFinalHitStep(this WeaponType type)
    {
        return type == WeaponType.Dagger
            ? ComboHitStep.FirstHit
            : (ComboHitStep)ModEntry.Config.Arsenal.Weapons.ComboHitsPerWeapon[type];
    }
}
