namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> that remembers where it came from.</summary>
internal sealed class ImmersiveProjectile : BasicProjectile
{
    private static readonly Lazy<Action<BasicProjectile, GameLocation>> _ExplosionAnimation = new(() =>
        typeof(BasicProjectile).RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>());

    public Slingshot WhatFiredMe { get; }
    public int MyID { get; }
    public bool IsQuincy { get; }
    public bool IsSnowball { get; }

    public ImmersiveProjectile(Slingshot whatFiredMe, int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct,
        int tailLength, float rotationVelocity, float xVelocity, float yVelocity, Vector2 startingPosition,
        string collisionSound, string firingSound, bool explode, bool damagesMonsters = false,
        GameLocation? location = null, Character? firer = null, bool spriteFromObjectSheet = false,
        onCollisionBehavior? collisionBehavior = null)
        : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, xVelocity,
            yVelocity, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer,
            spriteFromObjectSheet, collisionBehavior)
    {
        MyID = parentSheetIndex;
        WhatFiredMe = whatFiredMe;

        switch (spriteFromObjectSheet)
        {
            case true when ModEntry.Config.DisableSlingshotGracePeriod:
                ignoreTravelGracePeriod.Value = true;
                break;
            case false:
                switch (parentSheetIndex)
                {
                    case Constants.QUINCY_PROJECTILE_INDEX_I:
                        IsQuincy = true;
                        break;
                    case Constants.SNOWBALL_PROJECTILE_INDEX_I:
                        IsSnowball = true;
                        break;
                }

                break;
        }
    }

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (!damagesMonsters.Value) return;

        if (n is not Monster {IsMonster: true} monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        _ExplosionAnimation.Value(this, location);
        var firer = theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        var damage = damageToFarmer.Value;
        var knockback = IsQuincy
            ? 0f
            : (1f + WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier);
        var crate = !IsQuincy && ModEntry.Config.EnableSlingshotCrits
            ? (0.05f + 0.046f * WhatFiredMe.GetEnchantmentLevel<AquamarineEnchantment>()) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + (ModEntry.Config.RebalancedForges ? 0.5f : 0.1f) *
                WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>()) * (1f + firer.critPowerModifier);
        location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, knockback, 0, crate, cpower, true,
            firer);
    }
}