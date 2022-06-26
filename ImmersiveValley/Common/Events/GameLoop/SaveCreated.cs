namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveCreated"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SaveCreatedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SaveCreatedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.SaveCreated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
    {
        if (IsHooked) OnSaveCreatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveCreated" />
    protected abstract void OnSaveCreatedImpl(object? sender, SaveCreatedEventArgs e);
}