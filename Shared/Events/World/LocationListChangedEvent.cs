namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LocationListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class LocationListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LocationListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LocationListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.LocationListChanged += this.OnLocationListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.LocationListChanged -= this.OnLocationListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnLocationListChanged"/>
    protected abstract void OnLocationListChangedImpl(object? sender, LocationListChangedEventArgs e);

    /// <inheritdoc cref="IWorldEvents.LocationListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLocationListChangedImpl(sender, e);
        }
    }
}
