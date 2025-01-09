﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonsChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class ButtonsChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ButtonsChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.ButtonsChanged += this.OnButtonsChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Input.ButtonsChanged -= this.OnButtonsChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnButtonsChanged"/>
    protected abstract void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e);

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnButtonsChangedImpl(sender, e);
        }
    }
}
