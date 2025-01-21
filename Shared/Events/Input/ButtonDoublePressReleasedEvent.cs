namespace DaLion.Shared.Events;

#region using directives

using DaLion.Shared.Attributes;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ButtonDoublePressReleasedEvent"/> class, used for detecting double-press events.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
/// <param name="parent">The parent <see cref="ButtonDoublePressedEvent"/>.</param>
[ImplicitIgnore]
internal sealed class ButtonDoublePressReleasedEvent(
    EventManager manager,
    ButtonDoublePressedEvent parent)
    : ButtonReleasedEvent(manager)
{
    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        if (parent.KeybindList.GetState() != SButtonState.Released)
        {
            return;
        }

        parent.OnButtonDoublePressed!.Invoke();
        parent.ButtonDoublePressUpdateTickedEvent?.Disable();
        this.Disable();
    }
}
