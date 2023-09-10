namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate.ChargeInitiated;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>
///     A dynamic event raised when a
///     <see cref="Ultimates.IUltimate"/> is gains any charge while it was previously empty.
/// </summary>
internal sealed class UltimateChargeInitiatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeInitiatedEventArgs> _onChargeInitiatedImpl;

    /// <summary>Initializes a new instance of the <see cref="UltimateChargeInitiatedEvent"/> class.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeInitiatedEvent(
        Action<object?, IUltimateChargeInitiatedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        this._onChargeInitiatedImpl = callback;
        Ultimates.Ultimate.ChargeInitiated += this.OnChargeInitiated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        Ultimates.Ultimate.ChargeInitiated -= this.OnChargeInitiated;
    }

    /// <summary>
    ///     Raised when a player's combat <see cref="Ultimates.IUltimate"/> gains any charge while it was previously
    ///     empty.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeInitiated(object? sender, IUltimateChargeInitiatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this._onChargeInitiatedImpl(sender, e);
        }
    }
}
