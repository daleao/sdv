namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> is activated.</summary>
internal abstract class UltimateActivatedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> is activated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateActivated(object sender, UltimateActivatedEventArgs e)
    {
        if (enabled.Value) OnUltimateActivatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateActivated" />
    protected abstract void OnUltimateActivatedImpl(object sender, UltimateActivatedEventArgs e);
}