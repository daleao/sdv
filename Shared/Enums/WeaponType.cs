namespace DaLion.Shared.Enums;

#region using directives

using NetEscapades.EnumGenerators;
using StardewValley.Tools;

#endregion using directives

/// <summary>The actual type of <see cref="MeleeWeapon"/> or <see cref="StardewValley.Tools.Slingshot"/>.</summary>
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
