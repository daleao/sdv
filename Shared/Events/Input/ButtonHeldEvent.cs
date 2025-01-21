namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonHeldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
public abstract class ButtonHeldEvent(EventManager manager)
    : ButtonPressedEvent(manager)
{
    /// <summary>Gets the <see cref="StardewModdingAPI.Utilities.KeybindList"/> monitored by this event.</summary>
    public abstract KeybindList KeybindList { get; }

    /// <summary>Gets the <see cref="Action"/> invoked if the <seealso cref="KeybindList"/> is held for <seealso cref="HoldInterval"/> after two consecutive presses.</summary>
    public Action? OnButtonHeld { get; protected init; }

    /// <summary>Gets the event used to detect a hold.</summary>
    internal ButtonHoldUpdateTickedEvent? ButtonHoldUpdateTickedEvent { get; private set; }

    /// <summary>Gets the interval required for a double-press-and-hold, in milliseconds.</summary>
    protected virtual int HoldInterval { get; } = 250;

    /// <inheritdoc cref="OnButtonHeld"/>
    protected virtual void OnButtonHeldImpl()
    {
    }

    /// <inheritdoc />
    protected sealed override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (this.KeybindList.JustPressed())
        {
            (this.ButtonHoldUpdateTickedEvent ??=
                new ButtonHoldUpdateTickedEvent(this.Manager, this, this.HoldInterval)).Enable();
        }
    }
}
