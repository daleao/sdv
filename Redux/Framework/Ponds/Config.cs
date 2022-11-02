namespace DaLion.Redux.Framework.Ponds;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Ponds.</summary>
public sealed class Config
{
    /// <summary>Gets the number of days until an empty pond will begin spawning algae.</summary>
    [JsonProperty]
    public int DaysUntilAlgaeSpawn { get; internal set; } = 3;

    /// <summary>Gets the multiplier to a fish's base chance to produce roe each day.</summary>
    [JsonProperty]
    public float RoeProductionChanceMultiplier { get; internal set; } = 1f;
}
