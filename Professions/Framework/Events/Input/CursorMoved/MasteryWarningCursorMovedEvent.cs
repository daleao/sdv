namespace DaLion.Professions.Framework.Events.Input.CursorMoved;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="MasteryWarningCursorMovedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class MasteryWarningCursorMovedEvent(EventManager? manager = null)
    : CursorMovedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.WarningBox is not null;

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        State.WarningBox!.performHoverAction(Game1.getMouseX(true), Game1.getMouseY(true));
    }
}
