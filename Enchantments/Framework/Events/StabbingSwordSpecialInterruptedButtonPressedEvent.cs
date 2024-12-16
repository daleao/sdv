namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialInterruptedButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StabbingSwordSpecialInterruptedButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbingSwordSpecialInterruptedButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var user = Game1.player;
        if (user.CurrentTool is not MeleeWeapon { isOnSpecial: true })
        {
            this.Disable();
            return;
        }

        var direction = (Direction)user.FacingDirection;
        var angle = 0d;
        Vector2 trajectory;
        switch (e.Button)
        {
            case SButton.W or SButton.LeftThumbstickUp or SButton.DPadUp:
                if (direction.IsHorizontal())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = direction switch
                    {
                        Direction.Left => -90d,
                        Direction.Right => 90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)Direction.Up;
                }

                break;

            case SButton.D or SButton.LeftThumbstickRight or SButton.DPadRight:
                if (direction.IsVertical())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = direction switch
                    {
                        Direction.Up => -90d,
                        Direction.Down => 90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)Direction.Right;
                }

                break;

            case SButton.S or SButton.LeftThumbstickDown or SButton.DPadDown:
                if (direction.IsHorizontal())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = direction switch
                    {
                        Direction.Left => 90d,
                        Direction.Right => -90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)Direction.Down;
                }

                break;

            case SButton.A or SButton.LeftThumbstickLeft or SButton.DPadLeft:
                if (direction.IsVertical())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = direction switch
                    {
                        Direction.Up => 90d,
                        Direction.Down => -90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)Direction.Left;
                }

                break;
        }

        if (angle == 0)
        {
            return;
        }

        var frame = (Direction)user.FacingDirection switch
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
        this.Manager.Disable<StabbingSwordSpecialHomingUpdateTickedEvent>();
        this.Disable();
    }
}
