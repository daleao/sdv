namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.InventoryChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class InventoryChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="InventoryChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected InventoryChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Player.InventoryChanged += this.OnInventoryChanged;
    }

    /// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnInventoryChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnInventoryChanged"/>
    protected abstract void OnInventoryChangedImpl(object? sender, InventoryChangedEventArgs e);
}
