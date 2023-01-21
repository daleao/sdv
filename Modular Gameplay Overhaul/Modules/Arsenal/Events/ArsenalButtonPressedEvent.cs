namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events.Weapons;
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
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null)
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not { } tool || player.UsingTool || player.isRidingHorse() || !player.CanMove)
        {
            return;
        }

        var isActionButton = e.Button.IsActionButton();
        var isUseToolButton = e.Button.IsUseToolButton();
        if (!(isActionButton || isUseToolButton))
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (ArsenalModule.Config.FaceMouseCursor && !Game1.options.gamepadControls && tool is MeleeWeapon w && !w.isScythe())
        {
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        if (ArsenalModule.Config.EnableAutoSelection && ArsenalModule.State.SelectableArsenal is not null &&
            ArsenalModule.State.SelectableArsenal != tool &&
            ToolsModule.State.SelectableToolByType.ContainsKey(tool.GetType()) &&
            ArsenalSelector.TryFor(player, out var index))
        {
            Game1.player.CurrentToolIndex = index;
            tool = Game1.player.CurrentTool;
        }

        if (tool is not (MeleeWeapon or Slingshot))
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

        if (!isUseToolButton || !player.isMoving() || !player.running || !ArsenalModule.Config.SlickMoves)
        {
            return;
        }

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
