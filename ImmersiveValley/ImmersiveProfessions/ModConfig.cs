namespace DaLion.Stardew.Professions;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Mod key used by Prospector and Scavenger professions.</summary>
    public KeybindList ModKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Add custom mod Artisan machines to this list to make them compatible with the profession.</summary>
    public string[] CustomArtisanMachines { get; } = {
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
        "Yogurt Jar" // artisan valley
    };

    /// <summary>You must forage this many items before your forage becomes iridium-quality.</summary>
    public uint ForagesNeededForBestQuality { get; set; } = 500;

    /// <summary>You must mine this many minerals before your mined minerals become iridium-quality.</summary>
    public uint MineralsNeededForBestQuality { get; set; } = 500;

    /// <summary>If enabled, Automated machines will contribute toward EcologistItemsForaged and GemologistMineralsCollected.</summary>
    public bool ShouldCountAutomatedHarvests { get; set; } = false;

    /// <summary>The chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
    public double ChanceToStartTreasureHunt { get; set; } = 0.1;

    /// <summary>Whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    public bool AllowScavengerHuntsOnFarm { get; set; } = false;

    /// <summary>Increase this multiplier if you find that Scavenger hunts end too quickly.</summary>
    public float ScavengerHuntHandicap { get; set; } = 1f;

    /// <summary>Increase this multiplier if you find that Prospector hunts end too quickly.</summary>
    public float ProspectorHuntHandicap { get; set; } = 1f;

    /// <summary>You must be this close to the treasure hunt target before the indicator appears.</summary>
    public float TreasureDetectionDistance { get; set; } = 3f;

    /// <summary>The maximum speed bonus a Spelunker can reach.</summary>
    public uint SpelunkerSpeedCap { get; set; } = 10;

    /// <summary>Toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
    public bool EnableGetExcited { get; set; } = true;

    /// <summary>Whether Seaweed and Algae are considered junk for fishing purposes.</summary>
    public bool SeaweedIsJunk { get; set; } = true;

    /// <summary>You must catch this many fish of a given species to achieve instant catch.</summary>
    /// <remarks>Unused.</remarks>
    public uint FishNeededForInstantCatch { get; set; } = 500;

    /// <summary>If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.</summary>
    public float AnglerMultiplierCap { get; set; } = 1f;

    /// <summary>The maximum population of Aquarist Fish Ponds with legendary fish.</summary>
    public uint LegendaryPondPopulationCap { get; set; } = 6;

    /// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction the following season.</summary>
    public uint TrashNeededPerTaxBonusPct { get; set; } = 100;

    /// <summary>You must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
    public uint TrashNeededPerFriendshipPoint { get; set; } = 100;

    /// <summary>The maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    public float ConservationistTaxBonusCeiling { get; set; } = 0.37f;

    /// <summary>The maximum stacks that can be gained for each buff stat.</summary>
    public uint PiperBuffCap { get; set; } = 10;

    /// <summary>Required to allow Ultimate activation. Super Stat continues to apply.</summary>
    public bool EnableSpecials { get; set; } = true;

    /// <summary>Mod key used to activate Ultimate. Can be the same as <see cref="ModKey" />.</summary>
    public KeybindList SpecialActivationKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Whether Ultimate is activated on <see cref="SpecialActivationKey" /> hold (as opposed to press).</summary>
    public bool HoldKeyToActivateSpecial { get; set; } = true;

    /// <summary>How long <see cref="SpecialActivationKey" /> should be held to activate Ultimate, in seconds.</summary>
    public float SpecialActivationDelay { get; set; } = 1f;

    /// <summary>Affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too slowly.</summary>
    public double SpecialGainFactor { get; set; } = 1.0;

    /// <summary>Affects the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last longer.</summary>
    public double SpecialDrainFactor { get; set; } = 1.0;

    /// <summary>Required to apply prestige changes.</summary>
    public bool EnablePrestige { get; set; } = true;

    /// <summary>Multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
    public float SkillResetCostMultiplier { get; set; } = 1f;

    /// <summary>Whether resetting a skill also clears all corresponding recipes.</summary>
    public bool ForgetRecipesOnSkillReset { get; set; } = true;

    /// <summary>Whether the player can use the Statue of Prestige more than once per day.</summary>
    public bool AllowPrestigeMultiplePerDay { get; set; } = false;

    /// <summary>Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.</summary>
    public float BonusSkillExpPerReset { get; set; } = 0.1f;

    /// <summary>How much skill experience is required for each level up beyond 10.</summary>
    public uint RequiredExpPerExtendedLevel { get; set; } = 5000;

    /// <summary>Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    public uint PrestigeRespecCost { get; set; } = 20000;

    /// <summary>Monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
    public uint ChangeUltCost { get; set; } = 0;

    /// <summary>Multiplies all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
    public float[] BaseSkillExpMultiplierPerSkill { get; set; } = { 1f, 1f, 1f, 1f, 1f, 1f };

    /// <summary>Increases the health of all monsters.</summary>
    public float MonsterHealthMultiplier { get; set; } = 1f;

    /// <summary>Increases the damage dealt by all monsters.</summary>
    public float MonsterDamageMultiplier { get; set; } = 1f;

    /// <summary>Increases the resistance of all monsters.</summary>
    public float MonsterDefenseMultiplier { get; set; } = 1f;

    /// <summary>Enable if using the Vintage Interface v2 mod. Accepted values: "Brown", "Pink", "Off", "Automatic".</summary>
    public VintageInterfaceStyle VintageInterfaceSupport { get; set; } = VintageInterfaceStyle.Automatic;

    /// <summary>Determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons", "Gen4Ribbons".</summary>
    public ProgressionStyle PrestigeProgressionStyle { get; set; } = ProgressionStyle.StackedStars;

    /// <summary>Key used by trigger UI debugging events.</summary>
    public KeybindList DebugKey { get; set; } = KeybindList.Parse("LeftControl");

    #region dropdown enums

    public enum VintageInterfaceStyle
    {
        Off,
        Pink,
        Brown,
        Automatic
    }

    public enum ProgressionStyle
    {
        StackedStars,
        Gen3Ribbons,
        Gen4Ribbons
    }

    #endregion dropdown enums
}