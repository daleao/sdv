﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.MouseWheelScrolled"/> allowing dynamic enabling / disabling.</summary>
public abstract class MouseWheelScrolledEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="MouseWheelScrolledEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected MouseWheelScrolledEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnMouseWheelScrolled"/>
    protected abstract void OnMouseWheelScrolledImpl(object? sender, MouseWheelScrolledEventArgs e);

    /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnMouseWheelScrolledImpl(sender, e);
        }
    }
}
