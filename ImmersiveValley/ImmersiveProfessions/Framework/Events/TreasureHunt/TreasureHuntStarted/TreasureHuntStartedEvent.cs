namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using DaLion.Common.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"/> starts.</summary>
internal sealed class TreasureHuntStartedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntStartedEventArgs> _onStartedImpl;

    /// <summary>Initializes a new instance of the <see cref="TreasureHuntStartedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysEnabled">Whether the event should be allowed to override the <c>enabled</c> flag.</param>
    internal TreasureHuntStartedEvent(
        Action<object?, ITreasureHuntStartedEventArgs> callback, bool alwaysEnabled = false)
        : base(ModEntry.Events)
    {
        this._onStartedImpl = callback;
        this.AlwaysEnabled = alwaysEnabled;
    }

    /// <summary>Raised when a <see cref="TreasureHunts.ITreasureHunt"/> starts.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnStarted(object? sender, ITreasureHuntStartedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onStartedImpl(sender, e);
        }
    }
}
