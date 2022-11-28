namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
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
                _animationFrames = (sprite.CurrentAnimation.Count * 3) + 9;
            }
            else if (_currentFrame >= _animationFrames)
            {
                user.completelyStopAnimatingOrDoingAction();
                slingshot.Set_IsOnSpecial(false);
                user.forceCanMove();
#if RELEASE
                slingshot.Set_SpecialCooldown(SlingshotCooldown);
                if (!ModEntry.Config.EnableProfessions && user.professions.Contains(Farmer.acrobat))
                {
                    slingshot.Halve_SpecialCooldown();
                }

                if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>())
                {
                    slingshot.Halve_SpecialCooldown();
                }

                slingshot.Set_SpecialCooldown((int)(slingshot.Get_SpecialCooldown() *
                                                    slingshot.Get_EffectiveCooldownReduction() *
                                                    user.Get_CooldownReduction()));
#endif
                _currentFrame = -1;
            }
            else
            {
                var sprite = user.FarmerSprite;
                if (_currentFrame >= 6 && _currentFrame < _animationFrames - 6 && _currentFrame % 3 == 0)
                {
                    sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex++].frame;
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
            slingshot.Decrement_SpecialCooldown(Game1.currentGameTime.ElapsedGameTime.Milliseconds);
            if (slingshot.Get_SpecialCooldown() > 0)
            {
                return;
            }

            Game1.playSound("objectiveComplete");
#endif
            this.Disable();
        }
    }
}
