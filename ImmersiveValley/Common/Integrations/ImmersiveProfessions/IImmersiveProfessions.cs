namespace DaLion.Common.Integrations.WalkOfLife;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Interface for proxying.</summary>
public interface IImmersiveProfessions
{
    /// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
    public interface IManagedEvent
    {
        /// <summary>Whether this event is enabled.</summary>
        bool IsEnabled { get; }

        /// <summary>Whether this event is enabled for a specific splitscreen player.</summary>
        /// <param name="screenId">The player's screen id.</param>
        bool IsEnabledForScreen(int screenId);

        /// <summary>Enable this event on the current screen.</summary>
        bool Enable();

        /// <summary>Disable this event on the current screen.</summary>
        bool Disable();
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
        TreasureHuntType Type { get; }

        /// <summary>Whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
        bool IsActive { get; }

        /// <summary>The target tile containing treasure.</summary>
        Vector2? TreasureTile { get; }

        /// <summary>Try to start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        /// <returns><see langword="true"> if a hunt was started, otherwise <see langword="false">.</returns>
        bool TryStart(GameLocation location);

        /// <summary>Forcefully start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        /// <param name="target">The target treasure tile.</param>
        void ForceStart(GameLocation location, Vector2 target);

        /// <summary>End the active hunt unsuccessfully.</summary>
        void Fail();
    }

