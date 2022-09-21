namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod user-defined settings.</summary>
public sealed class ModConfig
{
    #region dropdown enums

    /// <summary>The version of <c>Vintage Interface</c> to use for compatibility.</summary>
    public enum VintageInterfaceStyle
    {
        /// <summary>None. Use vanilla interface.</summary>
        Off,

        /// <summary>Vintage Interface v1 (pink version).</summary>
        Pink,

        /// <summary>Vintage Interface v1 (brown version).</summary>
        Brown,

        /// <summary>Detect automatically based on the <see cref="IModRegistry"/>.</summary>
        Automatic,
    }

    /// <summary>The style used to indicate Skill Reset progression.</summary>
    public enum ProgressionStyle
    {
        /// <summary>Use stacked quality star icons, one per reset level.</summary>
        StackedStars,

        /// <summary>Use Generation 3 Pokemon contest ribbons.</summary>
        Gen3Ribbons,

        /// <summary>Use Generation 4 Pokemon contest ribbons.</summary>
        Gen4Ribbons,
    }

    #endregion dropdown enums

    /// <summary>Gets or sets mod key used by Prospector and Scavenger professions.</summary>
    public KeybindList ModKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets or sets a value indicating whether determines whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
    public bool ShouldJunimosInheritProfessions { get; set; } = false;

