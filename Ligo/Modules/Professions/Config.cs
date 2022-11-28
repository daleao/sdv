namespace DaLion.Ligo.Modules.Professions;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for Professions.</summary>
public sealed class Config
{
    #region dropdown enums

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

    /// <summary>Gets mod key used by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether determines whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
    [JsonProperty]
    public bool ShouldJunimosInheritProfessions { get; internal set; } = false;

    /// <summary>Gets custom mod Artisan machines. Add to this list to make them compatible with the profession.</summary>
    [JsonProperty]
    public string[] CustomArtisanMachines { get; internal set; } =
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

    /// <summary>Gets a value indicating whether Bee House will be affected by producer bonuses.</summary>
    [JsonProperty]
    public bool BeesAreAnimals { get; internal set; } = true;

    /// <summary>Gets the number of items that must be foraged before foraged items become iridium-quality.</summary>
    [JsonProperty]
    public uint ForagesNeededForBestQuality { get; internal set; } = 100;

    /// <summary>Gets the number of minerals that must be mined before mined minerals become iridium-quality.</summary>
    [JsonProperty]
    public uint MineralsNeededForBestQuality { get; internal set; } = 100;

    /// <summary>
    ///     Gets a value indicating whether if enabled, machine and building ownership will be ignored when determining whether to apply profession
    ///     bonuses.
    /// </summary>
    [JsonProperty]
    public bool LaxOwnershipRequirements { get; internal set; } = false;

    /// <summary>Gets the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    public float TrackPointerScale { get; internal set; } = 1f;

