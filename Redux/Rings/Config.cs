namespace DaLion.Redux.Rings;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Rings.</summary>
public sealed class Config
{
    /// <summary>Gets a value indicating whether improves certain underwhelming rings.</summary>
    [JsonProperty]
    public bool RebalancedRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether adds new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    public bool CraftableGemRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether adds new mining recipes for crafting Glow and Magnet rings.</summary>
    [JsonProperty]
    public bool CraftableGlowAndMagnetRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replaces the Glowstone Ring recipe.</summary>
    [JsonProperty]
    public bool ImmersiveGlowstoneRecipe { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replaces the Iridium Band recipe and effect. Adds new forge mechanics.</summary>
    [JsonProperty]
    public bool TheOneIridiumBand { get; internal set; } = true;

    /// <summary>Gets a value indicating whether if <see cref="TheOneIridiumBand"/> is enabled, adds additional requirements to create the ultimate ring.</summary>
    [JsonProperty]
    public bool TheOneInfinityBand { get; internal set; } = true;
}