    /// <summary>Gets or sets custom mod Artisan machines. Add to this list to make them compatible with the profession.</summary>
    public string[] CustomArtisanMachines { get; set; } =
    {
        "Alembic", // artisan valley
        "Artisanal Soda Maker", // artisanal soda makers
        "Butter Churn", // artisan valley
        "Canning Machine", // fresh meat
        "Carbonator", // artisanal soda makers
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
        "Marble Soda Machine", // fizzy drinks
        "Meat Press", // fresh meat
        "Pepper Blender", // artisan valley
        "Shaved Ice Machine", // shaved ice & frozen treats
        "Smoker", // artisan valley
        "Soap Press", // artisan valley
        "Sorbet Machine", // artisan valley
        "Still", // artisan valley
        "Syrup Maker", // artisanal soda makers
        "Vinegar Cask", // artisan valley
        "Wax Barrel", // artisan valley
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets or sets you must forage this many items before your forage becomes iridium-quality.</summary>
    public uint ForagesNeededForBestQuality { get; set; } = 100;

    /// <summary>Gets or sets you must mine this many minerals before your mined minerals become iridium-quality.</summary>
    public uint MineralsNeededForBestQuality { get; set; } = 100;

    /// <summary>
    ///     Gets or sets a value indicating whether if enabled, machine and building ownership will be ignored when determining whether to apply profession
    ///     bonuses.
    /// </summary>
    public bool LaxOwnershipRequirements { get; set; } = false;

    /// <summary>Gets or sets changes the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
    public float TrackPointerScale { get; set; } = 1f;

    /// <summary>Gets or sets changes the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
    public float TrackPointerBobbingRate { get; set; } = 1f;

    /// <summary>Gets or sets a value indicating whether if enabled, Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
    public bool DisableAlwaysTrack { get; set; } = false;

    /// <summary>Gets or sets the chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
    public double ChanceToStartTreasureHunt { get; set; } = 0.1;

    /// <summary>Gets or sets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    public bool AllowScavengerHuntsOnFarm { get; set; } = false;

    /// <summary>Gets or sets increase this multiplier if you find that Scavenger hunts end too quickly.</summary>
    public float ScavengerHuntHandicap { get; set; } = 1f;

    /// <summary>Gets or sets increase this multiplier if you find that Prospector hunts end too quickly.</summary>
    public float ProspectorHuntHandicap { get; set; } = 1f;

    /// <summary>Gets or sets you must be this close to the treasure hunt target before the indicator appears.</summary>
    public float TreasureDetectionDistance { get; set; } = 3f;

    /// <summary>Gets or sets the maximum speed bonus a Spelunker can reach.</summary>
    public uint SpelunkerSpeedCap { get; set; } = 10;

    /// <summary>Gets or sets a value indicating whether toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
    public bool EnableGetExcited { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether Seaweed and Algae are considered junk for fishing purposes.</summary>
    public bool SeaweedIsTrash { get; set; } = true;

    /// <summary>Gets or sets you must catch this many fish of a given species to achieve instant catch.</summary>
    /// <remarks>Unused.</remarks>
    public uint FishNeededForInstantCatch { get; set; } = 500;

    /// <summary>
    ///     Gets or sets if multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price
    ///     multiplier for fish sold by Angler.
    /// </summary>
    public float AnglerMultiplierCap { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets a value indicating whether determines whether to display the MAX icon below fish in the Collections Menu which have been caught at the
    ///     maximum size.
    /// </summary>
    public bool ShowFishCollectionMaxIcon { get; set; } = true;

    /// <summary>Gets or sets the maximum population of Aquarist Fish Ponds with legendary fish.</summary>
    public uint LegendaryPondPopulationCap { get; set; } = 6;

    /// <summary>Gets or sets you must collect this many junk items from crab pots for every 1% of tax deduction the following season.</summary>
    public uint TrashNeededPerTaxBonusPct { get; set; } = 100;

    /// <summary>Gets or sets you must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
    public uint TrashNeededPerFriendshipPoint { get; set; } = 100;

    /// <summary>Gets or sets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    public float ConservationistTaxBonusCeiling { get; set; } = 0.37f;

    /// <summary>Gets or sets the maximum stacks that can be gained for each buff stat.</summary>
    public uint PiperBuffCap { get; set; } = 10;

    /// <summary>Gets or sets a value indicating whether required to allow Ultimate activation. Super Stat continues to apply.</summary>
    public bool EnableSpecials { get; set; } = true;

    /// <summary>Gets or sets mod key used to activate Ultimate. Can be the same as <see cref="ModKey"/>.</summary>
    public KeybindList SpecialActivationKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets or sets a value indicating whether determines whether Ultimate is activated on <see cref="SpecialActivationKey"/> hold (as opposed to press).</summary>
    public bool HoldKeyToActivateSpecial { get; set; } = true;

    /// <summary>Gets or sets how long <see cref="SpecialActivationKey"/> should be held to activate Ultimate, in seconds.</summary>
    public float SpecialActivationDelay { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too
    ///     slowly.
    /// </summary>
    public double SpecialGainFactor { get; set; } = 1d;

    /// <summary>
    ///     Gets or sets affects the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last
    ///     longer.
    /// </summary>
    public double SpecialDrainFactor { get; set; } = 1d;

    /// <summary>Gets or sets a value indicating whether required to apply prestige changes.</summary>
    public bool EnablePrestige { get; set; } = true;

    /// <summary>Gets or sets multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
    public float SkillResetCostMultiplier { get; set; } = 1f;

    /// <summary>Gets or sets a value indicating whether determines whether resetting a skill also clears all corresponding recipes.</summary>
    public bool ForgetRecipes { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether determines whether the player can use the Statue of Prestige more than once per day.</summary>
    public bool AllowMultiplePrestige { get; set; } = false;

    /// <summary>Gets or sets cumulative multiplier to each skill's experience gain after a respective skill reset.</summary>
    public float PrestigeExpMultiplier { get; set; } = 0.1f;

    /// <summary>Gets or sets how much skill experience is required for each level up beyond 10.</summary>
    public uint RequiredExpPerExtendedLevel { get; set; } = 5000;

    /// <summary>Gets or sets monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    public uint PrestigeRespecCost { get; set; } = 20000;

    /// <summary>Gets or sets monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
    public uint ChangeUltCost { get; set; } = 0;

    /// <summary>Gets or sets multiplies all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
    public float[] BaseSkillExpMultipliers { get; set; } = { 1f, 1f, 1f, 1f, 1f, 1f };

    /// <summary>Gets or sets multiplies all skill experience gained from the start of the game, for custom skills.</summary>
    public Dictionary<string, float> CustomSkillExpMultipliers { get; set; } =
        new()
        {
            { "DaLion.Alchemy", 1 },
            { "blueberry.Cooking", 1 },
            { "spacechase0.Cooking", 1 },
            { "spacechase0.Luck", 1 },
            { "spacechase0.Magic", 1 },
        };

    /// <summary>Gets or sets enable if using the Vintage Interface v2 mod. Accepted values: "Brown", "Pink", "Off", "Automatic".</summary>
    public VintageInterfaceStyle VintageInterfaceSupport { get; set; } = VintageInterfaceStyle.Automatic;

    /// <summary>
    ///     Gets or sets determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons",
    ///     "Gen4Ribbons".
    /// </summary>
    public ProgressionStyle PrestigeProgressionStyle { get; set; } = ProgressionStyle.StackedStars;

    /// <summary>Gets or sets key used to trigger debug events.</summary>
    public KeybindList DebugKey { get; set; } = KeybindList.Parse("LeftControl");
}
