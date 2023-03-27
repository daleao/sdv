namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using System.Diagnostics;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
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
                DoCooldown(user, slingshot);
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
            DoCooldown(user, slingshot);
            this.Disable();
        }
    }

    [Conditional("RELEASE")]
    private static void DoCooldown(Farmer user, Slingshot slingshot)
    {
        if (slingshot.Get_IsOnSpecial())
        {
            SlingshotsModule.State.SlingshotCooldown = SlingshotCooldown;
            if (!ProfessionsModule.IsEnabled && user.professions.Contains(Farmer.acrobat))
            {
                SlingshotsModule.State.SlingshotCooldown /= 2;
            }

            if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>())
            {
                SlingshotsModule.State.SlingshotCooldown /= 2;
            }

            SlingshotsModule.State.SlingshotCooldown = (int)(SlingshotsModule.State.SlingshotCooldown *
                                                          slingshot.Get_EffectiveCooldownReduction() *
                                                          user.Get_CooldownReduction());
        }
        else
        {
            SlingshotsModule.State.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (SlingshotsModule.State.SlingshotCooldown <= 0)
            {
                Game1.playSound("objectiveComplete");
            }
        }
    }
}
