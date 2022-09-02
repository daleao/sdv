namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Extensions.Reflection;
using Common.Extensions.Xna;
using Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using Ultimates;
using VirtualProperties;

#endregion using directives

/// <summary>A <see cref="BasicProjectile"/> that remembers where it came from and some other properties.</summary>
internal sealed class ImmersiveProjectile : BasicProjectile
{
    private static readonly Lazy<Action<BasicProjectile, GameLocation>> _ExplosionAnimation = new(() =>
        typeof(BasicProjectile).RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>());

    public Item? WhatAmI { get; }
    public Slingshot WhatFiredMe { get; }
    public float Overcharge { get; set; }
    public bool DidBounce { get; set; }
    public bool DidPierce { get; set; }
    public bool IsQuincy { get; set; }
    public bool CanBeRecovered { get; set; }

    public ImmersiveProjectile(Slingshot whatFiredMe, float overcharge, bool canBeRecovered, int damageToFarmer, int parentSheetIndex, int bouncesTillDestruct,
        int tailLength, float rotationVelocity, float xVelocity, float yVelocity, Vector2 startingPosition,
        string collisionSound, string firingSound, bool explode, bool damagesMonsters = false,
        GameLocation? location = null, Character? firer = null, bool spriteFromObjectSheet = false,
        onCollisionBehavior? collisionBehavior = null)
        : base(damageToFarmer, parentSheetIndex, bouncesTillDestruct, tailLength, rotationVelocity, xVelocity,
            yVelocity, startingPosition, collisionSound, firingSound, explode, damagesMonsters, location, firer,
            spriteFromObjectSheet, collisionBehavior)
    {
        WhatFiredMe = whatFiredMe;
        WhatAmI = whatFiredMe.attachments[0]?.getOne();
        Overcharge = overcharge;
        switch (spriteFromObjectSheet)
        {
            case true when ModEntry.ArsenalConfig?.Value<bool?>("RemoveSlingshotGracePeriod") == true:
                ignoreTravelGracePeriod.Value = true;
                break;
            case false:
                IsQuincy = true;
                break;
        }

        CanBeRecovered = canBeRecovered;
    }

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (!damagesMonsters.Value) return;

        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        var firer = theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        var damage = damageToFarmer.Value;
        var knockback = IsQuincy
            ? 0f
            : (1f + WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier) * Overcharge;
        var crate = !IsQuincy && ModEntry.ArsenalConfig?.Value<bool?>("EnableSlingshotCrits") == true
            ? (0.05f + 0.046f * WhatFiredMe.GetEnchantmentLevel<AquamarineEnchantment>()) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + (ModEntry.ArsenalConfig?.Value<bool?>("RebalanceEnchants") == true ? 0.5f : 0.1f) *
                WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>()) * (1f + firer.critPowerModifier);
        if (currentTileSheetIndex == 766) // piper slime
        {
            // heal if slime
            if (monster.IsSlime())
            {
                var amount = Game1.random.Next(damage - 2, damage + 2);
                monster.Health = Math.Min(monster.Health + amount, monster.MaxHealth);
                location.debris.Add(new(amount,
                    new(monster.getStandingX() + 8, monster.getStandingY()), Color.Lime, 1f, monster));
                Game1.playSound("healSound");
                _ExplosionAnimation.Value(this, location);
            }
            // debuff if not
            else
            {
                location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, knockback, 0, crate,
                    cpower, true, firer);
                if (!monster.CanBeSlowed() || !(Game1.random.NextDouble() < 2d / 3d)) return;

                monster.get_SlowIntensity().Value = 2;
                monster.get_SlowTimer().Value = 5123 + Game1.random.Next(-2, 3) * 456;
                monster.set_Slower(firer);
            }

            return;
        }

        location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, knockback, 0, crate, cpower, true,
            firer);

        // check for piercing
        if (IsMineralAmmo() && Game1.random.NextDouble() < (Overcharge - 1f) / 2f)
            DidPierce = true;
        else
            _ExplosionAnimation.Value(this, location);

        // check for stun
        if (!DidPierce && firer.HasProfession(Profession.Rascal, true) && DidBounce)
            monster.stunTime = 5000;

        // increment Desperado ultimate meter
        if (firer.IsLocalPlayer && firer.get_Ultimate() is DeathBlossom { IsActive: false } blossom)
            blossom.ChargeValue += (DidBounce || DidPierce ? 18 : 12) - 10 * firer.health / firer.maxHealth;
    }

    public override bool update(GameTime time, GameLocation location)
    {
        var didCollide = base.update(time, location);

        if (!damagesMonsters.Value || Overcharge <= 1f || travelDistance < maxTravelDistance.Value || IsQuincy) return didCollide;

        // check if already collided
        if (didCollide)
        {
            if (!DidPierce) return didCollide;

            damageToFarmer.Value = (int)(damageToFarmer.Value * 0.6f);
            return false;
        }

        // get collision angle
        var velocity = new Vector2(xVelocity.Value, yVelocity.Value);
        var angle = velocity.AngleWithHorizontal();
        if (angle > 180) angle -= 360;

        // check for extended collision
        var originalHitbox = getBoundingBox();
        var newHitbox = new Rectangle(originalHitbox.X, originalHitbox.Y, originalHitbox.Width, originalHitbox.Height);
        var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
        if (isBulletTravelingVertically)
        {
            newHitbox.Inflate((int)(originalHitbox.Width * Overcharge), 0);
            if (newHitbox.Width <= originalHitbox.Width) return didCollide;
        }
        else
        {
            newHitbox.Inflate(0, (int)(originalHitbox.Height * Overcharge));
            if (newHitbox.Height <= originalHitbox.Height) return didCollide;
        }

        if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster monster) return didCollide;

        // do deal damage
        int actualDistance, monsterRadius, actualBulletRadius, extendedBulletRadius;
        if (isBulletTravelingVertically)
        {
            actualDistance = Math.Abs(monster.getStandingX() - originalHitbox.Center.X);
            monsterRadius = monster.GetBoundingBox().Width / 2;
            actualBulletRadius = originalHitbox.Width / 2;
            extendedBulletRadius = newHitbox.Width / 2;
        }
        else
        {
            actualDistance = Math.Abs(monster.getStandingY() - originalHitbox.Center.Y);
            monsterRadius = monster.GetBoundingBox().Height / 2;
            actualBulletRadius = originalHitbox.Height / 2;
            extendedBulletRadius = newHitbox.Height / 2;
        }

        var lerpFactor = (actualDistance - (actualBulletRadius + monsterRadius)) /
                         (extendedBulletRadius - actualBulletRadius);
        var multiplier = MathHelper.Lerp(1f, 0f, lerpFactor);
        var firer = theOneWhoFiredMe.Get(Game1.currentLocation) as Farmer ?? Game1.player;
        var damage = (int)(damageToFarmer.Value * multiplier);
        var knockback = WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>() * (1f + firer.knockbackModifier) *
                        multiplier;
        var crate = ModEntry.ArsenalConfig?.Value<bool?>("EnableSlingshotCrits") == true
            ? (0.05f + 0.046f * WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + (ModEntry.ArsenalConfig?.Value<bool?>("RebalanceEnchants") == true ? 0.5f : 0.1f) *
                WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>()) * (1f + firer.critPowerModifier);
        location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, knockback, 0,
            crate, cpower, true, firer);
        return didCollide;
    }

    /// <summary>Whether the projectile is a stone or mineral ore.</summary>
    public bool IsMineralAmmo() => WhatAmI?.ParentSheetIndex.IsMineralAmmoIndex() == true;

    /// <summary>Whether the projectile is a slab of slime.</summary>
    public bool IsSlimeAmmo() => WhatAmI?.ParentSheetIndex == 766;

    /// <summary>Whether the projectile is explosive.</summary>
    public bool IsExplosiveAmmo() => WhatAmI?.ParentSheetIndex == 442;

    /// <summary>Whether the projectile is a piece of wood.</summary>
    public bool IsWood() => WhatAmI?.ParentSheetIndex == SObject.wood;

    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public bool IsSquishyAmmo() => WhatAmI?.IsSquishyAmmo() == true;
}