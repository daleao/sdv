namespace DaLion.Overhaul.Modules.Combat.Events.Input.ButonPressed;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicking;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.Config.ControlsUi.EnableAutoSelection ||
                                      CombatModule.Config.ControlsUi.FaceMouseCursor ||
                                      CombatModule.Config.ControlsUi.SlickMoves;

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

        if (CombatModule.Config.ControlsUi.EnableAutoSelection && ModEntry.State.AreEnemiesAround &&
            player.CurrentTool != CombatModule.State.AutoSelectableRanged &&
            ToolSelector.TryFor(player, out var index))
        {
            player.CurrentToolIndex = index;
        }

        if (player.CurrentTool is not Slingshot)
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (CombatModule.Config.ControlsUi.FaceMouseCursor && !Game1.options.gamepadControls)
        {
            var location = player.currentLocation;
            var isNearActionableTile = false;
            foreach (var tile in player.getTileLocation()
                         .GetEightNeighbors(location.Map.DisplayWidth, location.Map.DisplayHeight))
            {
                if (!location.IsActionableTile(tile, player))
                {
                    continue;
                }

                isNearActionableTile = true;
                break;
            }

            if (!isNearActionableTile)
            {
                player.FaceTowardsTile(Game1.currentCursorTile);
            }
        }

        if (isActionButton && CombatModule.State.SlingshotCooldown > 0)
        {
            return;
        }

        if (!player.isMoving() || !player.running || !CombatModule.Config.ControlsUi.SlickMoves)
        {
            return;
        }

        var directionVector = originalDirection.ToVector();
        if (originalDirection.IsVertical())
        {
            directionVector *= -1f;
        }

        var driftVelocity = directionVector * (1f + (Game1.player.addedSpeed * 0.1f)) * 3f;
        CombatModule.State.DriftVelocity = driftVelocity;
        this.Manager.Enable<SlickMovesUpdateTickingEvent>();
    }
}
