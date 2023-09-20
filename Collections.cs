namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Holds collections which may be referenced by different modules.</summary>
internal static class Collections
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
        "Waterfall Snakehead" // ridgeside
    );

    /// <summary>Gets the ids of animal products and derived artisan goods.</summary>
    internal static ImmutableHashSet<int> AnimalDerivedProductIds { get; set; } = ImmutableHashSet.Create(
        ObjectIds.DinosaurEgg,
        ObjectIds.Mayonnaise,
        ObjectIds.DuckMayonnaise,
        ObjectIds.VoidMayonnaise,
        ObjectIds.Cheese,
        ObjectIds.GoatCheese,
        ObjectIds.Cloth,
        ObjectIds.DinosaurMayonnaise
    );

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static ImmutableHashSet<int> ResourceNodeIds { get; set; } = ImmutableHashSet.Create(
        // ores
        751, // copper node
        849, // copper ?
        290, // silver node
        850, // silver ?
        764, // gold node
        765, // iridium node
        95, // radioactive node

        // geodes
        75, // geode node
        76, // frozen geode node
        77, // magma geode node
        819, // omni geode node

        // gems
        8, // amethyst node
        10, // topaz node
        12, // emerald node
        14, // aquamarine node
        6, // jade node
        4, // ruby node
        2, // diamond node
        44, // gem node

        // other
        25, // mussel node
        816, // bone node
        817, // bone node
        818, // clay node
        843, // cinder shard node
        844, // cinder shard node
        46 // mystic stone
    );

    /// <summary>Gets or sets the ids of (valuable) resource clumps.</summary>
    internal static ImmutableHashSet<int> ResourceClumpIds { get; set; } = ImmutableHashSet<int>.Empty;
}
