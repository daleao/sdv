namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Enums;
using DaLion.Common.Events;
using DaLion.Common.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DriftButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DriftButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DriftButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!(e.Button.IsActionButton() || e.Button.IsUseToolButton()) || player.CurrentTool is not MeleeWeapon ||
            player.UsingTool || player.isRidingHorse())
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (!Game1.options.gamepadControls && ModEntry.Config.FaceMouseCursor)
        {
            // face mouse cursor
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        if (!player.isMoving() || !player.running || !ModEntry.Config.SlickMoves)
        {
            return;
        }

        // do drift
        var driftTrajectory = originalDirection.ToVector() *
                              (1f + (Game1.player.addedSpeed * 0.1f)) * 2f;
        player.setTrajectory(driftTrajectory);
    }
}
