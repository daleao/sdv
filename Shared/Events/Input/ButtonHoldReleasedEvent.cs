﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonHoldReleasedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
/// <param name="keybinds">The <see cref="KeybindList"/> monitored by this event.</param>
/// <param name="holdInterval">The interval required to activate a double-press-and-hold, in milliseconds.</param>
/// <param name="onHeld">The <see cref="Action"/> to be invoked on a simple double-press event.</param>
internal sealed class ButtonHoldReleasedEvent(
    EventManager manager,
    KeybindList keybinds,
    int holdInterval,
    Action onHeld)
    : ButtonReleasedEvent(manager)
{
    private int _lastPressedGameTime;

    protected override void OnEnabled()
    {
        this._lastPressedGameTime = Game1.currentGameTime.TotalGameTime.Milliseconds;
    }

    /// <summary>Gets the <see cref="KeybindList"/> monitored by this event.</summary>
    private KeybindList Keybinds { get; } = keybinds;

    /// <summary>Gets the interval required for a double-press-and-hold, in milliseconds.</summary>
    private int HoldInterval { get; } = holdInterval;

    /// <summary>Gets the <see cref="Action"/> to be invoked on a simple double-press event.</summary>
    private Action OnHeld { get; } = onHeld;

    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        if (!this.Keybinds.JustPressed())
        {
            return;
        }

        if (Game1.currentGameTime.TotalGameTime.Milliseconds >= this._lastPressedGameTime + this.HoldInterval)
        {
            this.OnHeld.Invoke();
        }

        this.Disable();
    }
}