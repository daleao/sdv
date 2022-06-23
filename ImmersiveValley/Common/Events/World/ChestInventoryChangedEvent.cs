namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ChestInventoryChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ChestInventoryChangedEvent : BaseEvent
{
    /// <inheritdoc cref="IWorldEvents.ChestInventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChestInventoryChanged(object sender, ChestInventoryChangedEventArgs e)
    {
        if (hooked.Value) OnChestInventoryChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnChestInventoryChanged" />
    protected abstract void OnChestInventoryChangedImpl(object sender, ChestInventoryChangedEventArgs e);
}