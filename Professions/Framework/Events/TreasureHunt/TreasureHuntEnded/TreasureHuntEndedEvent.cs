namespace DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntEnded;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"/> ends.</summary>
internal sealed class TreasureHuntEndedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntEndedEventArgs> _onEndedImpl;

    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TreasureHuntEndedEvent(
        Action<object?, ITreasureHuntEndedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onEndedImpl = callback;
        TreasureHunts.TreasureHunt.Ended += this.OnEnded;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        TreasureHunts.TreasureHunt.Ended -= this.OnEnded;
    }

    /// <summary>Raised when a <see cref="TreasureHunts.ITreasureHunt"/> ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEnded(object? sender, ITreasureHuntEndedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onEndedImpl(sender, e);
        }
    }
}
