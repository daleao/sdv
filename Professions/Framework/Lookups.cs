namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Classes;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets the qualified IDs of the Artisan machines.</summary>
    internal static HashSet<string> ArtisanMachines { get; } =
    [
        QualifiedBigCraftableIds.Cask,
        QualifiedBigCraftableIds.CheesePress,
        QualifiedBigCraftableIds.Loom,
        QualifiedBigCraftableIds.MayonnaiseMachine,
        QualifiedBigCraftableIds.OilMaker,
        QualifiedBigCraftableIds.PreservesJar,
        QualifiedBigCraftableIds.Keg,
        QualifiedBigCraftableIds.Dehydrator,
        QualifiedBigCraftableIds.FishSmoker,
    ];

    /// <summary>Gets the qualified IDs of artisan goods derived from animal produce.</summary>
    internal static HashSet<string> AnimalDerivedGoods { get; } =
    [
        QualifiedObjectIds.Mayonnaise,
        QualifiedObjectIds.DuckMayonnaise,
        QualifiedObjectIds.VoidMayonnaise,
        QualifiedObjectIds.DinosaurEgg,
        QualifiedObjectIds.Cheese,
        QualifiedObjectIds.GoatCheese,
        QualifiedObjectIds.Cloth,
        $"(O){UniqueId}/GoldenMayo",
        $"(O){UniqueId}/OstrichMayo",
    ];

    /// <summary>Gets the qualified IDs of the legendary and extended family fish.</summary>
    internal static HashSet<string> LegendaryFishes { get; } =
    [
        QualifiedObjectIds.Crimsonfish,
        QualifiedObjectIds.Angler,
        QualifiedObjectIds.Legend,
        QualifiedObjectIds.Glacierfish,
        QualifiedObjectIds.MutantCarp,
        QualifiedObjectIds.SonOfCrimsonfish,
        QualifiedObjectIds.MsAngler,
        QualifiedObjectIds.LegendII,
        QualifiedObjectIds.GlacierfishJr,
        QualifiedObjectIds.RadioactiveCarp
    ];

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static BiMap<string, string> FamilyPairs { get; } = new(new Dictionary<string, string>
    {
        { QualifiedObjectIds.Crimsonfish, QualifiedObjectIds.SonOfCrimsonfish },
        { QualifiedObjectIds.Angler, QualifiedObjectIds.MsAngler },
        { QualifiedObjectIds.Legend, QualifiedObjectIds.LegendII },
        { QualifiedObjectIds.MutantCarp, QualifiedObjectIds.RadioactiveCarp },
        { QualifiedObjectIds.Glacierfish, QualifiedObjectIds.GlacierfishJr },
    });

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static HashSet<string> ResourceNodeIds { get; set; } =
    [
        QualifiedObjectIds.Stone_Node_Copper0,
        QualifiedObjectIds.Stone_Node_Copper1,
        QualifiedObjectIds.Stone_Node_Iron0,
        QualifiedObjectIds.Stone_Node_Iron1,
        QualifiedObjectIds.Stone_Node_Gold,
        QualifiedObjectIds.Stone_Node_Iridium,
        QualifiedObjectIds.Stone_Node_Radioactive,
        QualifiedObjectIds.Stone_Node_Geode,
        QualifiedObjectIds.Stone_Node_FrozenGeode,
        QualifiedObjectIds.Stone_Node_MagmaGeode,
        QualifiedObjectIds.Stone_Node_OmniGeode,
        QualifiedObjectIds.Stone_Node_Diamond,
        QualifiedObjectIds.Stone_Node_Ruby,
        QualifiedObjectIds.Stone_Node_Jade,
        QualifiedObjectIds.Stone_Node_Amethyst,
        QualifiedObjectIds.Stone_Node_Topaz,
        QualifiedObjectIds.Stone_Node_Emerald,
        QualifiedObjectIds.Stone_Node_Aquamarine,
        QualifiedObjectIds.Stone_Node_Gemstone,
        QualifiedObjectIds.Stone_Node_Mussel,
        QualifiedObjectIds.Stone_Node_BoneFragment0,
        QualifiedObjectIds.Stone_Node_BoneFragment1,
        QualifiedObjectIds.Stone_Node_Clay,
        QualifiedObjectIds.Stone_Node_CinderShard0,
        QualifiedObjectIds.Stone_Node_CinderShard1,
        QualifiedObjectIds.Stone_Node_MysticStone
    ];

    /// <summary>Gets or sets the ids of (valuable) resource clumps.</summary>
    internal static HashSet<int> ResourceClumpIds { get; set; } = [];

    /// <summary>Gets a map from stone node ID to its corresponding resource ID.</summary>
    internal static Dictionary<string, string> ResourceFromNode { get; } = new()
    {
        { QualifiedObjectIds.Stone_Node_Regular0, QualifiedObjectIds.Stone },
        { QualifiedObjectIds.Stone_Node_Regular1, QualifiedObjectIds.Stone },
        { QualifiedObjectIds.Stone_Node_Regular2, QualifiedObjectIds.Stone },
        { QualifiedObjectIds.Stone_Node_Regular3, QualifiedObjectIds.Stone },
        { QualifiedObjectIds.Stone_Node_Regular4, QualifiedObjectIds.Stone },
        { QualifiedObjectIds.Stone_Node_Copper0, QualifiedObjectIds.CopperOre },
        { QualifiedObjectIds.Stone_Node_Copper1, QualifiedObjectIds.CopperOre },
        { QualifiedObjectIds.Stone_Node_Iron0, QualifiedObjectIds.IronOre },
        { QualifiedObjectIds.Stone_Node_Iron1, QualifiedObjectIds.IronOre },
        { QualifiedObjectIds.Stone_Node_Gold, QualifiedObjectIds.GoldOre },
        { QualifiedObjectIds.Stone_Node_Iridium, QualifiedObjectIds.IridiumOre },
        { QualifiedObjectIds.Stone_Node_Radioactive, QualifiedObjectIds.RadioactiveOre },
        { QualifiedObjectIds.Stone_Node_Geode, QualifiedObjectIds.Geode },
        { QualifiedObjectIds.Stone_Node_FrozenGeode, QualifiedObjectIds.FrozenGeode },
        { QualifiedObjectIds.Stone_Node_MagmaGeode, QualifiedObjectIds.MagmaGeode },
        { QualifiedObjectIds.Stone_Node_OmniGeode, QualifiedObjectIds.OmniGeode },
        { QualifiedObjectIds.Stone_Node_Amethyst, QualifiedObjectIds.Amethyst },
        { QualifiedObjectIds.Stone_Node_Topaz, QualifiedObjectIds.Topaz },
        { QualifiedObjectIds.Stone_Node_Emerald, QualifiedObjectIds.Emerald },
        { QualifiedObjectIds.Stone_Node_Aquamarine, QualifiedObjectIds.Aquamarine },
        { QualifiedObjectIds.Stone_Node_Jade, QualifiedObjectIds.Jade },
        { QualifiedObjectIds.Stone_Node_Ruby, QualifiedObjectIds.Ruby },
        { QualifiedObjectIds.Stone_Node_Diamond, QualifiedObjectIds.Diamond },
        { QualifiedObjectIds.Stone_Node_Mussel, QualifiedObjectIds.Mussel },
        { QualifiedObjectIds.Stone_Node_BoneFragment0, QualifiedObjectIds.BoneFragment },
        { QualifiedObjectIds.Stone_Node_BoneFragment1, QualifiedObjectIds.BoneFragment },
        { QualifiedObjectIds.Stone_Node_Clay, QualifiedObjectIds.Clay },
        { QualifiedObjectIds.Stone_Node_CinderShard0, QualifiedObjectIds.CinderShard },
        { QualifiedObjectIds.Stone_Node_CinderShard1, QualifiedObjectIds.CinderShard },
    };
}
