namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Holds collections which may be referenced by different modules.</summary>
internal static class Collections
{
    /// <summary>Gets the recognized artisan machines.</summary>
    internal static IReadOnlySet<string> ArtisanMachines { get; } = new HashSet<string>
    {
        "Cheese Press",
        "Keg",
        "Loom",
        "Mayonnaise Machine",
        "Oil Maker",
        "Preserves Jar",
    }.Concat(ProfessionsModule.Config.CustomArtisanMachines).ToHashSet();

    /// <summary>Gets the names of the legendary fish.</summary>
    internal static IReadOnlySet<string> LegendaryFishNames { get; } = new HashSet<string>
    {
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
    };

    /// <summary>Gets the resource that should spawn from a given stone.</summary>
    internal static IReadOnlyDictionary<int, int> ResourceFromStoneId { get; } = new Dictionary<int, int>
    {
        // stone
        { 668, 390 },
        { 670, 390 },
        { 845, 390 },
        { 846, 390 },
        { 847, 390 },

        // ores
        { 751, 378 },
        { 849, 378 },
        { 290, 380 },
        { 850, 380 },
        { 764, 384 },
        { 765, 386 },
        { 95, 909 },

        // geodes
        { 75, 535 },
        { 76, 536 },
        { 77, 537 },
        { 819, 749 },

        // gems
        { 8, 66 },
        { 10, 68 },
        { 12, 60 },
        { 14, 62 },
        { 6, 70 },
        { 4, 64 },
        { 2, 72 },

        // other
        { 25, 719 },
        { 816, 881 },
        { 817, 881 },
        { 818, 330 },
        { 843, 848 },
        { 844, 848 },
    };

    /// <summary>Gets or sets the ids of resource nodes.</summary>
    internal static IReadOnlySet<int> ResourceNodeIds { get; set; } = new HashSet<int>
    {
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
        46, // mystic stone
    };

    /// <summary>Gets the corresponding extended family pair by legendary fish id.</summary>
    internal static IReadOnlyDictionary<int, int> ExtendedFamilyPairs { get; } = new Dictionary<int, int>
    {
        { Constants.CrimsonfishIndex, Constants.SonOfCrimsonfishIndex },
        { Constants.AnglerIndex, Constants.MsAnglerIndex },
        { Constants.LegendIndex, Constants.Legend2Index },
        { Constants.MutantCarpIndex, Constants.RadioactiveCarpIndex },
        { Constants.GlacierfishIndex, Constants.GlacierfishJrIndex },
        { Constants.SonOfCrimsonfishIndex, Constants.CrimsonfishIndex },
        { Constants.MsAnglerIndex, Constants.AnglerIndex },
        { Constants.Legend2Index, Constants.LegendIndex },
        { Constants.RadioactiveCarpIndex, Constants.MutantCarpIndex },
        { Constants.GlacierfishJrIndex, Constants.GlacierfishIndex },
    };

    /// <summary>Gets the swords that should be converted to Stabbing Swords.</summary>
    internal static ISet<int> StabbingSwords { get; } = new HashSet<int>
    {
        Constants.SteelSmallswordIndex,
        Constants.CutlassIndex,
        Constants.RapierIndex,
        Constants.SteelFalchionIndex,
        Constants.PiratesSwordIndex,
        Constants.ForestSwordIndex,
        Constants.LavaKatanaIndex,
        Constants.DragontoothCutlassIndex,
        Constants.DarkSwordIndex,
    };
}
