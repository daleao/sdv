namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonDoublePressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
public abstract class ButtonDoublePressedEvent(EventManager manager)
    : ButtonPressedEvent(manager)
{
    private int _lastPressedGameTime;
    private ButtonDoublePressReleasedEvent? _buttonDoublePressReleasedEvent = null;

    /// <summary>Gets the <see cref="KeybindList"/> monitored by this event.</summary>
    protected abstract KeybindList Keybinds { get; }

    /// <summary>Gets the maximum accepted interval between presses, in milliseconds.</summary>
    protected virtual int DoublePressInterval { get; } = 200;

    /// <summary>Gets the interval required for a double-press-and-hold, in milliseconds.</summary>
    protected virtual int HoldInterval { get; } = 250;

    /// <summary>Invoked when <see cref="Keybinds"/> is pressed twice and immediately released.</summary>
    protected virtual void OnButtonDoublePressedImpl()
    {
    }

    /// <summary>Invoked when <see cref="Keybinds"/> is pressed twice and then held for a short interval.</summary>
    protected virtual void OnButtonDoublePressedAndHeldImpl()
    {
    }

    /// <inheritdoc />
    protected sealed override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!this.Keybinds.JustPressed())
        {
            return;
        }

        this._buttonDoublePressReleasedEvent ??=
            new ButtonDoublePressReleasedEvent(
                this.Manager,
                this.Keybinds,
                this.HoldInterval,
                this.OnButtonDoublePressedImpl,
                this.OnButtonDoublePressedAndHeldImpl);
        if (Game1.currentGameTime.TotalGameTime.Milliseconds <= this._lastPressedGameTime + this.DoublePressInterval)
        {
            this._buttonDoublePressReleasedEvent.Enable();
        }
        else
        {
            this._lastPressedGameTime = Game1.currentGameTime.TotalGameTime.Milliseconds;
        }
    }
}
