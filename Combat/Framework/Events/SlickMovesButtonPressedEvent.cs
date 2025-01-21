namespace DaLion.Combat.Framework.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlickMovesButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlickMovesButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlickMovesButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Config.FaceMouseCursor != CombatConfig.FaceCursorCondition.Never || Config.SlickMoves;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || Game1.isFestival())
        {
            return;
        }

        var player = Game1.player;
        if (player.UsingTool || player.isRidingHorse() || !player.CanMove)
        {
            return;
        }

        var isActionButton = e.Button.IsActionButton();
        var isUseToolButton = e.Button.IsUseToolButton();
        if (!isActionButton && !isUseToolButton)
        {
            return;
        }

        var isHoldingWeapon = player.CurrentTool is MeleeWeapon weapon && !weapon.isScythe();
        var originalDirection = (Direction)player.FacingDirection;
        if (!Game1.options.gamepadControls && Config.FaceMouseCursor != CombatConfig.FaceCursorCondition.Never)
        {
            var location = player.currentLocation;
            var isNearActionableTile = player.Tile
                .GetEightNeighbors(location.Map.DisplayWidth, location.Map.DisplayHeight)
                .Any(tile => location.IsActionableTile(tile, player));
            if (!isNearActionableTile && (Config.FaceMouseCursor == CombatConfig.FaceCursorCondition.Always || isHoldingWeapon))
            {
                player.FaceTowardsTile(Game1.currentCursorTile);
            }
        }

        if (isUseToolButton && isHoldingWeapon && State.ComboCooldown > 0)
        {
            return;
        }

        if (!player.isMoving() || !player.running || !isHoldingWeapon || !Config.SlickMoves)
        {
            return;
        }

        var directionVector = originalDirection.ToVector();
        if (originalDirection.IsVertical())
        {
            directionVector *= -1f;
        }

        var driftVelocity = directionVector * (1f + (player.addedSpeed / 10f)) * 4f;
        State.DriftVelocity = driftVelocity;
        this.Manager.Enable<SlickMovesUpdateTickingEvent>();
    }
}
