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
internal sealed class WeaponButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.Config.EnableAutoSelection ||
                                      CombatModule.Config.FaceMouseCursor ||
                                      CombatModule.Config.SlickMoves;

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

        if (CombatModule.Config.EnableAutoSelection && GlobalState.AreEnemiesAround &&
            player.CurrentTool != CombatModule.State.AutoSelectableMelee &&
            WeaponSelector.TryFor(player, out var index))
        {
            Game1.player.CurrentToolIndex = index;
        }

        if (player.CurrentTool is not MeleeWeapon weapon)
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (CombatModule.Config.FaceMouseCursor && !Game1.options.gamepadControls &&
            !weapon.isScythe())
        {
            var location = player.currentLocation;
            var isNearActionableTile = false;
            foreach (var tile in player.getTileLocation()
                         .GetEightNeighbors(location.Map.DisplayWidth, location.Map.DisplayHeight))
            {
                isNearActionableTile = location.IsActionableTile(tile, player);
                if (isNearActionableTile)
                {
                    break;
                }
            }

            if (!isNearActionableTile)
            {
                player.FaceTowardsTile(Game1.currentCursorTile);
            }
        }

        if (isActionButton)
        {
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
        }
        else if (isUseToolButton && CombatModule.State.ComboCooldown > 0)
        {
            return;
        }

        if (!player.isMoving() || !player.running || !CombatModule.Config.SlickMoves)
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
