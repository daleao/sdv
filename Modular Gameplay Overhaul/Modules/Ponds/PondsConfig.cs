namespace DaLion.Overhaul.Modules.Ponds;

#region using directives

using DaLion.Shared.Configs;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Ponds.</summary>
public sealed class PondsConfig : Config
{
    /// <summary>Gets the number of days until an empty pond will begin spawning algae.</summary>
    [JsonProperty]
    public uint DaysUntilAlgaeSpawn { get; internal set; } = 3;

    /// <summary>Gets the multiplier to a fish's base chance to produce roe each day.</summary>
    [JsonProperty]
    public float RoeProductionChanceMultiplier { get; internal set; } = 1f;
}
