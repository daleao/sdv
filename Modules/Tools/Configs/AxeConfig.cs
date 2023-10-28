namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Axe"/>.</summary>
public sealed class AxeConfig
{
    private float _baseStaminaCostMultiplier = 1f;
    private float _chargeStaminaCostMultiplier = 1f;
    private uint[] _radiusAtEachPowerLevel = { 1, 2, 3, 4, 5, 6, 7 };

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    [GMCMSection("general")]
    [GMCMPriority(0)]
    [GMCMRange(0f, 2f)]
    [GMCMInterval(0.05f)]
    public float BaseStaminaCostMultiplier
    {
        get => this._baseStaminaCostMultiplier;
        internal set => this._baseStaminaCostMultiplier = Math.Max(value, 0f);
    }

    /// <summary>Gets a value indicating whether enables charging the <see cref="Axe"/>.</summary>
    [JsonProperty]
    [GMCMSection("tols.charging")]
    [GMCMPriority(10)]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets axe must be at least this level to charge.</summary>
    [JsonProperty]
    [GMCMSection("tols.charging")]
    [GMCMPriority(11)]
    [GMCMOverride(typeof(GenericModConfigMenu), "AxeRequiredUpgradeForChargingOverride")]
    public UpgradeLevel RequiredUpgradeForCharging { get; internal set; } = UpgradeLevel.Copper;

    /// <summary>Gets a value which multiplies the stamina consumption for a <see cref="Shockwave"/>.</summary>
    [JsonProperty]
    [GMCMSection("tols.charging")]
    [GMCMPriority(12)]
    [GMCMRange(0f, 2f)]
    [GMCMInterval(0.05f)]
    public float ChargedStaminaCostMultiplier
    {
        get => this._chargeStaminaCostMultiplier;
        internal set => this._chargeStaminaCostMultiplier = Math.Max(value, 0f);
    }

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(50)]
    [GMCMOverride(typeof(GenericModConfigMenu), "AxeConfigRadiusAtEachLevelOverride")]
    public uint[] RadiusAtEachPowerLevel
    {
        get => this._radiusAtEachPowerLevel;
        internal set
        {
            if (value.Length < 7)
            {
                value = new uint[] { 1, 2, 3, 4, 5, 6, 7 };
            }

            this._radiusAtEachPowerLevel = value;
        }
    }

    /// <summary>Gets a value indicating whether to clear fruit tree seeds.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(51)]
    public bool ClearFruitTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(52)]
    public bool ClearFruitTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down fully-grown fruit trees.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(53)]
    public bool CutGrownFruitTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit tree seeds.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(54)]
    public bool ClearTreeSeeds { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear non-fruit trees that aren't fully grown.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(55)]
    public bool ClearTreeSaplings { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down full-grown non-fruit trees.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(56)]
    public bool CutGrownTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to cut down non-fruit trees that have a tapper.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(57)]
    public bool CutTappedTrees { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest giant crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(58)]
    public bool CutGiantCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear bushes.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(59)]
    public bool ClearBushes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(60)]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(61)]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(62)]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Master.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(100)]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Axe can be enchanted with Reaching.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(101)]
    public bool AllowReachingEnchantment { get; internal set; } = true;
}
