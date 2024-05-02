namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSwipeUpdateTickedEvent : UpdateTickedEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="SlingshotSwipeUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSwipeUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var user = Game1.player;
        if (user.CurrentTool is not Slingshot slingshot)
        {
            this.Disable();
            return;
        }

        _currentFrame++;
        if (_currentFrame == 0)
        {
            var frame = (FacingDirection)user.FacingDirection switch
            {
                FacingDirection.Up => 248,
                FacingDirection.Right => 240,
                FacingDirection.Down => 232,
                FacingDirection.Left => 256,
                _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                    (FacingDirection)user.FacingDirection),
            };

            var sprite = (FarmerSprite)user.Sprite;
            sprite.setCurrentFrame(frame, 0, 60, _animationFrames, user.FacingDirection == 3, true);
            _animationFrames = (sprite.CurrentAnimation.Count * 3) + 3;
        }
        else if (_currentFrame >= _animationFrames)
        {
            user.completelyStopAnimatingOrDoingAction();
            slingshot.Set_IsOnSpecial(false);
            user.DoSlingshotSpecialCooldown(slingshot);
            user.forceCanMove();
            _currentFrame = -1;
            this.Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame >= 3 && _currentFrame % 3 == 0)
            {
                sprite.CurrentFrame =
                    sprite.CurrentAnimation[sprite.currentAnimationIndex++ % sprite.CurrentAnimation.Count].frame;
            }

            if (_currentFrame == 10)
            {
                user.currentLocation.playSound("swordswipe");
            }
            else if (_currentFrame == 16)
            {
                slingshot.ShowSwordSwipe(user);
            }

            if (sprite.currentAnimationIndex >= 4)
            {
                var (x, y) = user.GetToolLocation(true);
                slingshot.DoDamage((int)x, (int)y, user);
            }

            user.UsingTool = true;
            user.CanMove = false;
        }
    }
}
