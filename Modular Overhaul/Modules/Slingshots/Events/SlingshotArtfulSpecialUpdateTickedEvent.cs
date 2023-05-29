namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotArtfulSpecialUpdateTickedEvent : UpdateTickedEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="SlingshotArtfulSpecialUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotArtfulSpecialUpdateTickedEvent(EventManager manager)
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

        if (slingshot.Get_IsOnSpecial())
        {
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
                user.forceCanMove();
                user.DoSlingshotSpecialCooldown(slingshot);
                _currentFrame = -1;
            }
            else
            {
                var sprite = user.FarmerSprite;
                if (_currentFrame >= 6 && _currentFrame % 3 == 0)
                {
                    sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex++].frame;
                }

                if (_currentFrame == 10)
                {
                    Game1.playSound("swordswipe");
                }
                else if (_currentFrame == 20)
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
        else
        {
            user.DoSlingshotSpecialCooldown(slingshot);
            this.Disable();
        }
    }
}
