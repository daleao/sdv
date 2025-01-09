namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LargeTerrainFeatureListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class LargeTerrainFeatureListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LargeTerrainFeatureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LargeTerrainFeatureListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.LargeTerrainFeatureListChanged += this.OnLargeTerrainFeatureListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.LargeTerrainFeatureListChanged -= this.OnLargeTerrainFeatureListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnLargeTerrainFeatureListChanged"/>
    protected abstract void OnLargeTerrainFeatureListChangedImpl(
        object? sender, LargeTerrainFeatureListChangedEventArgs e);

    /// <inheritdoc cref="IWorldEvents.LargeTerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLargeTerrainFeatureListChangedImpl(sender, e);
        }
    }
}
