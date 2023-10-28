namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Pickaxe"/>.</summary>
public sealed class PickaxeConfig
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

    /// <summary>Gets a value indicating whether enables charging the Pick.</summary>
    [JsonProperty]
    [GMCMSection("tols.charging")]
    [GMCMPriority(10)]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets pickaxe must be at least this level to charge. Must be greater than zero.</summary>
    [JsonProperty]
    [GMCMSection("tols.charging")]
    [GMCMPriority(11)]
    [GMCMOverride(typeof(GenericModConfigMenu), "PickaxeRequiredUpgradeForChargingOverride")]
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
    [GMCMOverride(typeof(GenericModConfigMenu), "PickaxeConfigRadiusAtEachLevelOverride")]
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

    /// <summary>Gets a value indicating whether to break boulders and meteorites.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(51)]
    public bool BreakBouldersAndMeteorites { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to harvest spawned items in the mines.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(52)]
    public bool HarvestMineSpawns { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to break containers in the mine.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(53)]
    public bool BreakMineContainers { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear placed objects.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(54)]
    public bool ClearObjects { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear placed paths and flooring.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(55)]
    public bool ClearFlooring { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear tilled dirt.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(56)]
    public bool ClearDirt { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(57)]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(58)]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like stones, boulders and weeds.</summary>
    [JsonProperty]
    [GMCMSection("tols.shockwave")]
    [GMCMPriority(59)]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Pick can be enchanted with Master.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(100)]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Pick can be enchanted with Reaching.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(101)]
    public bool AllowReachingEnchantment { get; internal set; } = true;
}
