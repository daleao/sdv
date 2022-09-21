namespace DaLion.Stardew.Rings;

/// <summary>The mod user-defined settings.</summary>
public sealed class ModConfig
{
    /// <summary>Gets or sets a value indicating whether improves certain underwhelming rings.</summary>
    public bool RebalancedRings { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether adds new combat recipes for crafting gemstone rings.</summary>
    public bool CraftableGemRings { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether adds new mining recipes for crafting Glow and Magnet rings.</summary>
    public bool CraftableGlowAndMagnetRings { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether replaces the Glowstone Ring recipe.</summary>
    public bool ImmersiveGlowstoneRecipe { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether replaces the Iridium Band recipe and effect. Adds new forge mechanics.</summary>
    public bool TheOneIridiumBand { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether if <see cref="TheOneIridiumBand"/> is enabled, adds additional requirements to create the ultimate ring.</summary>
    public bool TheOneInfinityBand { get; set; } = true;
}
