#pragma warning disable SA1201 // Elements should appear in the correct order
namespace DaLion.Overhaul.API;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interfaces required for proxying the mod API.</summary>
public interface IModularOverhaul
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

        /// <summary>Gets the professions which branch off from this profession, if any.</summary>
        IEnumerable<int> BranchingProfessions { get; }

        /// <summary>Gets the localized description text for this profession.</summary>
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
        /// <summary>Gets a value indicating whether this event is enabled.</summary>
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

    #region treasure hunts

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
        /// <summary>Gets whether this instance pertains to a Scavenger or a Prospector.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets a value indicating whether the <see cref="TreasureTile"/> is set to a valid target.</summary>
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

        /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets a value indicating whether the player successfully discovered the treasure.</summary>
        bool TreasureFound { get; }
    }

    /// <summary>Interface for the arguments of an event raised when a <see cref="ITreasureHunt"/> is begins.</summary>
    public interface ITreasureHuntStartedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
        TreasureHuntType Type { get; }

        /// <summary>Gets the coordinates of the target tile.</summary>
        Vector2 Target { get; }
    }

    #endregion treasure hunts

    #region limit break

    /// <summary>Interface for Ultimate abilities.</summary>
    public interface IUltimate
    {
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

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> charge increases.</summary>
    public interface IUltimateChargeIncreasedEventArgs
    {
        /// <summary>Gets the player who triggered the event.</summary>
        Farmer Player { get; }

        /// <summary>Gets the previous charge value.</summary>
        double OldValue { get; }

        /// <summary>Gets the new charge value.</summary>
        double NewValue { get; }
    }

    /// <summary>Interface for the arguments of an event raised when <see cref="IUltimate"/> gains charge from zero.</summary>
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

    #endregion limit break

    #region resonances

    /// <summary>The number of steps between two <see cref="IGemstone"/>s in a Diatonic Scale.</summary>
    public enum IntervalNumber
    {
        /// <summary>Zero. Both <see cref="IGemstone"/>s are identical.</summary>
        Unison,

        /// <summary>The second <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Second,

        /// <summary>The third <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Third,

        /// <summary>The fourth <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Fourth,

        /// <summary>The fifth <see cref="IGemstone"/> in the Diatonic Scale, also known as the Dominant.</summary>
        Fifth,

        /// <summary>The sixth <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Sixth,

        /// <summary>The seventh <see cref="IGemstone"/> in the Diatonic Scale.</summary>
        Seventh,

        /// <summary>A full scale. Effectively the same as <see cref="Unison"/>.</summary>
        Octave,
    }

    /// <summary>A harmonic set of <see cref="IGemstone"/> wavelengths.</summary>
    /// <remarks>
    ///     The interference of vibration patterns between neighboring <see cref="IGemstone"/>s may amplify, dampen or
    ///     even create new overtones.
    /// </remarks>
    public interface IChord
    {
        /// <summary>Gets the <see cref="IGemstone"/>s that make up the <see cref="IChord"/>.</summary>
        /// <remarks>
        ///     The notes are sorted by resulting harmony, with the <see cref="Root"/> at index zero and remaining notes
        ///     ordered by increasing intervals with the former.
        /// </remarks>
        IGemstone[] Notes { get; }

        /// <summary>
        ///     Gets the root <see cref="IGemstone"/> of the <see cref="IChord"/>, which determines the
        ///     perceived wavelength.
        /// </summary>
        IGemstone? Root { get; }

        /// <summary>Gets the amplitude of the <see cref="Root"/> note's resonance.</summary>
        double Amplitude { get; }
    }

    /// <summary>A gemstone which can be applied to an Infinity Band.</summary>
    /// <remarks>
    ///     Each <see cref="IGemstone"/> vibrates with a characteristic wavelength, which allows it to resonate with
    ///     others in the Diatonic Scale of <see cref="IGemstone"/>.
    /// </remarks>
    public interface IGemstone
    {
        /// <summary>Gets the index of the corresponding <see cref="SObject"/>.</summary>
        int ObjectIndex { get; }

        /// <summary>Gets the index of the corresponding <see cref="StardewValley.Objects.Ring"/>.</summary>
        int RingIndex { get; }

        /// <summary>Gets the characteristic frequency with which the <see cref="IGemstone"/> vibrates.</summary>
        /// <remarks>Measured in units of inverse Ruby wavelengths.</remarks>
        float Frequency { get; }

        /// <summary>Gets the characteristic color which results from <see cref="Frequency"/>.</summary>
        Color StoneColor { get; }

        /// <summary>Gets the inverse <see cref="StoneColor"/>.</summary>
        Color InverseColor { get; }

        /// <summary>Gets the second <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Second { get; }

        /// <summary>Gets the third <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Third { get; }

        /// <summary>Gets the fourth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Fourth { get; }

        /// <summary>Gets the fifth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Fifth { get; }

        /// <summary>Gets the sixth <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Sixth { get; }

        /// <summary>Gets the seventh <see cref="IGemstone"/> in the corresponding Diatonic Scale.</summary>
        IGemstone Seventh { get; }

        /// <summary>
        ///     Gets the ascending diatonic <see cref="IntervalNumber"/> between this and some other
        ///     <see cref="IGemstone"/>.
        /// </summary>
        /// <param name="other">Some other <see cref="IGemstone"/>.</param>
        /// <returns>The <see cref="IntervalNumber"/> between this and <paramref name="other"/>.</returns>
        IntervalNumber IntervalWith(IGemstone other);
    }

    #endregion resonances
}
#pragma warning restore SA1201 // Elements should appear in the correct order
