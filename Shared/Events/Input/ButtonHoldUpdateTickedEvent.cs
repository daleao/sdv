namespace DaLion.Shared.Events;

#region using directives

using DaLion.Shared.Attributes;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonHoldUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
/// <param name="parent">The parent <see cref="ButtonHeldEvent"/>.</param>
/// <param name="holdInterval">The interval required to activate a hold, in milliseconds.</param>
[ImplicitIgnore]
internal sealed class ButtonHoldUpdateTickedEvent(
    EventManager manager,
    ButtonHeldEvent parent,
    int holdInterval)
    : UpdateTickedEvent(manager)
{
    private int _beganHoldGameTime;

    protected override void OnEnabled()
    {
        this._beganHoldGameTime = Game1.currentGameTime.TotalGameTime.Milliseconds;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds < this._beganHoldGameTime + holdInterval)
        {
            return;
        }

        parent.OnButtonHeld!.Invoke();
        this.Disable();
    }
}
