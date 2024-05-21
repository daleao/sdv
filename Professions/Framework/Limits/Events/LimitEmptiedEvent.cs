namespace DaLion.Professions.Framework.Limits.Events;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> charge value returns to zero.</summary>
internal sealed class LimitEmptiedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitEmptiedEventArgs> _onEmptiedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitEmptiedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitEmptiedEvent(
        Action<object?, ILimitEmptiedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onEmptiedImpl = callback;
        LimitBreak.Emptied += this.OnEmptied;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        LimitBreak.Emptied -= this.OnEmptied;
    }

    /// <summary>Raised when the local player's <see cref="ILimitBreak"/> charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEmptied(object? sender, ILimitEmptiedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onEmptiedImpl(sender, e);
        }
    }
}
