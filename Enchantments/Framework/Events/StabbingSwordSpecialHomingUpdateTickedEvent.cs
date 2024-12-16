namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialHomingUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StabbingSwordSpecialHomingUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbingSwordSpecialHomingUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var user = Game1.player;
        if (user.CurrentTool is not MeleeWeapon { isOnSpecial: true })
        {
            this.Disable();
            return;
        }

        var cursorTile = Game1.currentCursorTile * Game1.tileSize;
        var cursorBox = new Rectangle((int)cursorTile.X, (int)cursorTile.Y, Game1.tileSize, Game1.tileSize);
        foreach (var character in user.currentLocation.characters)
        {
            if (character is not Monster { IsMonster: true } monster)
            {
                continue;
            }

            var monsterBox = monster.GetBoundingBox();
            if (!monsterBox.Intersects(cursorBox))
            {
                continue;
            }

            State.HoveredEnemy = monster;
            Log.D($"Hovering {monster.Name}!");
            return;
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        State.HoveredEnemy = null;
        Log.D("Hovering no one!");
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var user = Game1.player;
        if (user.CurrentTool is not MeleeWeapon { isOnSpecial: true })
        {
            this.Disable();
            return;
        }

        var hoveredEnemy = State.HoveredEnemy;
        if (hoveredEnemy is null)
        {
            return;
        }

        var currentDirection = (Direction)user.FacingDirection;
        Direction newDirection;
        if (currentDirection.IsHorizontal())
        {
            if (Math.Abs(user.Tile.X - hoveredEnemy.Tile.X) > 0.01f)
            {
                return;
            }

            newDirection = user.FaceTowardsTile(hoveredEnemy.Tile);
        }
        else
        {
            if (Math.Abs(user.Tile.Y - hoveredEnemy.Tile.Y) > 0.01f)
            {
                return;
            }

            newDirection = user.FaceTowardsTile(hoveredEnemy.Tile);
        }

        Log.D($"Auto-turned towards {newDirection}!");
        var angle = currentDirection.AngleWith(newDirection);
        var trajectory = new Vector2(user.xVelocity, user.yVelocity);
        var rotated = trajectory.Rotate(angle);
        user.setTrajectory(rotated);
        Log.D($"New trajectory: ({user.xVelocity}, {user.yVelocity})");
        var frame = newDirection switch
        {
            Direction.Up => 276,
            Direction.Right => 274,
            Direction.Down => 272,
            Direction.Left => 278,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Direction, int>(
                (Direction)user.FacingDirection),
        };

        var sprite = user.FarmerSprite;
        sprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == Game1.left, true);
        sprite.currentAnimationIndex++;
        sprite.CurrentFrame =
            sprite.CurrentAnimation[sprite.currentAnimationIndex % sprite.CurrentAnimation.Count].frame;
        this.Disable();
    }
}
