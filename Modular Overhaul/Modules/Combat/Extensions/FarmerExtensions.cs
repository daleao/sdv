namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Gets the overhauled total resilience of the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The total resilience, a number between 0 and 1.</returns>
    internal static float GetOverhauledResilience(this Farmer farmer)
    {
        var weaponResilience = farmer.CurrentTool switch
        {
            MeleeWeapon weapon => WeaponsModule.ShouldEnable
                ? weapon.Get_EffectiveResilience()
                : 10f / (10 + weapon.addedDefense.Value),
            Slingshot slingshot => SlingshotsModule.ShouldEnable && SlingshotsModule.Config.EnableEnchantments
                ? slingshot.Get_TopazResilience()
                : 0f,
            _ => 1f,
        };

        var playerResilience = farmer.resilience + farmer.Get_ResonantResilience();
        return weaponResilience * (10f / (10f + playerResilience));
    }
}
