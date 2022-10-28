namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonPressed"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ButtonPressedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ButtonPressedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.ButtonPressed += this.OnButtonPressed;
    }

    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnButtonPressedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnButtonPressed"/>
    protected abstract void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e);
}
