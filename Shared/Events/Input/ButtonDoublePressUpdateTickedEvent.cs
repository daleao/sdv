namespace DaLion.Shared.Events;

#region using directives

using DaLion.Shared.Attributes;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonDoublePressReleasedEvent"/> class, used for detecting double-press-and-hold events.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
/// <param name="parent">The parent <see cref="ButtonDoublePressedEvent"/>.</param>
/// <param name="holdInterval">Gets the interval required for a double-press-and-hold, in milliseconds.</param>
[ImplicitIgnore]
internal sealed class ButtonDoublePressUpdateTickedEvent(
    EventManager manager,
    ButtonDoublePressedEvent parent,
    int holdInterval)
    : UpdateTickedEvent(manager)
{
    private double _beganHoldGameTime;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._beganHoldGameTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds < this._beganHoldGameTime + holdInterval)
        {
            return;
        }

        parent.OnButtonDoublePressedAndHeld!.Invoke();
        parent.ButtonDoublePressReleasedEvent?.Disable();
        this.Disable();
    }
}
