namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonsChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ButtonsChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ButtonsChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (IsHooked) OnButtonsChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnButtonsChanged" />
    protected abstract void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e);
}