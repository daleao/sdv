namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Holds sets which may be referenced by different modules.</summary>
internal static class Sets
{
    /// <summary>Gets the recognized artisan machines.</summary>
    internal static ImmutableHashSet<string> ArtisanMachines { get; } = new HashSet<string>
    {
        "Cheese Press",
        "Keg",
        "Loom",
        "Mayonnaise Machine",
        "Oil Maker",
        "Preserves Jar",
    }.Concat(ProfessionsModule.Config.CustomArtisanMachines).ToImmutableHashSet();

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
        "Waterfall Snakehead"); // ridgeside

    /// <summary>Gets or sets the ids of animal products and derived artisan goods.</summary>
    internal static ImmutableHashSet<int> AnimalDerivedProductIds { get; set; } = ImmutableHashSet.Create(
        ObjectIds.DinosaurEgg,
        ObjectIds.Mayonnaise,
        ObjectIds.DuckMayonnaise,
        ObjectIds.VoidMayonnaise,
        ObjectIds.Cheese,
        ObjectIds.GoatCheese,
        ObjectIds.Cloth,
        ObjectIds.DinosaurMayonnaise);

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
}
