namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LargeTerrainFeatureListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LargeTerrainFeatureListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.LargeTerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLargeTerrainFeatureListChanged(object sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (hooked.Value) OnLargeTerrainFeatureListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLargeTerrainFeatureListChanged" />
    protected abstract void OnLargeTerrainFeatureListChangedImpl(object sender, LargeTerrainFeatureListChangedEventArgs e);
}