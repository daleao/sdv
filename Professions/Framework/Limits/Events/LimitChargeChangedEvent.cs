namespace DaLion.Professions.Framework.Limits.Events;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> charge increases.</summary>
internal sealed class LimitChargeChangedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitChargeChangedEventArgs> _onChargeChangedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitChargeChangedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitChargeChangedEvent(
        Action<object?, ILimitChargeChangedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onChargeChangedImpl = callback;
        LimitBreak.ChargeChanged += this.OnChargeChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        LimitBreak.ChargeChanged -= this.OnChargeChanged;
    }

    /// <summary>Raised when a player's combat <see cref="ILimitBreak"/> charge value is changed.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeChanged(object? sender, ILimitChargeChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeChangedImpl(sender, e);
        }
    }
}
