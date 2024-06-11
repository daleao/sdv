namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="MasteryWarningButtonPressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class MasteryWarningButtonPressedEvent(EventManager? manager = null)
    : ButtonPressedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.WarningBox is not null;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        switch (e.Button)
        {
            case SButton.MouseLeft:
                State.WarningBox!.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                break;
            case SButton.MouseRight:
                State.WarningBox!.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                break;
            case SButton.DPadUp:
            case SButton.DPadRight:
            case SButton.DPadDown:
            case SButton.DPadLeft:
            case SButton.ControllerA:
            case SButton.ControllerB:
            case SButton.ControllerX:
            case SButton.ControllerY:
            case SButton.ControllerBack:
            case SButton.ControllerStart:
                State.WarningBox!.receiveGamePadButton((Buttons)e.Button);
                break;
            default:
                State.WarningBox!.receiveKeyPress((Keys)e.Button);
                break;
        }
    }
}
