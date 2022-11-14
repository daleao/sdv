namespace DaLion.Ligo.Modules.Professions.TreasureHunts;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.TreasureHunt;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    private double _chanceAccumulator = 1d;

    /// <summary>Initializes a new instance of the <see cref="TreasureHunt"/> class.</summary>
    /// <param name="type">Either <see cref="Profession.Scavenger"/> or <see cref="Profession.Prospector"/>.</param>
    /// <param name="huntStartedMessage">The message displayed to the player when the hunt starts.</param>
    /// <param name="huntFailedMessage">The message displayed to the player when the hunt fails.</param>
    /// <param name="iconSourceRect">The <see cref="Rectangle"/> area of the corresponding profession's icon.</param>
    internal TreasureHunt(TreasureHuntType type, string huntStartedMessage, string huntFailedMessage, Rectangle iconSourceRect)
    {
        this.Type = type;
        this.HuntStartedMessage = huntStartedMessage;
        this.HuntFailedMessage = huntFailedMessage;
        this.IconSourceRect = iconSourceRect;
    }

    /// <inheritdoc cref="OnStarted"/>
    internal static event EventHandler<ITreasureHuntStartedEventArgs>? Started;

    /// <inheritdoc cref="OnEnded"/>
    internal static event EventHandler<ITreasureHuntEndedEventArgs>? Ended;

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool IsActive => this.TreasureTile is not null;

    /// <inheritdoc />
    public Vector2? TreasureTile { get; protected set; } = null;

    /// <summary>Gets a random number generator.</summary>
    protected Random Random { get; } = new(Guid.NewGuid().GetHashCode());

    /// <summary>Gets or sets the active hunt's <see cref="GameLocation"/>.</summary>
    protected GameLocation HuntLocation { get; set; } = null!; // not null when treasure tile is not null

    /// <summary>Gets the profession icon source <see cref="Rectangle"/>.</summary>
    protected Rectangle IconSourceRect { get; }

    /// <summary>Gets the hunt started message.</summary>
    protected string HuntStartedMessage { get; }

    /// <summary>Gets the hunt failed message.</summary>
    protected string HuntFailedMessage { get; }

    /// <summary>Gets or sets the elapsed time of the active hunt.</summary>
    protected uint Elapsed { get; set; }

    /// <summary>Gets or sets the time limit of the active hunt.</summary>
    protected uint TimeLimit { get; set; }

    /// <inheritdoc />
    public abstract bool TryStart(GameLocation location);

    /// <inheritdoc />
    public abstract void ForceStart(GameLocation location, Vector2 target);

    /// <inheritdoc />
    public abstract void Fail();

    /// <summary>Reset the accumulated bonus chance to trigger a new hunt.</summary>
    internal void ResetChanceAccumulator()
    {
        this._chanceAccumulator = 1d;
    }

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    internal void Update(uint ticks)
    {
        if ((!Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) || !Game1.shouldTimePass())
        {
            return;
        }

        if (ticks % 60 == 0 && ++this.Elapsed > this.TimeLimit)
        {
            this.Fail();
        }
        else
        {
            this.CheckForCompletion();
        }
    }

    /// <summary>Rolls the dice for a new treasure hunt or adjusts the odds for the next attempt.</summary>
    /// <returns><see langword="true"/> if the dice roll was successful, otherwise <see langword="false"/>.</returns>
    protected bool TryStart()
    {
        if (this.IsActive)
        {
            return false;
        }

        if (this.Random.NextDouble() > ModEntry.Config.Professions.ChanceToStartTreasureHunt * this._chanceAccumulator)
        {
            this._chanceAccumulator *= 1d + Game1.player.DailyLuck;
            return false;
        }

        this._chanceAccumulator = 1d;
        return true;
    }

    /// <summary>Forcefully sets the odds for the next hunt start attempt to 100%.</summary>
    protected virtual void ForceStart()
    {
        if (this.IsActive)
        {
            ThrowHelper.ThrowInvalidOperationException("A Treasure Hunt is already active in this instance.");
        }

        this._chanceAccumulator = 1d;
    }

    /// <summary>Selects a random tile and determines whether it is a valid treasure target.</summary>
    /// <param name="location">The game location.</param>
    /// <returns>A <see cref="Vector2"/> tile.</returns>
    protected abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Determines whether the treasure tile has been found.</summary>
    protected abstract void CheckForCompletion();

    /// <summary>Resets treasure tile and releases the treasure hunt update event.</summary>
    /// <param name="found">Whether the treasure was successfully found.</param>
    protected abstract void End(bool found);

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    protected void OnStarted()
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(Game1.player, this.Type, this.TreasureTile!.Value));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    protected void OnEnded(bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(Game1.player, this.Type, found));
    }
}
