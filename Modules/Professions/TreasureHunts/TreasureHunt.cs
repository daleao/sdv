namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntEnded;
using DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntStarted;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Broadcaster = DaLion.Shared.Networking.Broadcaster;

#endregion using directives

/// <summary>Base class for treasure hunts.</summary>
internal abstract class TreasureHunt : ITreasureHunt
{
    private double _chanceAccumulator = 1d;

    /// <summary>Initializes a new instance of the <see cref="TreasureHunt"/> class.</summary>
    /// <param name="type">Either <see cref="VanillaProfession.Scavenger"/> or <see cref="VanillaProfession.Prospector"/>.</param>
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

    /// <inheritdoc />
    public GameLocation? Location { get; protected set; }

    /// <summary>Gets a random number generator.</summary>
    protected Random Random { get; } = new(Guid.NewGuid().GetHashCode());

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
    [MemberNotNullWhen(true, "Location", "TreasureTile")]
    public virtual bool TryStart(GameLocation location)
    {
        if (this.IsActive)
        {
            return false;
        }

        if (this.Random.NextDouble() > ProfessionsModule.Config.ChanceToStartTreasureHunt * this._chanceAccumulator)
        {
            this._chanceAccumulator *= 1d + Game1.player.DailyLuck;
            return false;
        }

        this.TreasureTile = this.ChooseTreasureTile(location);
        if (this.TreasureTile is null)
        {
            return false;
        }

        this.Location = location;
        this.Elapsed = 0;
        this._chanceAccumulator = 1d;
        Game1.player.Get_IsHuntingTreasure().Value = true;
        HudPointer.Instance.Value.ShouldBob = true;

        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(I18n.TreasureHunt_Broadcast_Started(Game1.player.Name));
        }

        this.OnStarted();
        return true;
    }

    /// <inheritdoc />
    [MemberNotNull("Location", "TreasureTile")]
    public virtual void ForceStart(GameLocation location, Vector2 target)
    {
        if (this.IsActive)
        {
            ThrowHelper.ThrowInvalidOperationException("A Treasure Hunt is already active in this instance.");
        }

        this.TreasureTile = target;
        this.Location = location;
        this.Elapsed = 0;
        this._chanceAccumulator = 1d;
        Game1.player.Get_IsHuntingTreasure().Value = true;
        HudPointer.Instance.Value.ShouldBob = true;

        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(I18n.TreasureHunt_Broadcast_Started(Game1.player.Name));
        }

        this.OnStarted();
    }

    /// <inheritdoc />
    public abstract void Complete();

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
        if (!Game1.game1.ShouldTimePass())
        {
            return;
        }

        if (ticks % 60 == 0 && ++this.Elapsed > this.TimeLimit)
        {
            this.Fail();
        }
    }

    /// <summary>Selects the target treasure tile.</summary>
    /// <param name="location">The game location.</param>
    /// <returns>A <see cref="Vector2"/> tile.</returns>
    protected abstract Vector2? ChooseTreasureTile(GameLocation location);

    /// <summary>Resets treasure tile and releases the treasure hunt update event.</summary>
    /// <param name="success">Whether the treasure was successfully found.</param>
    protected virtual void End(bool success)
    {
        this.TreasureTile = null;
        Game1.player.Get_IsHuntingTreasure().Value = false;
        HudPointer.Instance.Value.ShouldBob = false;
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(success
                ? I18n.TreasureHunt_Broadcast_Ended_Success(Game1.player.Name)
                : I18n.TreasureHunt_Broadcast_Ended_Failure(Game1.player.Name));
        }

        this.OnEnded(success);
        Log.D(success ? "Found the treasure!" : "Failed the treasure hunt.");
    }

    /// <summary>Raised when a Treasure Hunt starts.</summary>
    private void OnStarted()
    {
        Started?.Invoke(this, new TreasureHuntStartedEventArgs(Game1.player, this.Type, this.TreasureTile!.Value));
    }

    /// <summary>Raised when a Treasure Hunt ends.</summary>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    private void OnEnded(bool found)
    {
        Ended?.Invoke(this, new TreasureHuntEndedEventArgs(Game1.player, this.Type, found));
    }
}
