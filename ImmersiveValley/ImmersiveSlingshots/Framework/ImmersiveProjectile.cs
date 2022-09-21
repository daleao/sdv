namespace DaLion.Stardew.Slingshots.Framework;

#region using directives

using System;
using DaLion.Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> that remembers where it came from.</summary>
internal sealed class ImmersiveProjectile : BasicProjectile
{
    private static readonly Lazy<Action<BasicProjectile, GameLocation>> ExplosionAnimation = new(() =>
        typeof(BasicProjectile)
            .RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>());

    public ImmersiveProjectile(
        Slingshot whatFiredMe,
        int damageToFarmer,
        int parentSheetIndex,
        int bouncesTillDestruct,
        int tailLength,
        float rotationVelocity,
        float xVelocity,
        float yVelocity,
        Vector2 startingPosition,
        string collisionSound,
        string firingSound,
        bool explode,
        bool damagesMonsters = false,
        GameLocation? location = null,
        Character? firer = null,
        bool spriteFromObjectSheet = false,
        onCollisionBehavior? collisionBehavior = null)
        : base(
            damageToFarmer,
            parentSheetIndex,
            bouncesTillDestruct,
            tailLength,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            collisionSound,
            firingSound,
            explode,
            damagesMonsters,
            location,
            firer,
            spriteFromObjectSheet,
            collisionBehavior)
    {
        this.MyId = parentSheetIndex;
        this.WhatFiredMe = whatFiredMe;

        switch (spriteFromObjectSheet)
        {
            case true when ModEntry.Config.DisableSlingshotGracePeriod:
                this.ignoreTravelGracePeriod.Value = true;
                break;
            case false:
                switch (parentSheetIndex)
                {
                    case Constants.QuincyProjectileIndex:
                        this.IsQuincy = true;
                        break;
                    case Constants.SnowballProjectileIndex:
                        this.IsSnowball = true;
                        break;
                }

                break;
        }
    }

    public Slingshot WhatFiredMe { get; }

    public int MyId { get; }

    public bool IsQuincy { get; }

    public bool IsSnowball { get; }

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (!this.damagesMonsters.Value)
        {
            return;
        }

        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        ExplosionAnimation.Value(this, location);
        var firer = this.theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        var damage = this.damageToFarmer.Value;
        var knockback = this.IsQuincy
            ? 0f
            : (1f + this.WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier);
        var crate = !this.IsQuincy && ModEntry.Config.EnableSlingshotCrits
            ? (0.05f + (0.046f * this.WhatFiredMe.GetEnchantmentLevel<AquamarineEnchantment>())) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + ((ModEntry.ArsenalConfig?.Value<bool?>("RebalanceEnchants") == true ? 0.5f : 0.1f) *
                   this.WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>())) * (1f + firer.critPowerModifier);
        location.damageMonster(
            monster.GetBoundingBox(),
            damage,
            damage + 1,
            false,
            knockback,
            0,
            crate,
            cpower,
            true,
            firer);
    }
}
