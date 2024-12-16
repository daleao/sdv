namespace DaLion.Arsenal;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Config schema for the Arsenal mod.</summary>
public sealed class ArsenalConfig
{
    /// <summary>Gets a value indicating whether to enable Dwarven Legacy quest-line for Masterworked weapons.</summary>
    [JsonProperty]
    public bool DwarvenLegacy { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable Hero's Journey quest-line for Infinity weapons.</summary>
    [JsonProperty]
    public bool InfinityPlusOne { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to color-code tool names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    [GMCMPriority(507)]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    [GMCMPriority(508)]
    [GMCMOverride(typeof(ArsenalConfigMenu), "CombatConfigColorByTierOverride")]
    public Color[] ColorByTier { get; internal set; } =
    {
        new(34, 17, 34),
        Color.Green,
        Color.Blue,
        Color.Purple,
        Color.Red,
        Color.MonoGameOrange,
        Color.MonoGameOrange,
    };
}
