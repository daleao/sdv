namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

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
internal sealed class SlingshotSpecialUpdateTickedEvent : UpdateTickedEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="SlingshotSpecialUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSpecialUpdateTickedEvent(EventManager manager)
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

        var facingDirection = (FacingDirection)user.FacingDirection;
        if (slingshot.Get_IsOnSpecial())
        {
            _currentFrame++;
            if (_currentFrame == 0)
            {
                var frame = (FacingDirection)user.FacingDirection switch
                {
                    FacingDirection.Up => 176,
                    FacingDirection.Right => 168,
                    FacingDirection.Down => 160,
                    FacingDirection.Left => 184,
                    _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                        (FacingDirection)user.FacingDirection),
                };

                var sprite = (FarmerSprite)user.Sprite;
                sprite.setCurrentFrame(frame, 0, 40, _animationFrames, user.FacingDirection == 3, true);
                _animationFrames = (sprite.CurrentAnimation.Count * 3) + 11;
            }
            else if (_currentFrame >= _animationFrames)
            {
                user.completelyStopAnimatingOrDoingAction();
                slingshot.Set_IsOnSpecial(false);
                user.forceCanMove();
                user.DoSlingshotSpecialCooldown(slingshot);
                _currentFrame = -1;
            }
            else
            {
                var sprite = user.FarmerSprite;
                if (_currentFrame >= 6 && _currentFrame < _animationFrames - 6 && _currentFrame % 3 == 0)
                {
                    sprite.CurrentFrame =
                        sprite.CurrentAnimation[sprite.currentAnimationIndex++ % sprite.CurrentAnimation.Count].frame;
                }

                if (_currentFrame == 6)
                {
                    Game1.playSound("swordswipe");
                }
                else if (_currentFrame == (facingDirection.IsHorizontal() ? 10 : 14))
                {
                    Farmer.showToolSwipeEffect(user);
                }

                if (sprite.currentAnimationIndex >= 4)
                {
                    var (x, y) = user.getTileLocation() * Game1.tileSize;
                    slingshot.DoDamage((int)x, (int)y, user);
                }

                user.UsingTool = true;
                user.CanMove = false;
            }
        }
        else
        {
            user.DoSlingshotSpecialCooldown(slingshot);
            this.Disable();
        }
    }
}
