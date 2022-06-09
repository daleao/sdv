namespace DaLion.Common.Integrations;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;

#endregion using directives

/// <summary>Interface for proxying.</summary>
public interface IImmersiveProfessions
{
    /// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
    public interface IEvent
    {
        /// <summary>Whether this event is enabled.</summary>
        bool IsEnabled { get; }

        /// <summary>Whether this event is enabled for a specific splitscreen player.</summary>
        /// <param name="screenId">The player's screen id.</param>
        bool IsEnabledForScreen(int screenId);

        /// <summary>Enable this event on the current screen.</summary>
        void Enable();

        /// <summary>Disable this event on the current screen.</summary>
        void Disable();
    }

    #region treasure hunt

    public enum TreasureHuntType
    {
        Scavenger,
        Prospector
    }

    /// <summary>Interface for treasure hunts.</summary>
    public interface ITreasureHunt
    {
        /// <summary>Whether this instance pertains to a Scavenger or a Prospector.</summary>
        public TreasureHuntType Type { get; }

        /// <summary>Whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
        public bool IsActive { get; }

        /// <summary>The target tile containing treasure.</summary>
        public Vector2? TreasureTile { get; }

        /// <summary>Try to start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        public bool TryStart(GameLocation location);

        /// <summary>Forcefully start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        /// <param name="target">The target treasure tile.</param>
        public void ForceStart(GameLocation location, Vector2 target);

        /// <summary>End the active hunt unsuccessfully.</summary>
        public void Fail();
    }

