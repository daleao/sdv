namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to the Scythe.</summary>
public sealed class ScytheConfig
{
    /// <summary>Gets the radius of the regular Scythe.</summary>
    [JsonProperty]
    [GMCMSection("tols.affected_tiles")]
    [GMCMPriority(0)]
    [GMCMRange(1, 10)]
    public uint RegularScytheRadius { get; internal set; } = 2;

    /// <summary>Gets the radius of the Golden Scythe.</summary>
    [JsonProperty]
    [GMCMSection("tols.affected_tiles")]
    [GMCMPriority(1)]
    [GMCMRange(1, 10)]
    public uint GoldScytheRadius { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether to clear tree saplings.</summary>
    [JsonProperty]
    [GMCMSection("tols.harvest")]
    [GMCMPriority(10)]
    public bool ClearTreeSaplings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to harvest crops.</summary>
    [JsonProperty]
    [GMCMSection("tols.harvest")]
    [GMCMPriority(11)]
    public bool HarvestCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest flowers.</summary>
    [JsonProperty]
    [GMCMSection("tols.harvest")]
    [GMCMPriority(12)]
    public bool HarvestFlowers { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to harvest forage.</summary>
    [JsonProperty]
    [GMCMSection("tols.harvest")]
    [GMCMPriority(13)]
    public bool HarvestForage { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the harvest settings apply only to Golden Scythe.</summary>
    [JsonProperty]
    [GMCMSection("tols.harvest")]
    [GMCMPriority(14)]
    public bool GoldScytheOnlyHarvest { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the scythes can be enchanted with Haymaker.</summary>
    [JsonProperty]
    [GMCMSection("tols.enchantments")]
    [GMCMPriority(50)]
    public bool AllowHaymakerEnchantment { get; internal set; } = true;
}
