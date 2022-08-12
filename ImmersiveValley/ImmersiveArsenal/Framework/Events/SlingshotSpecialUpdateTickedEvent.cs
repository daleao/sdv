namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Enums;
using Common.Events;
using Common.Exceptions;
using Common.Extensions.Stardew;
using Enchantments;
using Extensions;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSpecialUpdateTickedEvent : UpdateTickedEvent
{
    private static int _currentFrame = -1, _animationFrames;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSpecialUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var user = Game1.player;
        var slingshot = (Slingshot)user.CurrentTool;
        if (slingshot.get_IsOnSpecial())
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
                        (FacingDirection)user.FacingDirection)
                };

                var sprite = (FarmerSprite)user.Sprite;
                sprite.setCurrentFrame(frame, 0, 40, _animationFrames, user.FacingDirection == 3, true);
                _animationFrames = sprite.CurrentAnimation.Count * 3 + 9;
            }
            else if (_currentFrame >= _animationFrames)
            {
                user.completelyStopAnimatingOrDoingAction();
                slingshot.set_IsOnSpecial(false);
                user.forceCanMove();
#if RELEASE
            ModEntry.State.SlingshotCooldown = Constants.SLINGSHOT_COOLDOWN_TIME_I;
            if (ModEntry.ProfessionsApi is null && user.professions.Contains(Farmer.acrobat)) ModEntry.State.SlingshotCooldown
 /= 2;
            if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>()) ModEntry.State.SlingshotCooldown /= 2;
            if (slingshot.hasEnchantmentOfType<GarnetEnchantment>())
                ModEntry.State.SlingshotCooldown = (int)(ModEntry.State.SlingshotCooldown *
                                                         (1f - slingshot.GetEnchantmentLevel<TopazEnchantment>() * 0.1f));
            if (ModEntry.IsImmersiveRingsLoaded)
                ModEntry.State.SlingshotCooldown = (int) (ModEntry.State.SlingshotCooldown * (1f - user.Read<float>("CooldownReduction", modId: "DaLion.ImmersiveRings")));
#endif
                _currentFrame = -1;
            }
            else
            {
                var sprite = user.FarmerSprite;
                if (_currentFrame >= 6 && _currentFrame < _animationFrames - 6 && _currentFrame % 3 == 0)
                    sprite.CurrentFrame = sprite.CurrentAnimation[++sprite.currentAnimationIndex].frame;

                if (_currentFrame == 6)
                {
                    Farmer.showToolSwipeEffect(Game1.player);
                    Game1.playSound("swordswipe");
                }

                if (sprite.currentAnimationIndex >= 4)
                {
                    var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 64);
                    slingshot.DoDamage((int)x, (int)y, user);
                }

                Game1.player.UsingTool = true;
                Game1.player.CanMove = false;
            }
        }
        else
        {
#if RELEASE
            ModEntry.State.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (ModEntry.State.SlingshotCooldown > 0) return;

            Game1.playSound("objectiveComplete");
#endif
            Disable();
        }
    }
}