namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Enums;
using Common.Events;
using Common.Exceptions;
using Common.Extensions.Reflection;
using Enchantments;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbySwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static readonly Lazy<Action<MeleeWeapon, Farmer>> _BeginSpecialMove = new(() =>
        typeof(MeleeWeapon).RequireMethod("beginSpecialMove").CompileUnboundDelegate<Action<MeleeWeapon, Farmer>>());

    private static int _currentFrame = -1, _animationFrames;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbySwordSpecialUpdateTickingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        ++_currentFrame;
        if (_currentFrame == 0)
        {
            _BeginSpecialMove.Value(sword, user);
            _animationFrames = sword.hasEnchantmentOfType<InfinityEnchantment>() ? 24 : 15; // don't ask me why but this translated exactly to (5 tiles : 4 tiles)
            var frame = (FacingDirection)user.FacingDirection switch
            {
                FacingDirection.Up => 276,
                FacingDirection.Right => 274,
                FacingDirection.Down => 272,
                FacingDirection.Left => 278,
                _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                    (FacingDirection)user.FacingDirection)
            };

            ((FarmerSprite)user.Sprite).setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);

            Vector2 trajectory;
            if (ModEntry.IsCombatControlsLoaded && !Game1.options.gamepadControls)
            {
                trajectory = Common.Utility.GetRelativeCursorDirection() * 25f;
            }
            else
            {
                trajectory = (FacingDirection)user.FacingDirection switch
                {
                    FacingDirection.Up => new(0, 25f), // south
                    FacingDirection.Right => new(25f, 0), // east
                    FacingDirection.Down => new(0, -25f), // north
                    FacingDirection.Left => new(-25f, 0), // west
                    _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, Vector2>(
                        (FacingDirection)user.FacingDirection)
                };
            }

            trajectory *= 1f + Game1.player.addedSpeed / 10f;
            user.setTrajectory(trajectory);
            Game1.playSound("daggerswipe");
        }
        else if (_currentFrame > _animationFrames)
        {
            user.completelyStopAnimatingOrDoingAction();
            user.setTrajectory(Vector2.Zero);
            user.forceCanMove();
#if RELEASE
            MeleeWeapon.attackSwordCooldown = MeleeWeapon.attackSwordCooldownTime;
            if (!ModEntry.IsImmersiveProfessionsLoaded && user.professions.Contains(Farmer.acrobat)) MeleeWeapon.attackSwordCooldown /= 2;
            if (sword.hasEnchantmentOfType<ArtfulEnchantment>()) MeleeWeapon.attackSwordCooldown /= 2;
            if (ModEntry.Config.TopazPerk == ModConfig.Perk.Cooldown)
                MeleeWeapon.attackSwordCooldown = (int) (MeleeWeapon.attackSwordCooldown *
                                                         (1f - sword.GetEnchantmentLevel<TopazEnchantment>() * 0.1f));
#endif
            _currentFrame = -1;
            Disable();
        }
        else
        {
            var sprite = (FarmerSprite)user.Sprite;
            if (_currentFrame == 1) ++sprite.currentAnimationIndex;
            else if (_currentFrame == _animationFrames - 1) --sprite.currentAnimationIndex;

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex].frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}