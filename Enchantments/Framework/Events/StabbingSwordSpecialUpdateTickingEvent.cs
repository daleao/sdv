namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Enchantments.Framework.Extensions;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialUpdateTickingEvent(EventManager? manager = null)
    : UpdateTickingEvent(manager ?? EnchantmentsMod.EventManager)
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        Reflector
            .GetUnboundMethodDelegate<Action<MeleeWeapon, Farmer>>(sword, "beginSpecialMove")
            .Invoke(sword, user);

        var facingDirection = (FacingDirection)user.FacingDirection;
        var facingVector = facingDirection.ToVector();
        if (facingDirection.IsVertical())
        {
            facingVector *= -1f; // for some reason up and down are inverted
        }

        var trajectory = facingVector * (20f + (Game1.player.addedSpeed * 2f)) * 1.2f;
        user.setTrajectory(trajectory);

        _animationFrames = 16; // translates exactly to 4 tiles with 0 added speed
        var frame = (FacingDirection)user.FacingDirection switch
        {
            FacingDirection.Up => 276,
            FacingDirection.Right => 274,
            FacingDirection.Down => 272,
            FacingDirection.Left => 278,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                (FacingDirection)user.FacingDirection),
        };

        user.FarmerSprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);
        user.currentLocation.playSound("daggerswipe");
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        var user = Game1.player;
        user.completelyStopAnimatingOrDoingAction();
        user.setTrajectory(Vector2.Zero);
        user.forceCanMove();
        user.FarmerSprite.currentAnimationIndex = 0;
        _currentFrame = 0;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;

        // check for warps to prevent out-of-bounds
        var nextPosition = user.nextPosition(user.FacingDirection);
        if (user.currentLocation.isCollidingWithWarp(nextPosition, user) is { } warp)
        {
            sword.DoStabbingSpecialCooldown();
            this.Disable();
            user.warpFarmer(warp);
            return;
        }

        if (++_currentFrame > _animationFrames)
        {
            sword.DoStabbingSpecialCooldown();
            this.Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame == 1)
            {
                sprite.currentAnimationIndex++;
            }
            else if (_currentFrame == _animationFrames - 1)
            {
                sprite.currentAnimationIndex--;
            }

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex % sprite.CurrentAnimation.Count]
                .frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}
