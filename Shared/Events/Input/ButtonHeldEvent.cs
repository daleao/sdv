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
    private ButtonHoldReleasedEvent? _buttonHoldReleasedEvent = null;

    /// <summary>Gets the <see cref="KeybindList"/> monitored by this event.</summary>
    protected abstract KeybindList Keybinds { get; }

    /// <summary>Gets the interval required for a double-press-and-hold, in milliseconds.</summary>
    protected virtual int HoldInterval { get; } = 250;

    /// <summary>Invoked when <see cref="Keybinds"/> is held for a short interval.</summary>
    protected virtual void OnButtonHeldImpl()
    {
    }

    /// <inheritdoc />
    protected sealed override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!this.Keybinds.JustPressed())
        {
            return;
        }

        (this._buttonHoldReleasedEvent ??=
            new ButtonHoldReleasedEvent(
                this.Manager,
                this.Keybinds,
                this.HoldInterval,
                this.OnButtonHeldImpl)).Enable();
    }
}
