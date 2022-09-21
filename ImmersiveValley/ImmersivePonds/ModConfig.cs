namespace DaLion.Stardew.Ponds;

/// <summary>The mod user-defined settings.</summary>
internal sealed class ModConfig
{
    /// <summary>Gets or sets the number of days until an empty pond will begin spawning algae.</summary>
    public int DaysUntilAlgaeSpawn { get; set; } = 2;

    /// <summary>Gets or sets the multiplier to a fish's base chance to produce roe each day.</summary>
    public float RoeProductionChanceMultiplier { get; set; } = 1f;
}
