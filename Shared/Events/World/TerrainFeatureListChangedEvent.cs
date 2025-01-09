namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.TerrainFeatureListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class TerrainFeatureListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TerrainFeatureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected TerrainFeatureListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.TerrainFeatureListChanged += this.OnTerrainFeatureListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.TerrainFeatureListChanged -= this.OnTerrainFeatureListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnTerrainFeatureListChanged"/>
    protected abstract void OnTerrainFeatureListChangedImpl(object? sender, TerrainFeatureListChangedEventArgs e);

    /// <inheritdoc cref="IWorldEvents.TerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnTerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnTerrainFeatureListChangedImpl(sender, e);
        }
    }
}
