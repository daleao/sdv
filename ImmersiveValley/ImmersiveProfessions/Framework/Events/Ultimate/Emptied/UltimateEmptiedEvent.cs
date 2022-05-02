namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>The event raised when <see cref="Ultimate"/> charge value returns to zero.</summary>
internal abstract class UltimateEmptiedEvent : BaseEvent
{
    /// <summary>Raised when <see cref="Ultimate"/> charge value returns to zero.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUltimateEmptied(object sender, UltimateEmptiedEventArgs e)
    {
        if (enabled.Value) OnUltimateEmptiedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUltimateEmptied" />
    protected abstract void OnUltimateEmptiedImpl(object sender, UltimateEmptiedEventArgs e);
}