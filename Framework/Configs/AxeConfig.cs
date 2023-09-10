namespace DaLion.Chargeable.Framework.Configs;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Axe"/>.</summary>
public sealed class AxeConfig
{
    /// <summary>Gets a value indicating whether enables charging the <see cref="Axe"/>.</summary>
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets axe must be at least this level to charge.</summary>
    public UpgradeLevel RequiredUpgradeForCharging { get; internal set; } = UpgradeLevel.Copper;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    public uint[] RadiusAtEachPowerLevel { get; internal set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Gets a value indicating whether to clear fruit tree seeds.</summary>
    public bool ClearFruitTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear fruit trees that aren't fully grown.</summary>
    public bool ClearFruitTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down fully-grown fruit trees.</summary>
    public bool CutGrownFruitTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit tree seeds.</summary>
    public bool ClearTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit trees that aren't fully grown.</summary>
    public bool ClearTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down full-grown non-fruit trees.</summary>
    public bool CutGrownTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down non-fruit trees that have a tapper.</summary>
    public bool CutTappedTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest giant crops.</summary>
    public bool CutGiantCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear bushes.</summary>
    public bool ClearBushes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to play the shockwave animation when the charged Axe is released.</summary>
    public bool PlayShockwaveAnimation { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Reaching.</summary>
    public bool AllowReachingEnchantment { get; internal set; } = true;

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    public float StaminaCostMultiplier { get; internal set; } = 1f;
}
