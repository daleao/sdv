namespace DaLion.Redux.Framework.Arsenal.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ModEntry.Config.Arsenal.FaceMouseCursor || ModEntry.Config.Arsenal.SlickMoves;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!(e.Button.IsActionButton() || e.Button.IsUseToolButton()) || player.UsingTool || player.isRidingHorse())
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (!Game1.options.gamepadControls && ModEntry.Config.Arsenal.FaceMouseCursor)
        {
            // face mouse cursor
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        if (!player.isMoving() || !player.running || !ModEntry.Config.Arsenal.SlickMoves)
        {
            return;
        }

        // do drift
        var driftTrajectory = originalDirection.ToVector() *
                              (1f + (Game1.player.addedSpeed * 0.1f)) * 2f;
        player.setTrajectory(driftTrajectory);
    }
}
