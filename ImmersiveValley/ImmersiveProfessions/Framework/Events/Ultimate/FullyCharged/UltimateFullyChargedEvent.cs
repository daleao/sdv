namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> charge value reaches max value.</summary>
internal abstract class UltimateFullyChargedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> charge value reaches max value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateFullyCharged(object sender, UltimateFullyChargedEventArgs e)
    {
        if (enabled.Value) OnUltimateFullyChargedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateFullyCharged" />
    protected abstract void OnUltimateFullyChargedImpl(object sender, UltimateFullyChargedEventArgs e);
}