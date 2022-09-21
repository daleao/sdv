namespace DaLion.Stardew.Tools.Configs;

#region using directives

using DaLion.Stardew.Tools.Framework;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Pickaxe"/>.</summary>
public sealed class PickaxeConfig
{
    /// <summary>Gets or sets a value indicating whether enables charging the Pickaxe.</summary>
    public bool EnableCharging { get; set; } = true;

    /// <summary>Gets or sets pickaxe must be at least this level to charge. Must be greater than zero.</summary>
    public UpgradeLevel RequiredUpgradeForCharging { get; set; } = UpgradeLevel.Copper;

    /// <summary>Gets or sets the radius of affected tiles at each upgrade level.</summary>
    public int[] RadiusAtEachPowerLevel { get; set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Gets or sets a value indicating whether determines whether to break boulders and meteorites.</summary>
    public bool BreakBouldersAndMeteorites { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to harvest spawned items in the mines.</summary>
    public bool HarvestMineSpawns { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to break containers in the mine.</summary>
    public bool BreakMineContainers { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to clear placed objects.</summary>
    public bool ClearObjects { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear placed paths and flooring.</summary>
    public bool ClearFlooring { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear tilled dirt.</summary>
    public bool ClearDirt { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to clear live crops.</summary>
    public bool ClearLiveCrops { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear dead crops.</summary>
    public bool ClearDeadCrops { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to clear debris like stones, boulders and weeds.</summary>
    public bool ClearDebris { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to play the shockwave animation when the charged Pickaxe is released.</summary>
    public bool PlayShockwaveAnimation { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether the Pickaxe can be enchanted with Reaching.</summary>
    public bool AllowReachingEnchantment { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether the Pickaxe can be enchanted with Master.</summary>
    public bool AllowMasterEnchantment { get; set; } = true;
}
