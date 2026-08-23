namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using CropCategory = DaLion.Core.Framework.CropCategory;

#endregion using directives

/// <summary>Holds maps which may be referenced by the module.</summary>
internal static class Lookups
{
    /// <summary>Gets the qualified IDs of the Artisan machines.</summary>
    internal static HashSet<string> ArtisanMachines { get; } =
    [
        QIDs.CheesePress,
        QIDs.Loom,
        QIDs.MayonnaiseMachine,
        QIDs.OilMaker,
        QIDs.PreservesJar,
        QIDs.Keg,
        QIDs.Dehydrator,
        QIDs.Cask,
    ];

    /// <summary>Gets the qualified IDs of artisan goods derived from animal produce.</summary>
    internal static HashSet<string> AnimalDerivedGoods { get; } =
    [
        QIDs.Mayonnaise,
        QIDs.DuckMayonnaise,
        QIDs.VoidMayonnaise,
        QIDs.DinosaurEgg,
        QIDs.Cheese,
        QIDs.GoatCheese,
        QIDs.Cloth,
        $"(O){UniqueId}_GoldenMayo",
        $"(O){UniqueId}_OstrichMayo",
    ];

    /// <summary>Gets the IDs of resource nodes.</summary>
    internal static HashSet<string> ResourceNodeIds { get; } =
    [
        QIDs.Stone_Node_Copper0,
        QIDs.Stone_Node_Copper1,
        QIDs.Stone_Node_Iron0,
        QIDs.Stone_Node_Iron1,
        QIDs.Stone_Node_Gold,
        QIDs.Stone_Node_Iridium,
        QIDs.Stone_Node_Radioactive,
        QIDs.Stone_Node_Geode,
        QIDs.Stone_Node_FrozenGeode,
        QIDs.Stone_Node_MagmaGeode,
        QIDs.Stone_Node_OmniGeode,
        QIDs.Stone_Node_Diamond,
        QIDs.Stone_Node_Ruby,
        QIDs.Stone_Node_Jade,
        QIDs.Stone_Node_Amethyst,
        QIDs.Stone_Node_Topaz,
        QIDs.Stone_Node_Emerald,
        QIDs.Stone_Node_Aquamarine,
        QIDs.Stone_Node_Gemstone,
        QIDs.Stone_Node_Mussel,
        QIDs.Stone_Node_BoneFragment0,
        QIDs.Stone_Node_BoneFragment1,
        QIDs.Stone_Node_Clay,
        QIDs.Stone_Node_CinderShard0,
        QIDs.Stone_Node_CinderShard1,
        QIDs.Stone_Node_MysticStone
    ];

    /// <summary>Gets the IDs of (valuable) resource clumps.</summary>
    internal static HashSet<int> ResourceClumpIds { get; } = [];

    /// <summary>Gets the seed object ID from the corresponding crop ID.</summary>
    internal static Dictionary<string, string> SeedByCrop { get; } = [];

    /// <summary>Gets the respective <see cref="MachineTreatmentRules"/> for each artisan machine.</summary>
    internal static Dictionary<string, MachineTreatmentRules> MachineTreatments { get; } = [];

    /// <summary>Gets the corresponding items that can be used to apply any <see cref="MachineTreatmentCategory"/>.</summary>
    internal static Dictionary<MachineTreatmentCategory, string[]> TreatmentsByCategory { get; } = new()
    {
        { MachineTreatmentCategory.Overclock, [ QIDs.BatteryPack ] },
        { MachineTreatmentCategory.Fermentation, [ QIDs.OakResin ] },
        { MachineTreatmentCategory.Glazing, [ QIDs.MapleSyrup, SveIntegration.BIRCH_WATER_QID ] },
        { MachineTreatmentCategory.Sealing, [ QIDs.PineTar, SveIntegration.FIR_WAX_QID ] },
    };

    /// <summary>Gets the corresponding <see cref="MachineTreatmentCategory"/> for each valid treatment item.</summary>
    internal static Dictionary<string, MachineTreatmentCategory> CategoryByTreatment { get; } = new()
    {
        { QIDs.BatteryPack, MachineTreatmentCategory.Overclock },
        { QIDs.OakResin, MachineTreatmentCategory.Fermentation },
        { QIDs.MapleSyrup, MachineTreatmentCategory.Glazing },
        { QIDs.PineTar, MachineTreatmentCategory.Sealing },
        { SveIntegration.BIRCH_WATER_QID, MachineTreatmentCategory.Glazing },
        { SveIntegration.FIR_WAX_QID, MachineTreatmentCategory.Sealing },
    };

    /// <summary>Gets a list of vegetables belonging to each category.</summary>
    public static Dictionary<CropCategory, HashSet<string>> CropsByCategory { get; } = new()
    {
        { CropCategory.Grains, [] },
        { CropCategory.LeafyGreens, [] },
        { CropCategory.Legumes, [] },
        { CropCategory.Roots, [] },
        { CropCategory.Tubers, [] },
        { CropCategory.Gourds, [] },
    };

    /// <summary>Gets a list of vegetables belonging to each category.</summary>
    public static Dictionary<string, CropCategory> CategoryByCrop { get; } = [];

    /// <summary>Gets the feeds favored by each animal type.</summary>
    internal static Dictionary<string, HashSet<CropCategory>> AnimalFavoredFeeds { get; } = [];

    /// <summary>Gets or sets arrays of mammals or egg-layers.</summary>
    internal static AnimalsByReproductiveType AnimalReproductiveTypes { get; set; } = new([], []);
}
