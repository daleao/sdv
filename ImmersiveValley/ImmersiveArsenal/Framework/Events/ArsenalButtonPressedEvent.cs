namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Enums;
using Common.Events;
using Common.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalButtonPressedEvent(EventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!Context.IsPlayerFree || player.isRidingHorse() ||
            (!e.Button.IsActionButton() && !e.Button.IsUseToolButton()) || player.UsingTool || !player.CanMove ||
            player.CurrentTool is not (MeleeWeapon or Slingshot) || Game1.options.gamepadControls)
            return;

        var direction = player.FacingDirection;
        player.FaceTowardsTile(Game1.currentCursorTile);
        if (!player.isMoving() || !player.running) return;

        var driftTrajector = Common.Utility.VectorFromFacingDirection((FacingDirection)direction) *
                             (1f + Game1.player.addedSpeed * 0.1f) * 2f;
        player.setTrajectory(driftTrajector);
    }
}