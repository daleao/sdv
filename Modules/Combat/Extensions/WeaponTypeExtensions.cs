namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;

#endregion using directives

/// <summary>Extensions for the <see cref="WeaponType"/> enum.</summary>
public static partial class WeaponTypeExtensions
{
    /// <summary>Gets or sets the swords that should be converted to Stabbing Swords.</summary>
    internal static HashSet<int> StabbingSwords { get; set; } = new()
    {
        WeaponIds.BoneSword,
        WeaponIds.SteelSmallsword,
        WeaponIds.Cutlass,
        WeaponIds.Rapier,
        WeaponIds.SteelFalchion,
        WeaponIds.PiratesSword,
        WeaponIds.LavaKatana,
        WeaponIds.DragontoothCutlass,
        WeaponIds.DarkSword,
    };

    /// <summary>Gets the final combo hit of the <see cref="WeaponType"/>.</summary>
    /// <param name="type">The <see cref="WeaponType"/>.</param>
    /// <returns>The number of final hit for the <see cref="WeaponType"/>, as <see cref="ComboHitStep"/>.</returns>
    public static ComboHitStep GetFinalHitStep(this WeaponType type)
    {
        return type == WeaponType.Dagger
            ? ComboHitStep.FirstHit
            : (ComboHitStep)CombatModule.Config.ComboHitsPerWeapon[type];
    }
}
