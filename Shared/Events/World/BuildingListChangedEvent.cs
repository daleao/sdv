namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.BuildingListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class BuildingListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BuildingListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected BuildingListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.BuildingListChanged += this.OnBuildingListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.BuildingListChanged -= this.OnBuildingListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IWorldEvents.BuildingListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnBuildingListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnBuildingListChanged"/>
    protected abstract void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e);
}
