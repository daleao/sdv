namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ObjectListChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class ObjectListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ObjectListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ObjectListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.ObjectListChanged += this.OnObjectListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.ObjectListChanged -= this.OnObjectListChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnObjectListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnObjectListChanged"/>
    protected abstract void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e);
}
