namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbySwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="StabbySwordSpecialUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbySwordSpecialUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        ++_currentFrame;
        if (_currentFrame == 0)
        {
            ModEntry.Reflector
                .GetUnboundMethodDelegate<Action<MeleeWeapon, Farmer>>(sword, "beginSpecialMove")
                .Invoke(sword, user);

            var facingDirection = (FacingDirection)user.FacingDirection;
            var facingVector = facingDirection.ToVector();
            if (facingDirection.IsVertical())
            {
                facingVector *= -1f;
            }

            var trajectory = facingVector * (20f + (Game1.player.addedSpeed * 2f));
            user.setTrajectory(trajectory);

            _animationFrames =
                sword.hasEnchantmentOfType<ReduxArtfulEnchantment>()
                    ? 24
                    : 16; // don't ask me why but this translated exactly to (5 tiles : 4 tiles)
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
            Game1.playSound("daggerswipe");
        }
        else if (_currentFrame > _animationFrames)
        {
            user.completelyStopAnimatingOrDoingAction();
            user.setTrajectory(Vector2.Zero);
            user.forceCanMove();
#if RELEASE
            MeleeWeapon.attackSwordCooldown = MeleeWeapon.attackSwordCooldownTime;
            if (!ModEntry.Config.EnableProfessions && user.professions.Contains(Farmer.acrobat))
            {
                MeleeWeapon.attackSwordCooldown /= 2;
            }

            if (sword.hasEnchantmentOfType<ArtfulEnchantment>())
            {
                MeleeWeapon.attackSwordCooldown /= 2;
            }

            var cdr = 10f / (10f + sword.GetEnchantmentLevel<GarnetEnchantment>() +
                             sword.Read<float>(DataFields.ResonantCooldownReduction) +
                             user.Read<float>(DataFields.RingCooldownReduction));
#endif
            _currentFrame = -1;
            this.Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame == 1)
            {
                ++sprite.currentAnimationIndex;
            }
            else if (_currentFrame == _animationFrames - 1)
            {
                --sprite.currentAnimationIndex;
            }

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex].frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}
