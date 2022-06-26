namespace DaLion.Stardew.Tools.Configs;

#region using directives

using Framework;

#endregion using directives

/// <summary>Configs related to the Pickaxe.</summary>
public class PickaxeConfig
{
    /// <summary>Enables charging the Pickaxe.</summary>
    public bool EnableCharging { get; set; } = true;

    /// <summary>Pickaxe must be at least this level to charge. Must be greater than zero.</summary>
    public UpgradeLevel RequiredUpgradeForCharging { get; set; } = UpgradeLevel.Copper;

    /// <summary>The radius of affected tiles at each upgrade level.</summary>
    public int[] RadiusAtEachPowerLevel { get; set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Whether to break boulders and meteorites.</summary>
    public bool BreakBouldersAndMeteorites { get; set; } = true;

    /// <summary>Whether to harvest spawned items in the mines.</summary>
    public bool HarvestMineSpawns { get; set; } = true;

    /// <summary>Whether to break containers in the mine.</summary>
    public bool BreakMineContainers { get; set; } = true;

    /// <summary>Whether to clear placed objects.</summary>
    public bool ClearObjects { get; set; } = false;

    /// <summary>Whether to clear placed paths & flooring.</summary>
    public bool ClearFlooring { get; set; } = false;

    /// <summary>Whether to clear tilled dirt.</summary>
    public bool ClearDirt { get; set; } = true;

    /// <summary>Whether to clear bushes.</summary>
    public bool ClearBushes { get; set; } = true;

    /// <summary>Whether to clear live crops.</summary>
    public bool ClearLiveCrops { get; set; } = false;

    /// <summary>Whether to clear dead crops.</summary>
    public bool ClearDeadCrops { get; set; } = true;

    /// <summary>Whether to clear debris like stones, boulders and weeds.</summary>
    public bool ClearDebris { get; set; } = true;

    /// <summary>Whether to play the shockwave animation when the charged Pickaxe is released.</summary>
    public bool PlayShockwaveAnimation { get; set; } = true;

    /// <summary>Whether the Pickaxe can be enchanted with Reaching.</summary>
    public bool AllowReachingEnchantment { get; set; } = true;
}