namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="WateringCan"/>.</summary>
public sealed class WateringCanConfig
{
    private float _baseStaminaCostMultiplier = 1f;
    private (uint Length, uint Radius)[] _affectedTilesAtEachPowerLevel = { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4) };

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

    /// <summary>Gets the area of affected tiles at each power level for the Can, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    [GMCMSection("tols.affected_tiles")]
    [GMCMPriority(10)]
    [GMCMOverride(typeof(GenericModConfigMenu), "WateringCanConfigAffectedTilesAtEachPowerLevelOverride")]
    public (uint Length, uint Radius)[] AffectedTilesAtEachPowerLevel
    {
        get => this._affectedTilesAtEachPowerLevel;
        internal set
        {
            if (value.Length < 7)
            {
                value = new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4) };
            }

            this._affectedTilesAtEachPowerLevel = value;
        }
    }

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Master.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(50)]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Swift.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(51)]
    public bool AllowSwiftEnchantment { get; internal set; } = true;

    /// <summary>Gets the chance to reward farming experience when watering a cropped tile.</summary>
    [JsonProperty]
    [GMCMRange(0f, 1f)]
    [GMCMSection("other")]
    [GMCMPriority(1000)]
    public float CanExpRewardChance { get; internal set; } = 0.5f;

    /// <summary>Gets the amount of farming experience rewarded for watering a cropped tile.</summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(101)]
    [GMCMRange(0, 10)]
    public int CanExpRewardAmount { get; internal set; } = 1;

    /// <summary>Gets a value indicating whether to prevent refilling the can with salt or ocean water.</summary>
    [JsonProperty]
    [GMCMSection("other")]
    [GMCMPriority(102)]
    public bool PreventSaltWaterRefill { get; internal set; } = true;
}
