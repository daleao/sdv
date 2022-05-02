namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> charge value increases while it was previously empty.</summary>
internal abstract class UltimateChargeInitiatedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> charge value increases while it was previously empty.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateChargeInitiated(object sender, UltimateChargeInitiatedEventArgs e)
    {
        if (enabled.Value) OnUltimateChargeInitiatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateChargeInitiated" />
    protected abstract void OnUltimateChargeInitiatedImpl(object sender, UltimateChargeInitiatedEventArgs e);
}