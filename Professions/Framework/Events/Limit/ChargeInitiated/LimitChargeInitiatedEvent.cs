namespace DaLion.Professions.Framework.Events.Limit.ChargeInitiated;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> is gains any charge while it was previously empty.</summary>
internal sealed class LimitChargeInitiatedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitChargeInitiatedEventArgs> _onChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitChargeInitiatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitChargeInitiatedEvent(
        Action<object?, ILimitChargeInitiatedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onChargeInitiatedImpl = callback;
        Limits.LimitBreak.ChargeInitiated += this.OnChargeInitiated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Limits.LimitBreak.ChargeInitiated -= this.OnChargeInitiated;
    }

    /// <summary>
    ///     Raised when a player's combat <see cref="ILimitBreak"/> gains any charge while it was previously
    ///     empty.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeInitiated(object? sender, ILimitChargeInitiatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeInitiatedImpl(sender, e);
        }
    }
}
