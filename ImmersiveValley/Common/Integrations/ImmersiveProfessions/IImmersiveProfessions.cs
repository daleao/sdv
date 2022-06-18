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
}