    public interface ITreasureHuntStartedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }

        /// <summary>Whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>The coordinates of the target tile.</summary>
        public Vector2 Target { get; }
    }

    public interface ITreasureHuntEndedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }

        /// <summary>Whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Whether the player successfully discovered the treasure.</summary>
        public bool TreasureFound { get; }
    }

    #endregion treasure hunt

    #region ultimate

    public enum UltimateIndex
    {
        None = -1,
        Frenzy = 26,
        Ambush = 27,
        Pandemonium = 28,
        Blossom = 29
    }

    /// <summary>Interface for Ultimate abilities.</summary>
    public interface IUltimate : IDisposable
    {
        /// <summary>The index of this Ultimate, which corresponds to the index of the corresponding combat profession.</summary>
        UltimateIndex Index { get; }

        /// <summary>The current charge value.</summary>
        double ChargeValue { get; }

        /// <summary>The maximum charge value.</summary>
        int MaxValue { get; }

        /// <summary>The current charge value as a percentage.</summary>
        float PercentCharge { get; }

        /// <summary>Whether the current charge value is at max.</summary>
        bool IsFullyCharged { get; }

        /// <summary>Whether the current charge value is at zero.</summary>
        bool IsEmpty { get; }

        /// <summary>Whether this Ultimate is currently active.</summary>
        bool IsActive { get; }

        /// <summary>Check whether the <see cref="UltimateMeter"/> is currently showing.</summary>
        bool IsMeterVisible { get; }

        /// <summary>Check whether all activation conditions for this Ultimate are currently met.</summary>
        bool CanActivate { get; }
    }

    public interface IUltimateActivatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }
    }

    public interface IUltimateChargeIncreasedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }

        /// <summary>The previous charge value.</summary>
        public double OldValue { get; }

        /// <summary>The new charge value.</summary>
        public double NewValue { get; }
    }

    public interface IUltimateChargeInitiatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }

        /// <summary>The new charge value.</summary>
        public double NewValue { get; }
    }

    public interface IUltimateDeactivatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }
    }

    public interface IUltimateEmptiedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }
    }

    public interface IUltimateFullyChargedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        public Farmer Player { get; }
    }

    #endregion ultimate

    #region configs

    public interface IProfessionsConfig
    {
        /// <summary>Mod key used by Prospector and Scavenger professions.</summary>
        public KeybindList ModKey { get; }

        /// <summary>You must forage this many items before your forage becomes iridium-quality.</summary>
        public uint ForagesNeededForBestQuality { get; }

        /// <summary>You must mine this many minerals before your mined minerals become iridium-quality.</summary>
        public uint MineralsNeededForBestQuality { get; }

        /// <summary>If enabled, Automated machines will contribute toward EcologistItemsForaged and GemologistMineralsCollected.</summary>
        public bool ShouldCountAutomatedHarvests { get; }

        /// <summary>The chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
        public double ChanceToStartTreasureHunt { get; }

        /// <summary>Whether a Scavenger Hunt can trigger while entering a farm map.</summary>
        public bool AllowScavengerHuntsOnFarm { get; }

        /// <summary>Increase this multiplier if you find that Scavenger hunts end too quickly.</summary>
        public float ScavengerHuntHandicap { get; }

        /// <summary>Increase this multiplier if you find that Prospector hunts end too quickly.</summary>
        public float ProspectorHuntHandicap { get; }

        /// <summary>You must be this close to the treasure hunt target before the indicator appears.</summary>
        public float TreasureDetectionDistance { get; }

        /// <summary>The maximum speed bonus a Spelunker can reach.</summary>
        public uint SpelunkerSpeedCap { get; }

        /// <summary>Toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
        public bool EnableGetExcited { get; }

        /// <summary>Whether Seaweed and Algae are considered junk for fishing purposes.</summary>
        public bool SeaweedIsJunk { get; }

        /// <summary>You must catch this many fish of a given species to achieve instant catch.</summary>
        public uint FishNeededForInstantCatch { get; }

        /// <summary>If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.</summary>
        public float AnglerMultiplierCeiling { get; }

        /// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction next season.</summary>
        public uint TrashNeededPerTaxLevel { get; }

        /// <summary>You must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
        public uint TrashNeededPerFriendshipPoint { get; }

        /// <summary>The maximum tax deduction percentage allowed by the Ferngill Revenue Service.</summary>
        public float TaxDeductionCeiling { get; }

        /// <summary>The maximum stacks that can be gained for each buff stat.</summary>
        public uint PiperBuffCap { get; }

        /// <summary>Required to allow Ultimate activation. Super Stat continues to apply.</summary>
        public bool EnableUltimates { get; }

        /// <summary>Mod key used to activate Ultimate. Can be the same as <see cref="ModKey" />.</summary>
        public KeybindList UltimateKey { get; }

        /// <summary>Whether Ultimate is activated on <see cref="UltimateKey" /> hold (as opposed to press).</summary>
        public bool HoldKeyToActivateUltimate { get; }

        /// <summary>How long <see cref="UltimateKey" /> should be held to activate Ultimate, in seconds.</summary>
        public float UltimateActivationDelay { get; }

        /// <summary>Affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too slowly.</summary>
        public double UltimateGainFactor { get; }

        /// <summary>Affects the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last longer.</summary>
        public double UltimateDrainFactor { get; }

        /// <summary>Required to apply prestige changes.</summary>
        public bool EnablePrestige { get; }

        /// <summary>Multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
        public float SkillResetCostMultiplier { get; }

        /// <summary>Whether resetting a skill also clears all associated recipes.</summary>
        public bool ForgetRecipesOnSkillReset { get; }

        /// <summary>Whether the player can use the Statue of Prestige more than once per day.</summary>
        public bool AllowPrestigeMultiplePerDay { get; }

        /// <summary>Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.</summary>
        public float BonusSkillExpPerReset { get; }

        /// <summary>How much skill experience is required for each level up beyond 10.</summary>
        public uint RequiredExpPerExtendedLevel { get; }

        /// <summary>Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
        public uint PrestigeRespecCost { get; }

        /// <summary>Monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
        public uint ChangeUltCost { get; }

        /// <summary>Multiplies all skill experience gained from the start of the game.</summary>
        /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
        public float[] BaseSkillExpMultiplierPerSkill { get; }

        /// <summary>Increases the health of all monsters.</summary>
        public float MonsterHealthMultiplier { get; }

        /// <summary>Increases the damage dealt by all monsters.</summary>
        public float MonsterDamageMultiplier { get; }

        /// <summary>Increases the resistance of all monsters.</summary>
        public float MonsterDefenseMultiplier { get; }

        /// <summary>Enable if using the Vintage Interface v2 mod. Accepted values: "Brown", "Pink", "Off", "Automatic".</summary>
        public VintageInterfaceStyle VintageInterfaceSupport { get; }

        /// <summary>Determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons", "Gen4Ribbons".</summary>
        public ProgressionStyle Progression { get; }

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

    #endregion configs
}