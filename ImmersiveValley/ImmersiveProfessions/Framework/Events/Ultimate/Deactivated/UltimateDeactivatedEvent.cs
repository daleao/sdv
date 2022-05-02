namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> is deactivated.</summary>
internal abstract class UltimateDeactivatedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> is deactivated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateDeactivated(object sender, UltimateDeactivatedEventArgs e)
    {
        if (enabled.Value) OnUltimateDeactivatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateDeactivated" />
    protected abstract void OnUltimateDeactivatedImpl(object sender, UltimateDeactivatedEventArgs e);
}