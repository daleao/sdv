#pragma warning disable SA1201 // Elements should appear in the correct order
namespace DaLion.Common.Integrations.WalkOfLife;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Interface for proxying.</summary>
public interface IImmersiveProfessions
{
    /// <summary>Interface for all of the <see cref="Farmer"/>'s professions.</summary>
    public interface IProfession
    {
        /// <summary>Gets a string that uniquely identifies this profession.</summary>
        string StringId { get; }

        /// <summary>Gets the localized and gendered name for this profession.</summary>
        string DisplayName { get; }

        /// <summary>Gets the index used in-game to track professions acquired by the player.</summary>
        int Id { get; }

        /// <summary>Gets the level at which this profession is offered.</summary>
        /// <remarks>Either 5 or 10.</remarks>
        int Level { get; }

        /// <summary>Gets the <see cref="ISkill"/> which offers this profession.</summary>
        ISkill Skill { get; }

        /// <summary>Gets get the professions which branch off from this profession, if any.</summary>
        IEnumerable<int> BranchingProfessions { get; }

        /// <summary>Get the localized description text for this profession.</summary>
        /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
        /// <returns>A human-readability <see cref="string"/> description of the profession.</returns>
        string GetDescription(bool prestiged = false);
    }

    /// <summary>Interface for all of the <see cref="Farmer"/>'s skills.</summary>
    public interface ISkill
    {
        /// <summary>Gets the skill's unique string id.</summary>
        string StringId { get; }

        /// <summary>Gets the localized in-game name of this skill.</summary>
        string DisplayName { get; }

        /// <summary>Gets the current experience total gained by the local player for this skill.</summary>
        int CurrentExp { get; }

        /// <summary>Gets the current level for this skill.</summary>
        int CurrentLevel { get; }

        /// <summary>Gets the amount of experience required for the next level-up.</summary>
        int ExperienceToNextLevel { get; }

        /// <summary>Gets the base experience multiplier set by the player for this skill.</summary>
        float BaseExperienceMultiplier { get; }

        /// <summary>Gets the new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
        IEnumerable<int> NewLevels { get; }

        /// <summary>Gets the <see cref="IProfession"/>s associated with this skill.</summary>
        IList<IProfession> Professions { get; }

        /// <summary>Gets integer ids used in-game to track professions acquired by the player.</summary>
        IEnumerable<int> ProfessionIds { get; }

        /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
        /// <remarks>Should always contain exactly 2 elements.</remarks>
        IEnumerable<int> TierOneProfessionIds { get; }

        /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
        /// <remarks>
        ///     Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to
        ///     <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0
        ///     in the latter, and elements 2 and 3 correspond to branches of element 1.
        /// </remarks>
        IEnumerable<int> TierTwoProfessionIds { get; }
    }

    /// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
    public interface IManagedEvent
    {
        /// <summary>Gets a value indicating whether determines whether this event is enabled.</summary>
        bool IsEnabled { get; }

        /// <summary>Determines whether this event is enabled for a specific screen.</summary>
        /// <param name="screenId">A local peer's screen ID.</param>
        /// <returns><see langword="true"/> if the event is enabled for the specified screen, otherwise <see langword="false"/>.</returns>
        bool IsEnabledForScreen(int screenId);

        /// <summary>Enables this event on the current screen.</summary>
        /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
        bool Enable();

        /// <summary>Enables this event on the specified screen.</summary>
        /// <param name="screenId">A local peer's screen ID.</param>
        /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
        bool EnableForScreen(int screenId);

        /// <summary>Enables this event on the all screens.</summary>
        void EnableForAllScreens();

        /// <summary>Disables this event on the current screen.</summary>
        /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
        bool Disable();

        /// <summary>Disables this event on the specified screen.</summary>
        /// <param name="screenId">A local peer's screen ID.</param>
        /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
        bool DisableForScreen(int screenId);

        /// <summary>Disables this event on the all screens.</summary>
        void DisableForAllScreens();

        /// <summary>Resets this event's enabled state on all screens.</summary>
        void Reset();
    }

    #region treasure hunt

    /// <summary>The type of <see cref="ITreasureHunt"/>; either Scavenger or Prospector.</summary>
    public enum TreasureHuntType
    {
        /// <summary>A Scavenger Hunt.</summary>
        Scavenger,

        /// <summary>A Prospector Hunt.</summary>
        Prospector,
    }

    /// <summary>Interface for treasure hunts.</summary>
    public interface ITreasureHunt
    {
        /// <summary>Gets determines whether this instance pertains to a Scavenger or a Prospector.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets a value indicating whether determines whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
        bool IsActive { get; }

        /// <summary>Gets the target tile containing treasure.</summary>
        Vector2? TreasureTile { get; }

