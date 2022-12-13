namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"/> charge increases.</summary>
internal sealed class UltimateChargeIncreasedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeIncreasedEventArgs> _onChargeIncreasedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateChargeIncreasedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeIncreasedEvent(
        Action<object?, IUltimateChargeIncreasedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onChargeIncreasedImpl = callback;
        Ultimates.Ultimate.ChargeIncreased += this.OnChargeIncreased;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.ChargeIncreased -= this.OnChargeIncreased;
    }

    /// <summary>Raised when a player's combat <see cref="Ultimates.IUltimate"/> charge increases.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeIncreased(object? sender, IUltimateChargeIncreasedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeIncreasedImpl(sender, e);
        }
    }
}
