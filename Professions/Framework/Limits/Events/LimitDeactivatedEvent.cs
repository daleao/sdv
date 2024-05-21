namespace DaLion.Professions.Framework.Limits.Events;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> ends.</summary>
internal sealed class LimitDeactivatedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitDeactivatedEventArgs> _onDeactivatedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitDeactivatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitDeactivatedEvent(
        Action<object?, ILimitDeactivatedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onDeactivatedImpl = callback;
        LimitBreak.Deactivated += this.OnDeactivated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        LimitBreak.Deactivated -= this.OnDeactivated;
    }

    /// <summary>Raised when a player's combat <see cref="ILimitBreak"/> ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDeactivated(object? sender, ILimitDeactivatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onDeactivatedImpl(sender, e);
        }
    }
}
