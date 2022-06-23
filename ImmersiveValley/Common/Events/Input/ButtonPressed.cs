namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonPressed"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class ButtonPressedEvent : BaseEvent
{
    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (hooked.Value) OnButtonPressedImpl(sender, e);
    }

    /// <inheritdoc cref="OnButtonPressed" />
    protected abstract void OnButtonPressedImpl(object sender, ButtonPressedEventArgs e);
}