    /// <summary>Gets the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
    [JsonProperty]
    public float TrackPointerBobbingRate { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether if enabled, Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
    [JsonProperty]
    public bool DisableAlwaysTrack { get; internal set; } = false;

    /// <summary>Gets the chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
    [JsonProperty]
    public double ChanceToStartTreasureHunt { get; internal set; } = 0.1;

    /// <summary>Gets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    [JsonProperty]
    public bool AllowScavengerHuntsOnFarm { get; internal set; } = false;

    /// <summary>Gets a multiplier which is used to extend the duration of Scavenger hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    public float ScavengerHuntHandicap { get; internal set; } = 1f;

    /// <summary>Gets a multiplier which is used to extend the duration of Prospector hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    public float ProspectorHuntHandicap { get; internal set; } = 1f;

    /// <summary>Gets the minimum distance to the treasure hunt target before the indicator appears.</summary>
    [JsonProperty]
    public float TreasureDetectionDistance { get; internal set; } = 3f;

    /// <summary>Gets the maximum speed bonus a Spelunker can reach.</summary>
    [JsonProperty]
    public uint SpelunkerSpeedCap { get; internal set; } = 10;

    /// <summary>Gets a value indicating whether toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
    [JsonProperty]
    public bool EnableGetExcited { get; internal set; } = true;

    /// <summary>Gets the number of fish species that must be caught to achieve instant catch.</summary>
    /// <remarks>Unused.</remarks>
    [JsonProperty]
    public uint FishNeededForInstantCatch { get; internal set; } = 500;

    /// <summary>
    ///     Gets the maximum multiplier that will be added to fish sold by Angler. if multiple new fish mods are installed,
    ///     you may want to adjust this to a sensible value.
    /// </summary>
    [JsonProperty]
    public float AnglerMultiplierCap { get; internal set; } = 1f;

    /// <summary>
    ///     Gets a value indicating whether to display the MAX icon below fish in the Collections Menu which have been caught at the
    ///     maximum size.
    /// </summary>
    [JsonProperty]
    public bool ShowFishCollectionMaxIcon { get; internal set; } = true;

    /// <summary>Gets the maximum population of Aquarist Fish Ponds with legendary fish.</summary>
    [JsonProperty]
    public uint LegendaryPondPopulationCap { get; internal set; } = 6;

    /// <summary>Gets you must collect this many junk items from crab pots for every 1% of tax deduction the following season.</summary>
    [JsonProperty]
    public uint TrashNeededPerTaxBonusPct { get; internal set; } = 100;

    /// <summary>Gets you must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
    [JsonProperty]
    public uint TrashNeededPerFriendshipPoint { get; internal set; } = 100;

    /// <summary>Gets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    [JsonProperty]
    public float ConservationistTaxBonusCeiling { get; internal set; } = 0.37f;

    /// <summary>Gets the maximum stacks that can be gained for each buff stat.</summary>
    [JsonProperty]
    public uint PiperBuffCap { get; internal set; } = 10;

    /// <summary>Gets a value indicating whether to allow Ultimate activation. Super Stat continues to apply.</summary>
    [JsonProperty]
    public bool EnableSpecials { get; internal set; } = true;

    /// <summary>Gets mod key used to activate Ultimate. Can be the same as <see cref="ModKey"/>.</summary>
    [JsonProperty]
    public KeybindList SpecialActivationKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether determines whether Ultimate is activated on <see cref="SpecialActivationKey"/> hold (as opposed to press).</summary>
    [JsonProperty]
    public bool HoldKeyToActivateSpecial { get; internal set; } = true;

    /// <summary>Gets how long <see cref="SpecialActivationKey"/> should be held to activate Ultimate, in seconds.</summary>
    [JsonProperty]
    public float SpecialActivationDelay { get; internal set; } = 1f;

    /// <summary>
    ///     Gets the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too
    ///     slowly.
    /// </summary>
    [JsonProperty]
    public double SpecialGainFactor { get; internal set; } = 1d;

    /// <summary>
    ///     Gets the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last
    ///     longer.
    /// </summary>
    [JsonProperty]
    public double SpecialDrainFactor { get; internal set; } = 1d;

    /// <summary>Gets a value indicating whether to apply Prestige changes.</summary>
    [JsonProperty]
    public bool EnablePrestige { get; internal set; } = true;

    /// <summary>Gets the base skill reset cost multiplier. Set to 0 to reset for free.</summary>
    [JsonProperty]
    public float SkillResetCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether resetting a skill also clears all corresponding recipes.</summary>
    [JsonProperty]
    public bool ForgetRecipes { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the player can use the Statue of Prestige more than once per day.</summary>
    [JsonProperty]
    public bool AllowMultiplePrestige { get; internal set; } = false;

    /// <summary>Gets cumulative multiplier to each skill's experience gain after a respective skill reset.</summary>
    [JsonProperty]
    public float PrestigeExpMultiplier { get; internal set; } = 0.1f;

    /// <summary>Gets how much skill experience is required for each level up beyond 10.</summary>
    [JsonProperty]
    public uint RequiredExpPerExtendedLevel { get; internal set; } = 5000;

    /// <summary>Gets monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    [JsonProperty]
    public uint PrestigeRespecCost { get; internal set; } = 20000;

    /// <summary>Gets monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
    [JsonProperty]
    public uint ChangeUltCost { get; internal set; } = 0;

    /// <summary>Gets a multiplier that will be applied to all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
    [JsonProperty]
    public float[] BaseSkillExpMultipliers { get; internal set; } = { 1f, 1f, 1f, 1f, 1f, 1f };

    /// <summary>Gets multiplies all skill experience gained from the start of the game, for custom skills.</summary>
    [JsonProperty]
    public Dictionary<string, float> CustomSkillExpMultipliers { get; internal set; } =
        new()
        {
            { "DaLion.Alchemy", 1 },
            { "blueberry.Cooking", 1 },
            { "spacechase0.Cooking", 1 },
            { "spacechase0.Luck", 1 },
            { "spacechase0.Magic", 1 },
            { "drbirbdev.BinningSkill", 1 },
            { "drbirbdev.SocializingSkill", 1 },
        };

    /// <summary>
    ///     Gets determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons",
    ///     "Gen4Ribbons".
    /// </summary>
    [JsonProperty]
    public ProgressionStyle PrestigeProgressionStyle { get; internal set; } = ProgressionStyle.StackedStars;
}
