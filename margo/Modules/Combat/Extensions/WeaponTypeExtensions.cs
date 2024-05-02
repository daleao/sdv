namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Enums;

#endregion using directives

/// <summary>Extensions for the <see cref="WeaponType"/> enum.</summary>
public static class WeaponTypeExtensions
{
    /// <summary>Gets the final combo hit of the <see cref="WeaponType"/>.</summary>
    /// <param name="type">The <see cref="WeaponType"/>.</param>
    /// <returns>The number of final hit for the <see cref="WeaponType"/>, as <see cref="ComboHitStep"/>.</returns>
    public static ComboHitStep GetFinalHitStep(this WeaponType type)
    {
        return type == WeaponType.Dagger
            ? ComboHitStep.FirstHit
            : (ComboHitStep)CombatModule.Config.WeaponsSlingshots.ComboHitsPerWeaponType[type.ToStringFast()];
    }
}
