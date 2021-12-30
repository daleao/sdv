using System.Collections.Generic;

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds common methods and properties related to objects.</summary>
public static class Objects
{
    #region look-up tables

    /// <summary>Look-up table for different types of bait by id.</summary>
    public static Dictionary<int, string> BaitById { get; } = new()
    {
        {685, "Bait"},
        {703, "Magnet"},
        {774, "Wild Bait"},
        {908, "Magic Bait"}
    };

    /// <summary>Look-up table for what resource should spawn from a given stone.</summary>
    public static Dictionary<int, int> ResourceFromStoneId { get; } = new()
    {
        // stone
        {668, 390},
        {670, 390},
        {845, 390},
        {846, 390},
        {847, 390},

        // ores
        {751, 378},
        {849, 378},
        {290, 380},
        {850, 380},
        {764, 384},
        {765, 386},
        {95, 909},

        // geodes
        {75, 535},
        {76, 536},
        {77, 537},
        {819, 749},

        // gems
        {8, 66},
        {10, 68},
        {12, 60},
        {14, 62},
        {6, 70},
        {4, 64},
        {2, 72},

        // other
        {25, 719},
        {816, 881},
        {817, 881},
        {818, 330},
        {843, 848},
        {844, 848}
    };

    /// <summary>Look-up table for trappable treasure items using magnet.</summary>
    public static Dictionary<int, string[]> TrapperPirateTreasureTable { get; } = new()
    {
        {14, new[] {"0.003", "1", "1"}}, // neptune's glaive
        {51, new[] {"0.003", "1", "1"}}, // broken trident
        {166, new[] {"0.03", "1", "1"}}, // treasure chest
        {109, new[] {"0.009", "1", "1"}}, // ancient sword
        {110, new[] {"0.009", "1", "1"}}, // rusty spoon
        {111, new[] {"0.009", "1", "1"}}, // rusty spur
        {112, new[] {"0.009", "1", "1"}}, // rusty cog
        {117, new[] {"0.009", "1", "1"}}, // anchor
        {378, new[] {"0.39", "1", "24"}}, // copper ore
        {380, new[] {"0.24", "1", "24"}}, // iron ore
        {384, new[] {"0.12", "1", "24"}}, // gold ore
        {386, new[] {"0.065", "1", "2"}}, // iridium ore
        {516, new[] {"0.024", "1", "1"}}, // small glow ring
        {517, new[] {"1.009", "1", "1"}}, // glow ring
        {518, new[] {"0.024", "1", "1"}}, // small magnet ring
        {519, new[] {"1.009", "1", "1"}}, // magnet ring
        {527, new[] {"0.005", "1", "1"}}, // iridium band
        {529, new[] {"0.005", "1", "1"}}, // amethyst ring
        {530, new[] {"0.005", "1", "1"}}, // topaz ring
        {531, new[] {"0.005", "1", "1"}}, // aquamarine ring
        {532, new[] {"0.005", "1", "1"}}, // jade ring
        {533, new[] {"0.005", "1", "1"}}, // emerald ring
        {534, new[] {"0.005", "1", "1"}}, // ruby ring
        {890, new[] {"0.03", "1", "3"}} // qi bean
    };

    #endregion look-up tables

    #region hash sets

    /// <summary>Hash list of artisan machines.</summary>
    public static readonly IEnumerable<string> ArtisanMachines = new HashSet<string>
    {
        "Alembic", // artisan valley
        "Artisanal Soda Maker", // artisanal soda makers
        "Butter Churn", // artisan valley
        "Canning Machine", // fresh meat
        "Carbonator", // artisanal soda makers
        "Cheese Press", // vanilla
        "Cola Maker", // artisanal soda makers
        "Cream Soda Maker", // artisanal soda makers
        "DNA Synthesizer", // fresh meat
        "Dehydrator", // artisan valley
        "Drying Rack", // artisan valley
        "Espresso Machine", // artisan valley
        "Extruder", // artisan valley
        "Foreign Cask", // artisan valley
        "Glass Jar", // artisan valley
        "Grinder", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Infuser", // artisan valley
        "Juicer", // artisan valley
        "Keg", // vanilla
        "Loom", // vanilla
        "Marble Soda Machine", // fizzy drinks
        "Mayonnaise Machine", // vanilla
        "Meat Press", // fresh meat
        "Oil Maker", // vanilla
        "Pepper Blender", // artisan valley
        "Preserves Jar", // vanilla
        "Shaved Ice Machine", // shaved ice & frozen treats
        "Smoker", // artisan valley
        "Soap Press", // artisan valley
        "Sorbet Machine", // artisan valley
        "Still", // artisan valley
        "Syrup Maker", // artisanal soda makers
        "Vinegar Cask", // artisan valley
        "Wax Barrel", // artisan valley
        "Yogurt Jar" // artisan valley
    };

    /// <summary>Hash list of ids corresponding to animal produce or derived artisan goods.</summary>
    public static readonly IEnumerable<int> AnimalDerivedProductIDs = new HashSet<int>
    {
        107, // dinosaur egg
        306, // mayonnaise
        307, // duck mayonnaise
        308, // void mayonnaise
        340, // honey
        424, // cheese
        426, // goat cheese
        428, // cloth
        807 // dinosaur mayonnaise
    };

    /// <summary>Hash list of stone ids corresponding to resource nodes.</summary>
    public static readonly IEnumerable<int> ResourceNodeIDs = new HashSet<int>
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
        46 // mystic stone
    };

    /// <summary>Hash list of fish names corresponding to legendary fish.</summary>
    public static readonly IEnumerable<string> LegendaryFishNames = new HashSet<string>
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
        "Pufferchick" // stardew aquarium
    };

    #endregion hash sets
}