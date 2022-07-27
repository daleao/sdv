namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using Common.Extensions.Reflection;
using Enchantments;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbySwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static readonly Action<MeleeWeapon, Farmer> _BeginSpecialMove = typeof(MeleeWeapon).RequireMethod("beginSpecialMove")
        .CompileUnboundDelegate<Action<MeleeWeapon, Farmer>>();

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
            _BeginSpecialMove(sword, user);
            _animationFrames = sword.hasEnchantmentOfType<InfinityEnchantment>() ? 24 : 15; // don't ask me why but this translated exactly to (5 tiles : 4 tiles)
#pragma warning disable CS8509
            var frame = user.FacingDirection switch
            {
                Game1.up => 276,
                Game1.right => 274,
                Game1.down => 272,
                Game1.left => 278
            };

            ((FarmerSprite)user.Sprite).setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);

            Vector2 trajectory = user.FacingDirection switch
            {
                Game1.up => new(0, 25), // south
                Game1.right => new(25, 0), // east
                Game1.down => new(0, -25), // north
                Game1.left => new(-25, 0), // west
            };
#pragma warning restore CS8509

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