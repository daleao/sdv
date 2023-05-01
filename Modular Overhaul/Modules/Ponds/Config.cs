namespace DaLion.Overhaul.Modules.Ponds;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for PNDS.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets the number of days until an empty pond will begin spawning algae.</summary>
    [JsonProperty]
    public uint DaysUntilAlgaeSpawn { get; internal set; } = 3;

    /// <summary>Gets the multiplier to a fish's base chance to produce roe each day.</summary>
    [JsonProperty]
    public float RoeProductionChanceMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether the quality of produced roe should be always the same as the quality of the producing fish. If set to false, then the quality will be less than or equal to that of the producing fish.</summary>
    [JsonProperty]
    public bool RoeAlwaysFishQuality { get; internal set; } = false;
}
