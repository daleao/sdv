namespace DaLion.Professions.Framework.Events.Limit.ChargeIncreased;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="ILimitBreak"/> charge increases.</summary>
internal sealed class LimitChargeIncreasedEvent : ManagedEvent
{
    private readonly Action<object?, ILimitChargeIncreasedEventArgs> _onChargeIncreasedImpl;

    /// <summary>Initializes a new instance of the <see cref="LimitChargeIncreasedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitChargeIncreasedEvent(
        Action<object?, ILimitChargeIncreasedEventArgs> callback,
        EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this._onChargeIncreasedImpl = callback;
        Limits.LimitBreak.ChargeIncreased += this.OnChargeIncreased;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Limits.LimitBreak.ChargeIncreased -= this.OnChargeIncreased;
    }

    /// <summary>Raised when a player's combat <see cref="ILimitBreak"/> charge increases.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeIncreased(object? sender, ILimitChargeIncreasedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeIncreasedImpl(sender, e);
        }
    }
}
