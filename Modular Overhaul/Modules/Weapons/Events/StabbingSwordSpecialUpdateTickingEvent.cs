namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Enchantments.Events;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="StabbingSwordSpecialUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbingSwordSpecialUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

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

        var trajectory = facingVector * (20f + (Game1.player.addedSpeed * 2f)) *
                         (sword.hasEnchantmentOfType<MeleeArtfulEnchantment>()
                             ? 1.5f
                             : 1.2f);
        user.setTrajectory(trajectory);

        _animationFrames = sword.hasEnchantmentOfType<MeleeArtfulEnchantment>()
                ? 24
                : 16; // translates exactly to (6 tiles : 4 tiles) with 0 added speed
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
        Game1.playSound(sword.CurrentParentTileIndex == ItemIDs.LavaKatana ? "fireball" : "daggerswipe");
        this.Manager.Enable<StabbingSwordSpecialInterruptedButtonPressedEvent>();
        if (WeaponsModule.Config.FaceMouseCursor)
        {
            this.Manager.Enable<StabbingSwordSpecialHomingUpdateTickedEvent>();
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        var user = Game1.player;
        user.completelyStopAnimatingOrDoingAction();
        user.setTrajectory(Vector2.Zero);
        user.forceCanMove();
        _currentFrame = 0;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        if (++_currentFrame > _animationFrames)
        {
            if (sword.hasEnchantmentOfType<MeleeArtfulEnchantment>())
            {
                if (!this.Manager.Enable<ArtfulDashUpdateTickedEvent>())
                {
                    this.Manager.Disable<ArtfulDashUpdateTickedEvent>();
                }
            }
            else
            {
                user.DoStabbingSpecielCooldown(sword);
            }

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

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex].frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}
