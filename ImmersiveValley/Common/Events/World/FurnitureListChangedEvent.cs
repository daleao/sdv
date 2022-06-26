namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.FurnitureListChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class FurnitureListChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected FurnitureListChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.FurnitureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
    {
        if (IsHooked) OnFurnitureListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnFurnitureListChanged" />
    protected abstract void OnFurnitureListChangedImpl(object? sender, FurnitureListChangedEventArgs e);
}