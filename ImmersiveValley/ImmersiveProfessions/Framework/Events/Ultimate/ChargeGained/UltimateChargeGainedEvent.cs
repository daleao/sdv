namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> charge value increases.</summary>
internal abstract class UltimateChargeGainedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> charge value increases.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateChargeGained(object sender, UltimateChargeGainedEventArgs e)
    {
        if (enabled.Value) OnUltimateChargeGainedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateChargeGained" />
    protected abstract void OnUltimateChargeGainedImpl(object sender, UltimateChargeGainedEventArgs e);
}