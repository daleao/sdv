namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonReleased"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ButtonReleasedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ButtonReleasedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IInputEvents.ButtonReleased"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (IsEnabled) OnButtonReleasedImpl(sender, e);
    }

    /// <inheritdoc cref="OnButtonReleased" />
    protected abstract void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e);
}