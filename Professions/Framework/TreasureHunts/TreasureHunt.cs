namespace DaLion.Professions.Framework.TreasureHunts;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntEnded;
using DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntStarted;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using Broadcaster = DaLion.Shared.Networking.Broadcaster;

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
    public Vector2? TreasureTile { get; protected set; }

    /// <inheritdoc />
    public GameLocation? Location { get; protected set; }

    /// <inheritdoc />
    public bool IsActive => this.TreasureTile is not null;

    /// <inheritdoc />
    public uint Elapsed { get; protected set; }

    /// <inheritdoc />
    public uint TimeLimit { get; protected set; }

    /// <summary>Gets a random number generator.</summary>
    protected Random Random { get; } = new(Guid.NewGuid().GetHashCode());

    /// <summary>Gets the profession icon source <see cref="Rectangle"/>.</summary>
    protected Rectangle IconSourceRect { get; }

    /// <summary>Gets the hunt started message.</summary>
    protected string HuntStartedMessage { get; }

    /// <summary>Gets the hunt failed message.</summary>
    protected string HuntFailedMessage { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Location), nameof(TreasureTile))]
    public virtual bool TryStart(double chance)
    {
        if (this.IsActive || !this.Random.NextBool(chance))
        {
            return false;
        }

        if (!this.TrySetTreasureTile())
        {
            return false;
        }

        this.Location = Game1.currentLocation;
        this.Elapsed = 0;
        HudPointer.Instance.ShouldBob = true;
        if (Context.IsMultiplayer)
        {
            Game1.player.Get_IsHuntingTreasure().Value = true;
            Broadcaster.SendPublicChat(I18n.TreasureHunt_Broadcast_Started(Game1.player.Name));
        }

        this.StartImpl(this.Location, this.TreasureTile.Value);
        return true;
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Location), nameof(TreasureTile))]
    public virtual void ForceStart(Vector2 target)
    {
        if (this.IsActive)
        {
            ThrowHelper.ThrowInvalidOperationException("A Treasure Hunt is already active in this instance.");
        }

        this.TreasureTile = target;
        this.Location = Game1.currentLocation;
        this.Elapsed = 0;
        HudPointer.Instance.ShouldBob = true;
        if (Context.IsMultiplayer)
        {
            Game1.player.Get_IsHuntingTreasure().Value = true;
            Broadcaster.SendPublicChat(I18n.TreasureHunt_Broadcast_Started(Game1.player.Name));
        }

        this.StartImpl(this.Location, target);
    }

    /// <inheritdoc />
    public abstract void Complete();

    /// <inheritdoc />
    public abstract void Fail();

    /// <summary>Check for completion or failure.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started.</param>
    internal void Update(uint ticks)
    {
        if (!Game1.game1.ShouldTimePass())
        {
            return;
        }

        if (ticks % 60 == 0 && ++this.Elapsed > this.TimeLimit)
        {
            this.Fail();
        }
    }

    /// <summary>Attempts to select the target treasure tile.</summary>
    /// <returns><see langword="true"/> if a valid tile was set, other <see langword="false"/>.</returns>
    [MemberNotNullWhen(true, nameof(TreasureTile))]
    protected abstract bool TrySetTreasureTile();

    /// <summary>Sets the <see cref="TimeLimit"/> for this hunt.</summary>
    /// <returns>The set time limit, for convenience.</returns>
    protected abstract uint SetTimeLimit();

    /// <summary>Resets treasure tile and releases the treasure hunt update event.</summary>
    /// <param name="success">Whether the treasure was successfully found.</param>
    protected virtual void End(bool success)
    {
        this.TreasureTile = null;
        HudPointer.Instance.ShouldBob = false;
        if (Context.IsMultiplayer)
        {
            Game1.player.Get_IsHuntingTreasure().Value = false;
            Broadcaster.SendPublicChat(success
                ? I18n.TreasureHunt_Broadcast_Ended_Success(Game1.player.Name)
                : I18n.TreasureHunt_Broadcast_Ended_Failure(Game1.player.Name));
        }

        this.OnEnded(success);
        Log.D(success ? "Found the treasure!" : "Failed the treasure hunt.");
    }

    /// <summary>Start-up logic implementation.</summary>
    /// <param name="location">Reference to <see cref="Location"/>.</param>
    /// <param name="treasureTile">Reference to the chosen <see cref="TreasureTile"/>.</param>
    protected virtual void StartImpl(GameLocation location, Vector2 treasureTile)
    {
        var timeLimit = this.SetTimeLimit();
        this.OnStarted(treasureTile, timeLimit);
    }

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    /// <param name="treasureTile">Reference to the chosen <see cref="TreasureTile"/>.</param>
    /// <param name="timeLimit">Reference to the <see cref="TimeLimit"/>.</param>
    private void OnStarted(Vector2 treasureTile, uint timeLimit)
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(Game1.player, this.Profession, treasureTile, timeLimit));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    private void OnEnded(bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(Game1.player, this.Profession, found));
    }
}
