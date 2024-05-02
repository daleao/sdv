namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.MenuChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class MenuChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="MenuChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected MenuChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.MenuChanged += this.OnMenuChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.MenuChanged -= this.OnMenuChanged;
    }

    /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnMenuChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnMenuChanged"/>
    protected abstract void OnMenuChangedImpl(object? sender, MenuChangedEventArgs e);
}
