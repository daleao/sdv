namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.TerrainFeatureListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class TerrainFeatureListChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.TerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
    {
        if (hooked.Value) OnTerrainFeatureListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnTerrainFeatureListChanged" />
    protected abstract void OnTerrainFeatureListChangedImpl(object sender, TerrainFeatureListChangedEventArgs e);
}