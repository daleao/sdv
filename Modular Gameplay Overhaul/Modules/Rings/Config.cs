namespace DaLion.Overhaul.Modules.Rings;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for RNGS.</summary>
public sealed class Config : Shared.Configs.Config
{
    #region dropdown enums

    /// <summary>The texture that should be used as the resonance light source.</summary>
    public enum ResonanceLightsourceTexture
    {
        /// <summary>The default, Vanilla sconce light texture.</summary>
        Sconce = 4,

        /// <summary>A more opaque sconce light texture.</summary>
        Stronger = 100,

        /// <summary>A floral-patterned light texture.</summary>
        Patterned = 101,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to improve certain underwhelming rings.</summary>
    [JsonProperty]
    public bool RebalancedRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    public bool CraftableGemRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to impadd new mining recipes for crafting Glow and Magnet rings.</summary>
    [JsonProperty]
    public bool BetterGlowstoneProgression { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the Iridium Band recipe and effect.</summary>
    [JsonProperty]
    public bool TheOneInfinityBand { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    public bool EnableResonance { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the resonance glow should inherit the root note's color.</summary>
    [JsonProperty]
    public bool ColorfulResonance { get; internal set; } = true;

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    public ResonanceLightsourceTexture LightsourceTexture { get; internal set; } = ResonanceLightsourceTexture.Sconce;
}
