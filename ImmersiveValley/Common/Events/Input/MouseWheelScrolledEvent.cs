namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.MouseWheelScrolled"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class MouseWheelScrolledEvent : BaseEvent
{
    /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
    {
        if (hooked.Value) OnMouseWheelScrolledImpl(sender, e);
    }

    /// <inheritdoc cref="OnMouseWheelScrolled" />
    protected abstract void OnMouseWheelScrolledImpl(object sender, MouseWheelScrolledEventArgs e);
}