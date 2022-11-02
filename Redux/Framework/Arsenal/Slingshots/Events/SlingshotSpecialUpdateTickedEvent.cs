namespace DaLion.Redux.Framework.Arsenal.Slingshots.Events;

#region using directives

using DaLion.Redux.Framework.Arsenal.Slingshots.Extensions;
using DaLion.Redux.Framework.Arsenal.Slingshots.VirtualProperties;
using DaLion.Redux.Framework.Arsenal.Weapons.Enchantments;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSpecialUpdateTickedEvent : UpdateTickedEvent
{
    private const int SlingshotCooldown = 2000;
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
        var slingshot = (Slingshot)user.CurrentTool;
        if (slingshot.Get_IsOnSpecial())
        {
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

                var sprite = (FarmerSprite)user.Sprite;
                sprite.setCurrentFrame(frame, 0, 40, _animationFrames, user.FacingDirection == 3, true);
                _animationFrames = (sprite.CurrentAnimation.Count * 3) + 9;
            }
            else if (_currentFrame >= _animationFrames)
            {
                user.completelyStopAnimatingOrDoingAction();
                slingshot.Set_IsOnSpecial(false);
                user.forceCanMove();
#if RELEASE
                ModEntry.State.Arsenal.SlingshotCooldown = SlingshotCooldown;
                if (!ModEntry.Config.EnableProfessions && user.professions.Contains(Farmer.acrobat))
                {
                    ModEntry.State.Arsenal.SlingshotCooldown /= 2;
                }

                if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>())
                {
                    ModEntry.State.Arsenal.SlingshotCooldown /= 2;
                }

                if (slingshot.hasEnchantmentOfType<GarnetEnchantment>())
                {
                    ModEntry.State.Arsenal.SlingshotCooldown = (int)(ModEntry.State.Arsenal.SlingshotCooldown *
                                                                     (1f - (slingshot
                                                                                .GetEnchantmentLevel<
                                                                                    TopazEnchantment>() *
                                                                            0.1f)));
                }

                if (ModEntry.Config.EnableRings)
                {
                    ModEntry.State.Arsenal.SlingshotCooldown = (int)(ModEntry.State.Arsenal.SlingshotCooldown *
                                                                     (1f - user.Read<float>(
                                                                         DataFields.ResonantCooldownReduction)));
                }
#endif
                _currentFrame = -1;
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
                    Game1.playSound("swordswipe");
                }

                if (sprite.currentAnimationIndex >= 4)
                {
                    var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 64);
                    slingshot.DoDamage((int)x, (int)y, user);
                }

                user.UsingTool = true;
                user.CanMove = false;
            }
        }
        else
        {
#if RELEASE
            ModEntry.State.Arsenal.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (ModEntry.State.Arsenal.SlingshotCooldown > 0)
            {
                return;
            }

            Game1.playSound("objectiveComplete");
#endif
            this.Disable();
        }
    }
}