    /// <summary>Interface for the arguments of a <see cref="TreasureHuntStartedEvent"/>.</summary>
    public interface ITreasureHuntEndedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Whether the player successfully discovered the treasure.</summary>
        bool TreasureFound { get; }
    }

    /// <summary>Interface for the arguments of a <see cref="TreasureHuntStartedEvent"/>.</summary>
    public interface ITreasureHuntStartedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>The coordinates of the target tile.</summary>
        Vector2 Target { get; }
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
    public interface IUltimate
    {
        /// <summary>The index of this Ultimate, which equals the index of the corresponding combat profession.</summary>
        UltimateIndex Index { get; }

        /// <summary>Whether this Ultimate is currently active.</summary>
        bool IsActive { get; }

        /// <summary>The current charge value.</summary>
        double ChargeValue { get; set; }

        /// <summary>The maximum charge value.</summary>
        int MaxValue { get; }

        /// <summary>Check whether all activation conditions for this Ultimate are currently met.</summary>
        bool CanActivate { get; }

        /// <summary>Check whether the <see cref="UltimateHUD"/> is currently showing.</summary>
        bool IsHudVisible { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateActivatedEvent"/>.</summary>
    public interface IUltimateActivatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateChargeIncreasedEvent"/>.</summary>
    public interface IUltimateChargeIncreasedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>The previous charge value.</summary>
        double OldValue { get; }

        /// <summary>The new charge value.</summary>
        double NewValue { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateChargeInitiatedEvent"/>.</summary>
    public interface IUltimateChargeInitiatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>The new charge value.</summary>
        double NewValue { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateDeactivatedEvent"/>.</summary>
    public interface IUltimateDeactivatedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateEmptiedEvent"/>.</summary>
    interface IUltimateEmptiedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an <see cref="UltimateFullyChargedEvent"/>.</summary>
    interface IUltimateFullyChargedEventArgs
    {
        /// <summary>The player who triggered the event.</summary>
        Farmer Player { get; }
    }

    #endregion ultimate

    #region configs

    /// <summary>The mod user-defined settings.</summary>
    public interface IModConfig
    {
        /// <summary>Mod key used by Prospector and Scavenger professions.</summary>
        KeybindList ModKey { get; set; }

        /// <summary>Whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
        bool ShouldJunimosInheritProfessions { get; set; }

        /// <summary>Add custom mod Artisan machines to this list to make them compatible with the profession.</summary>
        string[] CustomArtisanMachines { get; }

        /// <summary>You must forage this many items before your forage becomes iridium-quality.</summary>
        uint ForagesNeededForBestQuality { get; set; }

        /// <summary>You must mine this many minerals before your mined minerals become iridium-quality.</summary>
        uint MineralsNeededForBestQuality { get; set; }

        /// <summary>If enabled, machine and building ownership will be ignored when determining whether to apply profession bonuses.</summary>
        bool LaxOwnershipRequirements { get; set; }

        /// <summary>Changes the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
        float TrackPointerScale { get; set; }

        /// <summary>Changes the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
        float TrackPointerBobbingRate { get; set; }

        /// <summary>If enabled, Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
        bool DisableAlwaysTrack { get; set; }

        /// <summary>The chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
        double ChanceToStartTreasureHunt { get; set; }

        /// <summary>Whether a Scavenger Hunt can trigger while entering a farm map.</summary>
        bool AllowScavengerHuntsOnFarm { get; set; }

        /// <summary>Increase this multiplier if you find that Scavenger hunts end too quickly.</summary>
        float ScavengerHuntHandicap { get; set; }

        /// <summary>Increase this multiplier if you find that Prospector hunts end too quickly.</summary>
        float ProspectorHuntHandicap { get; set; }

        /// <summary>You must be this close to the treasure hunt target before the indicator appears.</summary>
        float TreasureDetectionDistance { get; set; }

        /// <summary>The maximum speed bonus a Spelunker can reach.</summary>
        uint SpelunkerSpeedCap { get; set; }

        /// <summary>Toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
        bool EnableGetExcited { get; set; }

        /// <summary>Whether Seaweed and Algae are considered junk for fishing purposes.</summary>
        bool SeaweedIsTrash { get; set; }

        /// <summary>You must catch this many fish of a given species to achieve instant catch.</summary>
        /// <remarks>Unused.</remarks>
        uint FishNeededForInstantCatch { get; set; }

        /// <summary>If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.</summary>
        float AnglerMultiplierCap { get; set; }

        /// <summary>The maximum population of Aquarist Fish Ponds with legendary fish.</summary>
        uint LegendaryPondPopulationCap { get; set; }

        /// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction the following season.</summary>
        uint TrashNeededPerTaxBonusPct { get; set; }

        /// <summary>You must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
        uint TrashNeededPerFriendshipPoint { get; set; }

        /// <summary>The maximum income deduction allowed by the Ferngill Revenue Service.</summary>
        float ConservationistTaxBonusCeiling { get; set; }

        /// <summary>The maximum stacks that can be gained for each buff stat.</summary>
        uint PiperBuffCap { get; set; }

        /// <summary>Required to allow Ultimate activation. Super Stat continues to apply.</summary>
        bool EnableSpecials { get; set; }

        /// <summary>Mod key used to activate Ultimate. Can be the same as <see cref="ModKey" />.</summary>
        KeybindList SpecialActivationKey { get; set; }

        /// <summary>Whether Ultimate is activated on <see cref="SpecialActivationKey" /> hold (as opposed to press).</summary>
        bool HoldKeyToActivateSpecial { get; set; }

        /// <summary>How long <see cref="SpecialActivationKey" /> should be held to activate Ultimate, in seconds.</summary>
        float SpecialActivationDelay { get; set; }

        /// <summary>Affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too slowly.</summary>
        double SpecialGainFactor { get; set; }

        /// <summary>Affects the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last longer.</summary>
        double SpecialDrainFactor { get; set; }

        /// <summary>Required to apply prestige changes.</summary>
        bool EnablePrestige { get; set; }

        /// <summary>Multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
        float SkillResetCostMultiplier { get; set; }

        /// <summary>Whether resetting a skill also clears all corresponding recipes.</summary>
        bool ForgetRecipesOnSkillReset { get; set; }

        /// <summary>Whether the player can use the Statue of Prestige more than once per day.</summary>
        bool AllowPrestigeMultiplePerDay { get; set; }

        /// <summary>Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.</summary>
        float BonusSkillExpPerReset { get; set; }

        /// <summary>How much skill experience is required for each level up beyond 10.</summary>
        uint RequiredExpPerExtendedLevel { get; set; }

        /// <summary>Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
        uint PrestigeRespecCost { get; set; }

        /// <summary>Monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
        uint ChangeUltCost { get; set; }

        /// <summary>Multiplies all skill experience gained from the start of the game.</summary>
        /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
        float[] BaseSkillExpMultiplierPerSkill { get; set; }

        /// <summary>Increases the health of all monsters.</summary>
        float MonsterHealthMultiplier { get; set; }

        /// <summary>Increases the damage dealt by all monsters.</summary>
        float MonsterDamageMultiplier { get; set; }

        /// <summary>Increases the resistance of all monsters.</summary>
        float MonsterDefenseMultiplier { get; set; }

        /// <summary>Enable if using the Vintage Interface v2 mod. Accepted values: "Brown", "Pink", "Off", "Automatic".</summary>
        VintageInterfaceStyle VintageInterfaceSupport { get; set; }

        /// <summary>Determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons", "Gen4Ribbons".</summary>
        ProgressionStyle PrestigeProgressionStyle { get; set; }

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