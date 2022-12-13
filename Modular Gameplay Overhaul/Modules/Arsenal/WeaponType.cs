namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using DaLion.Shared.Enums;
using NetEscapades.EnumGenerators;
using StardewValley.Tools;

#endregion using directives

/// <summary>The type of a <see cref="MeleeWeapon"/>.</summary>
[EnumExtensions]
public enum WeaponType
{
    /// <summary>The <see cref="MeleeWeapon.stabbingSword"/> type.</summary>
    StabbingSword,

    /// <summary>The <see cref="MeleeWeapon.dagger"/> type.</summary>
    Dagger,

    /// <summary>The <see cref="MeleeWeapon.club"/> type.</summary>
    Club,

    /// <summary>The <see cref="MeleeWeapon.defenseSword"/> type.</summary>
    DefenseSword,

    /// <summary>The <see cref="Slingshot"/> type.</summary>
    Slingshot,
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
            : (ComboHitStep)ArsenalModule.Config.Weapons.ComboHitsPerWeapon[type];
    }
}
