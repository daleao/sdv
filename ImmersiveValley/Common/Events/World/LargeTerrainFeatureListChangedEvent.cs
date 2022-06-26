namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LargeTerrainFeatureListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LargeTerrainFeatureListChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LargeTerrainFeatureListChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.LargeTerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (Hooked.Value) OnLargeTerrainFeatureListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLargeTerrainFeatureListChanged" />
    protected abstract void OnLargeTerrainFeatureListChangedImpl(object? sender, LargeTerrainFeatureListChangedEventArgs e);
}