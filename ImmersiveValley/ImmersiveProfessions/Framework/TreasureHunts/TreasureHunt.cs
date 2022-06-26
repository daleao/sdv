namespace DaLion.Stardew.Professions.Framework.TreasureHunts;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

using Framework.Events.TreasureHunt;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool IsActive => TreasureTile is not null;

    /// <inheritdoc />
    public Vector2? TreasureTile { get; protected set; } = null;

    #region event handlers

    /// <inheritdoc cref="OnStarted"/>
    internal static event EventHandler<ITreasureHuntStartedEventArgs>? Started;

    /// <inheritdoc cref="OnEnded"/>
    internal static event EventHandler<ITreasureHuntEndedEventArgs>? Ended;

    #endregion event handlers

    protected uint elapsed;
    protected uint timeLimit;
    protected string huntStartedMessage = null!;
    protected string huntFailedMessage = null!;
    protected GameLocation huntLocation = null!;
    protected Rectangle iconSourceRect;
    protected readonly Random random = new(Guid.NewGuid().GetHashCode());
    
    private double _chanceAccumulator = 1.0;

    /// <summary>Construct an instance.</summary>
    internal TreasureHunt()
    {
        Type = GetType() == typeof(ScavengerHunt) ? TreasureHuntType.Scavenger : TreasureHuntType.Prospector;
    }

    #region public methods

    /// <inheritdoc />
    public abstract bool TryStart(GameLocation location);

    /// <inheritdoc />
    public abstract void ForceStart(GameLocation location, Vector2 target);

    /// <inheritdoc />
    public abstract void Fail();

    #endregion public methods

    #region internal methods

    /// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
    internal void ResetChanceAccumulator()
    {
        _chanceAccumulator = 1.0;
    }

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    internal void Update(uint ticks)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        if (ticks % 60 == 0 && ++elapsed > timeLimit) Fail();
        else CheckForCompletion();
    }

    #endregion internal methods

    #region protected methods

    /// <summary>Roll the dice for a new treasure hunt or adjust the odds for the next attempt.</summary>
    /// <returns><see langword="true"> if the dice roll was successful, otherwise <see langword="false">.</returns>
    protected bool TryStart()
    {
        if (IsActive) return false;

        if (random.NextDouble() > ModEntry.Config.ChanceToStartTreasureHunt * _chanceAccumulator)
        {
            _chanceAccumulator *= 1.0 + Game1.player.DailyLuck;
            return false;
        }

        _chanceAccumulator = 1.0;
        return true;
    }

    /// <summary>Check if a treasure hunt can be started immediately and adjust the odds for the next attempt.</summary>
    protected virtual void ForceStart()
    {
        if (IsActive) throw new InvalidOperationException("A Treasure Hunt is already active in this instance.");
        _chanceAccumulator = 1.0;
    }

    /// <summary>Select a random tile and make sure it is a valid treasure target.</summary>
    /// <param name="location">The game location.</param>
    protected abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Check if the player has found the treasure tile.</summary>
    protected abstract void CheckForCompletion();

    /// <summary>Reset treasure tile and release treasure hunt update event.</summary>
    protected abstract void End(bool found);

    #endregion protected methods

    #region event callbacks

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    protected void OnStarted()
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(Game1.player, Type, TreasureTile!.Value));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    protected void OnEnded(bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(Game1.player, Type, found));
    }

    #endregion event callbacks

}