namespace DaLion.Professions.Framework;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
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

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static Dictionary<string, string> FamilyPairs { get; } = new()
    {
        { QIDs.Crimsonfish, QIDs.SonOfCrimsonfish },
        { QIDs.Angler, QIDs.MsAngler },
        { QIDs.Legend, QIDs.LegendII },
        { QIDs.MutantCarp, QIDs.RadioactiveCarp },
        { QIDs.Glacierfish, QIDs.GlacierfishJr },
        { QIDs.SonOfCrimsonfish, QIDs.Crimsonfish },
        { QIDs.MsAngler, QIDs.Angler },
        { QIDs.LegendII, QIDs.Legend },
        { QIDs.RadioactiveCarp, QIDs.MutantCarp },
        { QIDs.GlacierfishJr, QIDs.Glacierfish },
        { "MNF.MoreNewFish_tui", "MNF.MoreNewFish_la" },
        { "MNF.MoreNewFish_la", "MNF.MoreNewFish_tui" },
    };

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static HashSet<string> ResourceNodeIds { get; set; } =
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

    /// <summary>Gets or sets the ids of (valuable) resource clumps.</summary>
    internal static HashSet<int> ResourceClumpIds { get; set; } = [];
}
