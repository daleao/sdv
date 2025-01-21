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
    private double _lastPressedGameTime;

    /// <summary>Gets the <see cref="StardewModdingAPI.Utilities.KeybindList"/> monitored by this event.</summary>
    public abstract KeybindList KeybindList { get; }

    /// <summary>Gets the <see cref="Action"/> invoked if the <seealso cref="KeybindList"/> is immediately release after two consecutive presses.</summary>
    public Action? OnButtonDoublePressed { get; protected init; }

    /// <summary>Gets the <see cref="Action"/> invoked if the <seealso cref="KeybindList"/> is held for <seealso cref="HoldInterval"/> after two consecutive presses.</summary>
    public Action? OnButtonDoublePressedAndHeld { get; protected init; }

    /// <summary>Gets the event used to detect a simple double press.</summary>
    internal ButtonDoublePressReleasedEvent? ButtonDoublePressReleasedEvent { get; private set; }

    /// <summary>Gets the event used to detect a double-press-and-hold.</summary>
    internal ButtonDoublePressUpdateTickedEvent? ButtonDoublePressUpdateTickedEvent { get; private set; }

    /// <summary>Gets the maximum accepted interval between presses, in milliseconds.</summary>
    protected virtual int DoublePressInterval { get; } = 200;

    /// <summary>Gets the interval required for a double-press-and-hold, in milliseconds.</summary>
    protected virtual int HoldInterval { get; } = 250;

    /// <inheritdoc cref="OnButtonDoublePressed"/>
    public virtual void OnButtonDoublePressedImpl()
    {
    }

    /// <inheritdoc cref="OnButtonDoublePressedAndHeld"/>
    public virtual void OnButtonDoublePressedAndHeldImpl()
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (!this.KeybindList.IsBound)
        {
            this.Disable();
        }
    }

    /// <inheritdoc />
    protected sealed override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!this.KeybindList.JustPressed())
        {
            return;
        }

        if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds <= this._lastPressedGameTime + this.DoublePressInterval)
        {
            if (this.OnButtonDoublePressed is not null)
            {
                (this.ButtonDoublePressReleasedEvent ??=
                    new ButtonDoublePressReleasedEvent(this.Manager, this)).Enable();
            }

            if (this.OnButtonDoublePressedAndHeld is not null)
            {
                (this.ButtonDoublePressUpdateTickedEvent ??=
                    new ButtonDoublePressUpdateTickedEvent(this.Manager, this, this.HoldInterval)).Enable();
            }
        }
        else
        {
            this._lastPressedGameTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}
