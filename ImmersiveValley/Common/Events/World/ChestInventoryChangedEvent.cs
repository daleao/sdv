namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ChestInventoryChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ChestInventoryChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ChestInventoryChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.ChestInventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
    {
        if (IsEnabled) OnChestInventoryChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnChestInventoryChanged" />
    protected abstract void OnChestInventoryChangedImpl(object? sender, ChestInventoryChangedEventArgs e);
}