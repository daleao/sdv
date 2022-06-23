namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IPlayerEvents.InventoryChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class InventoryChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IPlayerEvents.InventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
    {
        if (hooked.Value) OnInventoryChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnInventoryChanged" />
    protected abstract void OnInventoryChangedImpl(object sender, InventoryChangedEventArgs e);
}