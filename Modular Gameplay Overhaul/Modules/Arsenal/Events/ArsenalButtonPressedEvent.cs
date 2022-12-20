namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

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
    public override bool IsEnabled => ArsenalModule.Config.FaceMouseCursor || ArsenalModule.Config.SlickMoves;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var isActionButton = e.Button.IsActionButton();
        var isUseToolButton = e.Button.IsUseToolButton();
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !(isActionButton || isUseToolButton))
        {
            return;
        }

        var player = Game1.player;
        var tool = player.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot) || player.UsingTool || player.isRidingHorse() || !player.CanMove)
        {
            return;
        }

        if (isActionButton)
        {
            switch (tool)
            {
                case MeleeWeapon weapon:
                    if (weapon.isScythe())
                    {
                        return;
                    }

                    switch (weapon.type.Value)
                    {
                        case MeleeWeapon.stabbingSword when MeleeWeapon.attackSwordCooldown > 0:
                        case MeleeWeapon.defenseSword when MeleeWeapon.defenseCooldown > 0:
                        case MeleeWeapon.dagger when MeleeWeapon.daggerCooldown > 0:
                        case MeleeWeapon.club when MeleeWeapon.clubCooldown > 0:
                            return;
                    }

                    break;
                case Slingshot when ArsenalModule.State.SlingshotCooldown > 0:
                    return;
            }
        }
        else if (isUseToolButton && tool is MeleeWeapon && ArsenalModule.State.ComboCooldown > 0)
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (!Game1.options.gamepadControls && ArsenalModule.Config.FaceMouseCursor)
        {
            // face mouse cursor
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        if (!isUseToolButton || !player.isMoving() || !player.running || !ArsenalModule.Config.SlickMoves)
        {
            return;
        }

        // do drift
        var directionVector = originalDirection.ToVector();
        if (originalDirection.IsVertical())
        {
            directionVector *= -1f;
        }

        var driftVelocity = directionVector * (1f + (Game1.player.addedSpeed * 0.1f)) * 3f;
        ArsenalModule.State.DriftVelocity = driftVelocity;
        this.Manager.Enable<SlickMovesUpdateTickingEvent>();
    }
}
