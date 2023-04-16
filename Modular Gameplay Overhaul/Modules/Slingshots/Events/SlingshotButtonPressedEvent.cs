namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
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
    public override bool IsEnabled => SlingshotsModule.Config.SlickMoves;

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

        if (SlingshotsModule.Config.EnableAutoSelection && Globals.AreEnemiesAround &&
            player.CurrentTool != SlingshotsModule.State.AutoSelectableSlingshot &&
            SlingshotSelector.TryFor(player, out var index))
        {
            player.CurrentToolIndex = index;
        }

        if (player.CurrentTool is not Slingshot)
        {
            return;
        }

        if (isActionButton && SlingshotsModule.State.SlingshotCooldown > 0)
        {
            return;
        }

        if (!player.isMoving() || !player.running || !SlingshotsModule.Config.SlickMoves)
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        var directionVector = originalDirection.ToVector();
        if (originalDirection.IsVertical())
        {
            directionVector *= -1f;
        }

        var driftVelocity = directionVector * (1f + (Game1.player.addedSpeed * 0.1f)) * 3f;
        SlingshotsModule.State.DriftVelocity = driftVelocity;
        this.Manager.Enable<SlickMovesUpdateTickingEvent>();
    }
}
