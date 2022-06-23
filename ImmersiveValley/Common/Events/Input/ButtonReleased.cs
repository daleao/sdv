namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonReleased"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ButtonReleasedEvent : BaseEvent
{
    /// <inheritdoc cref="IInputEvents.ButtonReleased"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
    {
        if (hooked.Value) OnButtonReleasedImpl(sender, e);
    }

    /// <inheritdoc cref="OnButtonReleased" />
    protected abstract void OnButtonReleasedImpl(object sender, ButtonReleasedEventArgs e);
}