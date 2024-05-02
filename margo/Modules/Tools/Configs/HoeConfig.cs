namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="Hoe"/>.</summary>
public sealed class HoeConfig
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

    /// <summary>Gets the area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    [GMCMSection("tols.affected_tiles")]
    [GMCMPriority(10)]
    [GMCMOverride(typeof(GenericModConfigMenu), "HoeConfigAffectedTilesAtEachPowerLevelOverride")]
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

    /// <summary>Gets a value indicating whether the Hoe can be enchanted with Master.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(50)]
    public bool AllowMasterEnchantment { get; internal set; } = true;
}
