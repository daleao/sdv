namespace DaLion.Stardew.Tools.Configs;

#region using directives

using DaLion.Stardew.Tools.Framework;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Axe"/>.</summary>
public sealed class AxeConfig
{
    /// <summary>Gets or sets a value indicating whether enables charging the Axe.</summary>
    public bool EnableCharging { get; set; } = true;

    /// <summary>Gets or sets axe must be at least this level to charge.</summary>
    public UpgradeLevel RequiredUpgradeForCharging { get; set; } = UpgradeLevel.Copper;

    /// <summary>Gets or sets the radius of affected tiles at each upgrade level.</summary>
    public int[] RadiusAtEachPowerLevel { get; set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Gets or sets a value indicating whether determines whether to clear fruit tree seeds.</summary>
    public bool ClearFruitTreeSeeds { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear fruit trees that aren't fully grown.</summary>
    public bool ClearFruitTreeSaplings { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to cut down fully-grown fruit trees.</summary>
    public bool CutGrownFruitTrees { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear non-fruit tree seeds.</summary>
    public bool ClearTreeSeeds { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear non-fruit trees that aren't fully grown.</summary>
    public bool ClearTreeSaplings { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to cut down full-grown non-fruit trees.</summary>
    public bool CutGrownTrees { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to cut down non-fruit trees that have a tapper.</summary>
    public bool CutTappedTrees { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to harvest giant crops.</summary>
    public bool CutGiantCrops { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear bushes.</summary>
    public bool ClearBushes { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to clear live crops.</summary>
    public bool ClearLiveCrops { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether to clear dead crops.</summary>
    public bool ClearDeadCrops { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
    public bool ClearDebris { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether to play the shockwave animation when the charged Axe is released.</summary>
    public bool PlayShockwaveAnimation { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether the Axe can be enchanted with Reaching.</summary>
    public bool AllowReachingEnchantment { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether the Axe can be enchanted with Master.</summary>
    public bool AllowMasterEnchantment { get; set; } = true;
}
