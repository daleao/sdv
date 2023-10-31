namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
internal static class Lookups
{
    /// <summary>Gets the names of the legendary fish.</summary>
    internal static ImmutableHashSet<string> LegendaryFishes { get; } = ImmutableHashSet.Create(
        "Crimsonfish", // vanilla
        "Angler", // vanilla
        "Legend", // vanilla
        "Glacierfish", // vanilla
        "Mutant Carp", // vanilla
        "Son of Crimsonfish", // qi extended
        "Ms. Angler", // qi extended
        "Legend II", // qi extended
        "Glacierfish Jr.", // qi extended
        "Radioactive Carp", // qi extended
        "Pufferchick", // stardew aquarium
        "Deep Ridge Angler", // ridgeside
        "Sockeye Salmon", // ridgeside
        "Waterfall Snakehead", // ridgeside
        "Tui", // more new fish
        "La"); // more new fish

    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    internal static BiMap<int, int> FamilyPairs { get; } = new(new Dictionary<int, int>
    {
        { ObjectIds.Crimsonfish, ObjectIds.SonOfCrimsonfish },
        { ObjectIds.Angler, ObjectIds.MsAngler },
        { ObjectIds.Legend, ObjectIds.LegendII },
        { ObjectIds.MutantCarp, ObjectIds.RadioactiveCarp },
        { ObjectIds.Glacierfish, ObjectIds.GlacierfishJr },
        { 1127, 1128 }, // Tui and La, More New Fish
    });

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static ImmutableHashSet<int> ResourceNodeIds { get; set; } = ImmutableHashSet.Create(
        ObjectIds.Stone_Node_Copper0,
        ObjectIds.Stone_Node_Copper1,
        ObjectIds.Stone_Node_Iron0,
        ObjectIds.Stone_Node_Iron1,
        ObjectIds.Stone_Node_Gold,
        ObjectIds.Stone_Node_Iridium,
        ObjectIds.Stone_Node_Radioactive,
        ObjectIds.Stone_Node_Geode,
        ObjectIds.Stone_Node_FrozenGeode,
        ObjectIds.Stone_Node_MagmaGeode,
        ObjectIds.Stone_Node_OmniGeode,
        ObjectIds.Stone_Node_Diamond,
        ObjectIds.Stone_Node_Ruby,
        ObjectIds.Stone_Node_Jade,
        ObjectIds.Stone_Node_Amethyst,
        ObjectIds.Stone_Node_Topaz,
        ObjectIds.Stone_Node_Emerald,
        ObjectIds.Stone_Node_Aquamarine,
        ObjectIds.Stone_Node_Gemstone,
        ObjectIds.Stone_Node_Mussel,
        ObjectIds.Stone_Node_BoneFragment0,
        ObjectIds.Stone_Node_BoneFragment1,
        ObjectIds.Stone_Node_Clay,
        ObjectIds.Stone_Node_CinderShard0,
        ObjectIds.Stone_Node_CinderShard1,
        ObjectIds.Stone_Node_MysticStone);

    /// <summary>Gets or sets the ids of (valuable) resource clumps.</summary>
    internal static ImmutableHashSet<int> ResourceClumpIds { get; set; } = ImmutableHashSet<int>.Empty;

    /// <summary>Gets a map from stone node ID to its corresponding resource ID.</summary>
    internal static ImmutableDictionary<int, int> ResourceFromNode { get; } = new Dictionary<int, int>
    {
        { ObjectIds.Stone_Node_Regular0, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular1, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular2, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular3, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular4, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Copper0, ObjectIds.CopperOre },
        { ObjectIds.Stone_Node_Copper1, ObjectIds.CopperOre },
        { ObjectIds.Stone_Node_Iron0, ObjectIds.IronOre },
        { ObjectIds.Stone_Node_Iron1, ObjectIds.IronOre },
        { ObjectIds.Stone_Node_Gold, ObjectIds.GoldOre },
        { ObjectIds.Stone_Node_Iridium, ObjectIds.IridiumOre },
        { ObjectIds.Stone_Node_Radioactive, ObjectIds.RadioactiveOre },
        { ObjectIds.Stone_Node_Geode, ObjectIds.Geode },
        { ObjectIds.Stone_Node_FrozenGeode, ObjectIds.FrozenGeode },
        { ObjectIds.Stone_Node_MagmaGeode, ObjectIds.MagmaGeode },
        { ObjectIds.Stone_Node_OmniGeode, ObjectIds.OmniGeode },
        { ObjectIds.Stone_Node_Amethyst, ObjectIds.Amethyst },
        { ObjectIds.Stone_Node_Topaz, ObjectIds.Topaz },
        { ObjectIds.Stone_Node_Emerald, ObjectIds.Emerald },
        { ObjectIds.Stone_Node_Aquamarine, ObjectIds.Aquamarine },
        { ObjectIds.Stone_Node_Jade, ObjectIds.Jade },
        { ObjectIds.Stone_Node_Ruby, ObjectIds.Ruby },
        { ObjectIds.Stone_Node_Diamond, ObjectIds.Diamond },
        { ObjectIds.Stone_Node_Mussel, ObjectIds.Mussel },
        { ObjectIds.Stone_Node_BoneFragment0, ObjectIds.BoneFragment },
        { ObjectIds.Stone_Node_BoneFragment1, ObjectIds.BoneFragment },
        { ObjectIds.Stone_Node_Clay, ObjectIds.Clay },
        { ObjectIds.Stone_Node_CinderShard0, ObjectIds.CinderShard },
        { ObjectIds.Stone_Node_CinderShard1, ObjectIds.CinderShard },
    }.ToImmutableDictionary();
}
