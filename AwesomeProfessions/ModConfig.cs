namespace DaLion.Stardew.Professions;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Mod key used by Prospector and Scavenger professions.</summary>
    public KeybindList ModKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

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
    public uint FishNeededForInstantCatch { get; set; } = 500;

    /// <summary>If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.</summary>
    public float AnglerMultiplierCeiling { get; set; } = 1f;

    /// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction next season.</summary>
    public uint TrashNeededPerTaxLevel { get; set; } = 100;

    /// <summary>You must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
    public uint TrashNeededPerFriendshipPoint { get; set; } = 100;

    /// <summary>The maximum tax deduction percentage allowed by the Ferngill Revenue Service.</summary>
    public float TaxDeductionCeiling { get; set; } = 0.25f;

    /// <summary>Required to allow Ultimate activation. Super Stat continues to apply.</summary>
    public bool EnableUltimates { get; set; } = true;

    /// <summary>Mod key used to activate Ultimate. Can be the same as <see cref="ModKey" />.</summary>
    public KeybindList UltimateKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Whether Ultimate is activated on <see cref="UltimateKey" /> hold (as opposed to press).</summary>
    public bool HoldKeyToActivateUltimate { get; set; } = true;

    /// <summary>How long <see cref="UltimateKey" /> should be held to activate Ultimate, in seconds.</summary>
    public float UltimateActivationDelay { get; set; } = 1f;

    /// <summary>Affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too slowly.</summary>
    public double UltimateGainFactor { get; set; } = 1.0;

    /// <summary>Affects the rate at which the Ultimate meter depletes during Ultimate. Increase this to make Ultimate last longer..</summary>
    public double UltimateDrainFactor { get; set; } = 1.0;

    /// <summary>Required to apply prestige changes.</summary>
    public bool EnablePrestige { get; set; } = true;

    /// <summary>Multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
    public float SkillResetCostMultiplier { get; set; } = 1f;

    /// <summary>Whether resetting a skill also clears all associated recipes.</summary>
    public bool ForgetRecipesOnSkillReset { get; set; } = true;

    /// <summary>Whether the player can use the Statue of Prestige more than once per day.</summary>
    public bool AllowPrestigeMultiplePerDay { get; set; } = false;

    /// <summary>Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.</summary>
    public float BonusSkillExpPerReset { get; set; } = 0.1f;

    /// <summary>How much skill experience is required for each level up beyond 10.</summary>
    public uint RequiredExpPerExtendedLevel { get; set; } = 20000;

    /// <summary>Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    public uint PrestigeRespecCost { get; set; } = 20000;

    /// <summary>Monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
    public uint ChangeUltCost { get; set; } = 0;

    /// <summary>Multiplies all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
    public float[] BaseSkillExpMultiplierPerSkill { get; set; } = { 1f, 1f, 1f, 1f, 1f };

    /// <summary>Increases the health of all monsters.</summary>
    public float MonsterHealthMultiplier { get; set; } = 1f;

    /// <summary>Increases the damage dealt by all monsters.</summary>
    public float MonsterDamageMultiplier { get; set; } = 1f;

    /// <summary>Increases the resistance of all monsters.</summary>
    public float MonsterDefenseMultiplier { get; set; } = 1f;

    /// <summary>Causes Fish Ponds to produce Roe, Ink or Algae in proportion to fish population.</summary>
    public bool RebalanceFishPonds { get; set; } = true;

    /// <summary>Improves certain underwhelming forges.</summary>
    public bool RebalanceForges { get; set; } = true;

    /// <summary>The visual style for different honey mead icons, if using BetterArtisanGoodIcons. Allowed values: 'ColoredBottles', 'ColoredCaps'.</summary>
    public string HoneyMeadStyle { get; set; } = "ColoredBottles";

    /// <summary>Enable if using the Vintage Interface v2 mod.</summary>
    public bool UseVintageInterface { get; set; } = false;

    /// <summary>Replicates SVE's config setting of the same name.</summary>
    public bool UseGaldoranThemeAllTimes { get; set; } = false;
    
    /// <summary>Replicates SVE's config setting of the same name.</summary>
    public bool DisableGaldoranTheme { get; set; } = false;

    /// <summary>Key used by trigger UI debugging events.</summary>
    public KeybindList DebugKey { get; set; } = KeybindList.Parse("LeftControl");
}