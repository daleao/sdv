namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Enums;
using DaLion.Common.Events;
using DaLion.Common.Exceptions;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ClubComboUpdateTickingEvent : UpdateTickingEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="ClubComboUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ClubComboUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var club = (MeleeWeapon)user.CurrentTool;
        ++_currentFrame;
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

            var sprite = user.FarmerSprite;
            sprite.setCurrentFrame(frame, 0, 40, _animationFrames, user.FacingDirection == 3, true);
            _animationFrames = (sprite.CurrentAnimation.Count * 3) + 9;
        }
        else if (_currentFrame >= _animationFrames)
        {
            user.completelyStopAnimatingOrDoingAction();
            user.forceCanMove();
            _currentFrame = -1;
            this.Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame >= 6 && _currentFrame < _animationFrames - 6 && _currentFrame % 3 == 0)
            {
                sprite.CurrentFrame = sprite.CurrentAnimation[++sprite.currentAnimationIndex].frame;
            }

            if (_currentFrame == 6)
            {
                Farmer.showToolSwipeEffect(user);
                Game1.playSound("clubswipe");
            }

            if (sprite.currentAnimationIndex >= 4)
            {
                var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 64);
                club.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            }

            user.UsingTool = true;
            user.CanMove = false;
        }
    }
}
