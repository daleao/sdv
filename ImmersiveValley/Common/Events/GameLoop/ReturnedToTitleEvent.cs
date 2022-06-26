namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.ReturnedToTitle"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ReturnedToTitleEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ReturnedToTitleEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        if (IsHooked) OnReturnedToTitleImpl(sender, e);
    }

    /// <inheritdoc cref="OnReturnedToTitle" />
    protected abstract void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e);
}