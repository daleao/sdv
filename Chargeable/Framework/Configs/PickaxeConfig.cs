namespace DaLion.Chargeable.Framework.Configs;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Pickaxe"/>.</summary>
public sealed class PickaxeConfig
{
    /// <summary>Gets a value indicating whether enables charging the Pick.</summary>
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets pickaxe must be at least this level to charge. Must be greater than zero.</summary>
    public UpgradeLevel RequiredUpgradeForCharging { get; internal set; } = UpgradeLevel.Copper;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    public uint[] RadiusAtEachPowerLevel { get; internal set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Gets a value indicating whether to break boulders and meteorites.</summary>
    public bool BreakBouldersAndMeteorites { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to harvest spawned items in the mines.</summary>
    public bool HarvestMineSpawns { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to break containers in the mine.</summary>
    public bool BreakMineContainers { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear placed objects.</summary>
    public bool ClearObjects { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear placed paths and flooring.</summary>
    public bool ClearFlooring { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear tilled dirt.</summary>
    public bool ClearDirt { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like stones, boulders and weeds.</summary>
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to play the shockwave animation when the charged Pick is released.</summary>
    public bool PlayShockwaveAnimation { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Pick can be enchanted with Reaching.</summary>
    public bool AllowReachingEnchantment { get; internal set; } = true;

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    public float StaminaCostMultiplier { get; internal set; } = 1f;
}
