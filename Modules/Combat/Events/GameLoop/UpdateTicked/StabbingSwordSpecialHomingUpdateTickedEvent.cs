namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

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
        for (var i = 0; i < user.currentLocation.characters.Count; i++)
        {
            var character = user.currentLocation.characters[i];
            if (character is not Monster monster)
            {
                continue;
            }

            var monsterBox = monster.GetBoundingBox();
            if (!monsterBox.Intersects(cursorBox))
            {
                continue;
            }

            CombatModule.State.HoveredEnemy = monster;
            Log.D($"[CMBT]: Hovering {monster.Name}!");
            return;
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        CombatModule.State.HoveredEnemy = null;
        Log.D("[CMBT]: Hovering no one!");
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

        var hoveredEnemy = CombatModule.State.HoveredEnemy;
        if (hoveredEnemy is null)
        {
            return;
        }

        var currentDirection = (FacingDirection)user.FacingDirection;
        FacingDirection newDirection;
        if (currentDirection.IsHorizontal())
        {
            if (user.getTileX() != hoveredEnemy.getTileX())
            {
                return;
            }

            newDirection = user.FaceTowardsTile(hoveredEnemy.getTileLocation());
        }
        else
        {
            if (user.getTileY() != hoveredEnemy.getTileY())
            {
                return;
            }

            newDirection = user.FaceTowardsTile(hoveredEnemy.getTileLocation());
        }

        Log.D($"[CMBT]: Auto-turned towards {newDirection}!");
        var angle = currentDirection.AngleWith(newDirection);
        var trajectory = new Vector2(user.xVelocity, user.yVelocity);
        var rotated = trajectory.Rotate(angle);
        user.setTrajectory(rotated);
        Log.D($"[CMBT]: New trajectory: ({user.xVelocity}, {user.yVelocity})");
        var frame = newDirection switch
        {
            FacingDirection.Up => 276,
            FacingDirection.Right => 274,
            FacingDirection.Down => 272,
            FacingDirection.Left => 278,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                (FacingDirection)user.FacingDirection),
        };

        var sprite = user.FarmerSprite;
        sprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);
        sprite.currentAnimationIndex++;
        sprite.CurrentFrame =
            sprite.CurrentAnimation[sprite.currentAnimationIndex % sprite.CurrentAnimation.Count].frame;
        this.Disable();
    }
}
