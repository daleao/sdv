namespace DaLion.Professions.Framework.Hunting;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Extensions.Stardew;
using Events;
using Microsoft.Xna.Framework;
using Shared.Extensions;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHunt"/> class.</summary>
    /// <param name="profession">Either <see cref="Profession.Scavenger"/> or <see cref="Profession.Prospector"/>.</param>
    /// <param name="huntStartedMessage">The message displayed to the player when the hunt starts.</param>
    /// <param name="huntFailedMessage">The message displayed to the player when the hunt fails.</param>
    /// <param name="iconSourceRect">The <see cref="Rectangle"/> area of the corresponding profession's icon.</param>
    internal TreasureHunt(TreasureHuntProfession profession, string huntStartedMessage, string huntFailedMessage, Rectangle iconSourceRect)
    {
        this.Profession = profession;
        this.HuntStartedMessage = huntStartedMessage;
        this.HuntFailedMessage = huntFailedMessage;
        this.IconSourceRect = iconSourceRect;
    }

    /// <inheritdoc cref="OnStarted"/>
    internal static event EventHandler<ITreasureHuntStartedEventArgs>? Started;

    /// <inheritdoc cref="OnEnded"/>
    internal static event EventHandler<ITreasureHuntEndedEventArgs>? Ended;

    /// <inheritdoc />
    public TreasureHuntProfession Profession { get; }

    /// <inheritdoc />
    public Vector2? TargetTile { get; protected set; }

    /// <inheritdoc />
    public GameLocation? Location { get; protected set; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(TargetTile))]
    [MemberNotNullWhen(true, nameof(Location))]
    public bool IsActive => this.TargetTile is not null;

    /// <inheritdoc />
    public int Elapsed { get; protected set; }

    /// <inheritdoc />
    public abstract int TimeLimit { get; }

    /// <summary>Gets or sets the number of pooled points towards triggering a hunt.</summary>
    public abstract int TriggerPool { get; protected set; }

    /// <summary>Gets the pool point threshold required to trigger a hunt.</summary>
    protected abstract int TriggerThreshold { get; }

    /// <summary>Gets a random number generator.</summary>
    protected Random Random { get; } = new(Guid.NewGuid().GetHashCode());

    /// <summary>Gets the profession icon source <see cref="Rectangle"/>.</summary>
    protected Rectangle IconSourceRect { get; }

    /// <summary>Gets the hunt started message.</summary>
    protected string HuntStartedMessage { get; }

    /// <summary>Gets the hunt failed message.</summary>
    protected string HuntFailedMessage { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Location), nameof(TargetTile))]
    public virtual bool TryStart(GameLocation location)
    {
        if (this.IsActive || ReferenceEquals(this.Location, location) || location.currentEvent is not null ||
            !this.IsLocationSuitable(location) || !this.ChooseTreasureTile(location))
        {
            Log.D("[Treasure Hunt]: Failed to start.");
            return false;
        }

        this.Location = location;
        this.Elapsed = 0;
        HudPointer.Instance.ShouldBob = true;
        this.StartImpl(this.Location, this.TargetTile.Value);
        Log.D($"[Treasure Hunt]: Started hunt at {location}.");
        return true;
    }

    /// <inheritdoc />
    public abstract void Complete();

    /// <inheritdoc />
    public abstract void Fail();

    /// <summary>Updates the <see cref="TriggerPool"/>.</summary>
    /// <param name="criteria">The respective point-granting criteria.</param>
    /// <remarks>Expected criteria quantity and order is defined by the implementation.</remarks>
    public abstract void UpdateTriggerPool(params int[] criteria);

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    internal virtual void TimeUpdate(uint ticks)
    {
        if (!Game1.game1.ShouldTimePass())
        {
            return;
        }

#if RELEASE
        if (ticks % 60 == 0 && ++this.Elapsed > this.TimeLimit)
        {
            this.Fail();
        }
#endif
    }

    /// <summary>Attempts to select the target treasure tile.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if a valid tile was set, otherwise <see langword="false"/>.</returns>
    [MemberNotNullWhen(true, nameof(TargetTile))]
    protected abstract bool ChooseTreasureTile(GameLocation location);

    /// <summary>Checks if the current location is suitable for a hunt.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if a hunt can begin at the location, otherwise <see langword="false"/>.</returns>
    protected abstract bool IsLocationSuitable(GameLocation location);

    /// <summary>Resets treasure tile and releases the treasure hunt update event.</summary>
    /// <param name="success">Whether the treasure was successfully found.</param>
    protected virtual void End(bool success)
    {
        this.TargetTile = null;
        this.Location = null;
        HudPointer.Instance.ShouldBob = false;
        this.OnEnded(success);
        Log.D(success ? "[Treasure Hunt]: Treasure found!" : "[Treasure Hunt]: Failed!");
    }

    /// <summary>Start-up logic implementation.</summary>
    /// <param name="location">Reference to <see cref="Location"/>.</param>
    /// <param name="treasureTile">Reference to the chosen <see cref="TargetTile"/>.</param>
    protected virtual void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        this.OnStarted(treasureTile, this.TimeLimit);
    }

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    /// <param name="treasureTile">Reference to the chosen <see cref="TargetTile"/>.</param>
    /// <param name="timeLimit">Reference to the <see cref="TimeLimit"/>.</param>
    private void OnStarted(Vector2 treasureTile, int timeLimit)
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(Game1.player, this.Profession, treasureTile, timeLimit));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    private void OnEnded(bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(Game1.player, this.Profession, found));
    }

    /// <summary>Rolls a big fat stack of ores or metal bars.</summary>
    /// <param name="baseMin">The minimum value of the base roll.</param>
    /// <param name="baseMax">The maximum value of the base roll.</param>
    /// <param name="chanceToDouble">The base chance to double the stack.</param>
    /// <param name="chanceDecay">The decay to the <paramref name="chanceToDouble"/> after a successful doubling.</param>
    /// <returns>A big fat stack.</returns>
    protected int RollStack(int baseMin, int baseMax, double chanceToDouble, double chanceDecay)
    {
        var stack = this.Random.Next(baseMin, baseMax + 1);
        while (this.Random.NextBool(chanceToDouble))
        {
            stack *= 2;
            chanceToDouble *= chanceDecay;
        }

        return stack;
    }
}
