namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntTriggerTimeChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntTriggerTimeChangedEvent(EventManager? manager = null)
    : TimeChangedEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        var player = Game1.player;
        var location = Game1.currentLocation;
        if (location.IsRainingHere())
        {
            return;
        }

        var targetPosition = player.GetBoundingBox();
        switch (player.FacingDirection)
        {
            case Game1.up:
                targetPosition.Y -= (int)Math.Ceiling(player.getMovementSpeed()) * 3;
                targetPosition.Inflate(Game1.tileSize, 0);
                break;
            case Game1.right:
                targetPosition.X += (int)Math.Ceiling(player.getMovementSpeed()) * 3;
                targetPosition.Inflate(0, Game1.tileSize);
                break;
            case Game1.down:
                targetPosition.Y += (int)Math.Ceiling(player.getMovementSpeed()) * 3;
                targetPosition.Inflate(Game1.tileSize, 0);
                break;
            case Game1.left:
                targetPosition.X -= (int)Math.Ceiling(player.getMovementSpeed()) * 3;
                targetPosition.Inflate(0, Game1.tileSize);
                break;
        }

        if (location.isCollidingWithWarp(targetPosition, player) is not null)
        {
            return;
        }

        if (State.ScavengerHunt!.TryStart(location))
        {
            this.Disable();
        }
    }
}
