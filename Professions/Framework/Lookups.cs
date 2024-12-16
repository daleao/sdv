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
        QualifiedBigCraftableIds.Cask,
        QualifiedBigCraftableIds.CheesePress,
        QualifiedBigCraftableIds.Loom,
        QualifiedBigCraftableIds.MayonnaiseMachine,
        QualifiedBigCraftableIds.OilMaker,
        QualifiedBigCraftableIds.PreservesJar,
        QualifiedBigCraftableIds.Keg,
        QualifiedBigCraftableIds.Dehydrator,
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
        $"(O){UniqueId}_GoldenMayo",
        $"(O){UniqueId}_OstrichMayo",
    ];

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static Dictionary<string, string> FamilyPairs { get; } = new()
    {
        { QualifiedObjectIds.Crimsonfish, QualifiedObjectIds.SonOfCrimsonfish },
        { QualifiedObjectIds.Angler, QualifiedObjectIds.MsAngler },
        { QualifiedObjectIds.Legend, QualifiedObjectIds.LegendII },
        { QualifiedObjectIds.MutantCarp, QualifiedObjectIds.RadioactiveCarp },
        { QualifiedObjectIds.Glacierfish, QualifiedObjectIds.GlacierfishJr },
        { QualifiedObjectIds.SonOfCrimsonfish, QualifiedObjectIds.Crimsonfish },
        { QualifiedObjectIds.MsAngler, QualifiedObjectIds.Angler },
        { QualifiedObjectIds.LegendII, QualifiedObjectIds.Legend },
        { QualifiedObjectIds.RadioactiveCarp, QualifiedObjectIds.MutantCarp },
        { QualifiedObjectIds.GlacierfishJr, QualifiedObjectIds.Glacierfish },
        { "MNF.MoreNewFish_tui", "MNF.MoreNewFish_la" },
        { "MNF.MoreNewFish_la", "MNF.MoreNewFish_tui" },
    };

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
}
