namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.InventoryChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class InventoryChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected InventoryChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (IsHooked) OnInventoryChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnInventoryChanged" />
    protected abstract void OnInventoryChangedImpl(object? sender, InventoryChangedEventArgs e);
}