        /// <summary>Try to start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        /// <returns><see langword="true"/> if a hunt was started, otherwise <see langword="false"/>.</returns>
        bool TryStart(GameLocation location);

        /// <summary>Forcefully start a new hunt at the specified location.</summary>
        /// <param name="location">The game location.</param>
        /// <param name="target">The target treasure tile.</param>
        void ForceStart(GameLocation location, Vector2 target);

        /// <summary>End the active hunt unsuccessfully.</summary>
        void Fail();
    }

    /// <summary>Interface for the arguments of an event raised when a <see cref="ITreasureHunt"/> ends.</summary>
    public interface ITreasureHuntEndedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets determines whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets a value indicating whether determines whether the player successfully discovered the treasure.</summary>
        bool TreasureFound { get; }
    }

    /// <summary>Interface for the arguments of an event raised when a <see cref="ITreasureHunt"/> is begins.</summary>
    public interface ITreasureHuntStartedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets determines whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets the coordinates of the target tile.</summary>
        Vector2 Target { get; }
    }

    #endregion treasure hunt

    #region ultimate

    /// <summary>Interface for Ultimate abilities.</summary>
    public interface IUltimate
    {
        ///// <summary>Gets the corresponding combat profession.</summary>
        //IProfession Profession { get; }

        /// <summary>Gets the localized and gendered name for this <see cref="IUltimate"/>.</summary>
        string DisplayName { get; }

        /// <summary>Gets get the localized description text for this <see cref="IUltimate"/>.</summary>
        string Description { get; }

        /// <summary>Gets the index of the <see cref="IUltimate"/>, which equals the index of the corresponding combat profession.</summary>
        int Index { get; }

        /// <summary>Gets a value indicating whether determines whether this Ultimate is currently active.</summary>
        bool IsActive { get; }

        /// <summary>Gets or sets the current charge value.</summary>
        double ChargeValue { get; set; }

        /// <summary>Gets the maximum charge value.</summary>
        int MaxValue { get; }

        /// <summary>Gets a value indicating whether check whether all activation conditions for this Ultimate are currently met.</summary>
        bool CanActivate { get; }

        /// <summary>Gets a value indicating whether check whether the Ultimate HUD element is currently rendering.</summary>
        bool IsHudVisible { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> is activated.</summary>
    public interface IUltimateActivatedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> gains charge.</summary>
    public interface IUltimateChargeIncreasedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets the previous charge value.</summary>
        double OldValue { get; }

        /// <summary>Gets the new charge value.</summary>
        double NewValue { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> gain charge from zero.</summary>
    public interface IUltimateChargeInitiatedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets the new charge value.</summary>
        double NewValue { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> is deactivated.</summary>
    public interface IUltimateDeactivatedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> returns to zero charge.</summary>
    public interface IUltimateEmptiedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> reaches full charge.</summary>
    public interface IUltimateFullyChargedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }
    }

    #endregion ultimate

    #region configs

    /// <summary>The mod user-defined settings.</summary>
    public interface IModConfig
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
        KeybindList ModKey { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
        bool ShouldJunimosInheritProfessions { get; set; }

        /// <summary>Gets or sets add custom mod Artisan machines. Add to this list to make them compatible with the profession.</summary>
        string[] CustomArtisanMachines { get; set; }

        /// <summary>Gets or sets you must forage this many items before your forage becomes iridium-quality.</summary>
        uint ForagesNeededForBestQuality { get; set; }

        /// <summary>Gets or sets you must mine this many minerals before your mined minerals become iridium-quality.</summary>
        uint MineralsNeededForBestQuality { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether if enabled, machine and building ownership will be ignored when determining whether to apply profession
        ///     bonuses.
        /// </summary>
        bool LaxOwnershipRequirements { get; set; }

        /// <summary>Gets or sets changes the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
        float TrackPointerScale { get; set; }

        /// <summary>Gets or sets changes the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
        float TrackPointerBobbingRate { get; set; }

        /// <summary>Gets or sets a value indicating whether if enabled, Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
        bool DisableAlwaysTrack { get; set; }

        /// <summary>Gets or sets the chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
        double ChanceToStartTreasureHunt { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
        bool AllowScavengerHuntsOnFarm { get; set; }

        /// <summary>Gets or sets increase this multiplier if you find that Scavenger hunts end too quickly.</summary>
        float ScavengerHuntHandicap { get; set; }

        /// <summary>Gets or sets increase this multiplier if you find that Prospector hunts end too quickly.</summary>
        float ProspectorHuntHandicap { get; set; }

        /// <summary>Gets or sets you must be this close to the treasure hunt target before the indicator appears.</summary>
        float TreasureDetectionDistance { get; set; }

        /// <summary>Gets or sets the maximum speed bonus a Spelunker can reach.</summary>
        uint SpelunkerSpeedCap { get; set; }

        /// <summary>Gets or sets a value indicating whether toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
        bool EnableGetExcited { get; set; }

        /// <summary>Gets or sets you must catch this many fish of a given species to achieve instant catch.</summary>
        /// <remarks>Unused.</remarks>
        uint FishNeededForInstantCatch { get; set; }

        /// <summary>
        ///     Gets or sets if multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price
        ///     multiplier for fish sold by Angler.
        /// </summary>
        float AnglerMultiplierCap { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether determines whether to display the MAX icon below fish in the Collections Menu which have been caught at the
        ///     maximum size.
        /// </summary>
        bool ShowFishCollectionMaxIcon { get; set; }

        /// <summary>Gets or sets the maximum population of Aquarist Fish Ponds with legendary fish.</summary>
        uint LegendaryPondPopulationCap { get; set; }

        /// <summary>Gets or sets you must collect this many junk items from crab pots for every 1% of tax deduction the following season.</summary>
        uint TrashNeededPerTaxBonusPct { get; set; }

        /// <summary>Gets or sets you must collect this many junk items from crab pots for every 1 point of friendship towards villagers.</summary>
        uint TrashNeededPerFriendshipPoint { get; set; }

        /// <summary>Gets or sets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
        float ConservationistTaxBonusCeiling { get; set; }

        /// <summary>Gets or sets the maximum stacks that can be gained for each buff stat.</summary>
        uint PiperBuffCap { get; set; }

        /// <summary>Gets or sets a value indicating whether required to allow Ultimate activation. Super Stat continues to apply.</summary>
        bool EnableSpecials { get; set; }

        /// <summary>Gets or sets mod key used to activate Ultimate. Can be the same as <see cref="ModKey"/>.</summary>
        KeybindList SpecialActivationKey { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether Ultimate is activated on <see cref="SpecialActivationKey"/> hold (as opposed to press).</summary>
        bool HoldKeyToActivateSpecial { get; set; }

        /// <summary>Gets or sets how long <see cref="SpecialActivationKey"/> should be held to activate Ultimate, in seconds.</summary>
        float SpecialActivationDelay { get; set; }

        /// <summary>
        ///     Gets or sets affects the rate at which one builds the Ultimate meter. Increase this if you feel the gauge raises too
        ///     slowly.
        /// </summary>
        double SpecialGainFactor { get; set; }

        /// <summary>
        ///     Gets or sets affects the rate at which the Ultimate meter depletes during Ultimate. Decrease this to make Ultimate last
        ///     longer.
        /// </summary>
        double SpecialDrainFactor { get; set; }

        /// <summary>Gets or sets a value indicating whether required to apply prestige changes.</summary>
        bool EnablePrestige { get; set; }

        /// <summary>Gets or sets multiplies the base skill reset cost. Set to 0 to reset for free.</summary>
        float SkillResetCostMultiplier { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether resetting a skill also clears all corresponding recipes.</summary>
        bool ForgetRecipes { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether the player can use the Statue of Prestige more than once per day.</summary>
        bool AllowMultiplePrestige { get; set; }

        /// <summary>Gets or sets cumulative multiplier to each skill's experience gain after a respective skill reset.</summary>
        float PrestigeExpMultiplier { get; set; }

        /// <summary>Gets or sets how much skill experience is required for each level up beyond 10.</summary>
        uint RequiredExpPerExtendedLevel { get; set; }

        /// <summary>Gets or sets monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
        uint PrestigeRespecCost { get; set; }

        /// <summary>Gets or sets monetary cost of changing the combat Ultimate. Set to 0 to change for free.</summary>
        uint ChangeUltCost { get; set; }

        /// <summary>Gets or sets multiplies all skill experience gained from the start of the game.</summary>
        /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat.</remarks>
        float[] BaseSkillExpMultipliers { get; set; }

        /// <summary>Gets or sets multiplies all skill experience gained from the start of the game, for custom skills.</summary>
        public Dictionary<string, float> CustomSkillExpMultipliers { get; set; }

        /// <summary>Gets or sets enable if using the Vintage Interface v2 mod. Accepted values: "Brown", "Pink", "Off", "Automatic".</summary>
        VintageInterfaceStyle VintageInterfaceSupport { get; set; }

        /// <summary>
        ///     Gets or sets determines the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons",
        ///     "Gen4Ribbons".
        /// </summary>
        ProgressionStyle PrestigeProgressionStyle { get; set; }
    }

    #endregion configs
}
#pragma warning restore SA1201 // Elements should appear in the correct order
