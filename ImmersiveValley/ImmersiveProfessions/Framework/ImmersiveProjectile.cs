namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Xna;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A <see cref="BasicProjectile"/> that remembers where it came from and some other properties.</summary>
internal sealed class ImmersiveProjectile : BasicProjectile
{
    private static readonly Lazy<Action<BasicProjectile, GameLocation>> ExplosionAnimation = new(() =>
        typeof(BasicProjectile)
            .RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>());

    public ImmersiveProjectile(
        Slingshot whatFiredMe,
        float overcharge,
        bool canBeRecovered,
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
        this.WhatFiredMe = whatFiredMe;
        this.WhatAmI = whatFiredMe.attachments[0]?.getOne();
        this.Overcharge = overcharge;
        switch (spriteFromObjectSheet)
        {
            case true when ModEntry.ArsenalConfig?.Value<bool?>("RemoveSlingshotGracePeriod") == true:
                this.ignoreTravelGracePeriod.Value = true;
                break;
            case false:
                this.IsQuincy = true;
                break;
        }

        this.CanBeRecovered = canBeRecovered;
    }

    public Item? WhatAmI { get; }

    public Slingshot WhatFiredMe { get; }

    public float Overcharge { get; set; }

    public bool DidBounce { get; set; }

    public bool DidPierce { get; set; }

    public bool IsQuincy { get; set; }

    public bool CanBeRecovered { get; set; }

    public bool IsMineralAmmo => this.WhatAmI?.ParentSheetIndex.IsMineralAmmoIndex() == true;

    public bool IsSlimeAmmo => this.WhatAmI?.ParentSheetIndex == 766;

    public bool IsExplosiveAmmo => this.WhatAmI?.ParentSheetIndex == 442;

    public bool IsWood => this.WhatAmI?.ParentSheetIndex == SObject.wood;

    public bool IsSquishyAmmo => this.WhatAmI?.IsSquishyAmmo() == true;

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

        var firer = this.theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        var damage = this.damageToFarmer.Value;
        var knockback = this.IsQuincy
            ? 0f
            : (1f + this.WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier) *
              this.Overcharge;
        var crate = !this.IsQuincy && ModEntry.ArsenalConfig?.Value<bool?>("EnableSlingshotCrits") == true
            ? (0.05f + (0.046f * this.WhatFiredMe.GetEnchantmentLevel<AquamarineEnchantment>())) *
              (1f + firer.critChanceModifier)
            : 0;
        var cpower =
            (1f + ((ModEntry.ArsenalConfig?.Value<bool?>("RebalanceEnchants") == true ? 0.5f : 0.1f) *
                   this.WhatFiredMe.GetEnchantmentLevel<JadeEnchantment>())) * (1f + firer.critPowerModifier);
        if (this.currentTileSheetIndex.Value == 766)
        {
            // piper slime
            if (monster.IsSlime())
            {
                // heal if slime
                var amount = Game1.random.Next(damage - 2, damage + 2);
                monster.Health = Math.Min(monster.Health + amount, monster.MaxHealth);
                location.debris.Add(new Debris(
                    amount,
                    new Vector2(monster.getStandingX() + 8, monster.getStandingY()),
                    Color.Lime,
                    1f,
                    monster));
                Game1.playSound("healSound");
                ExplosionAnimation.Value(this, location);
            }
            else
            {
                // debuff if not
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
                if (!monster.CanBeSlowed() || !(Game1.random.NextDouble() < 2d / 3d))
                {
                    return;
                }

                monster.Get_SlowIntensity().Value = 2;
                monster.Get_SlowTimer().Value = 5123 + (Game1.random.Next(-2, 3) * 456);
                monster.Set_Slower(firer);
            }

            return;
        }

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

        // check for piercing
        if (this.IsMineralAmmo && Game1.random.NextDouble() < (this.Overcharge - 1f) / 2f)
        {
            this.DidPierce = true;
        }
        else
        {
            ExplosionAnimation.Value(this, location);
        }

        // check for stun
        if (!this.DidPierce && firer.HasProfession(Profession.Rascal, true) && this.DidBounce)
        {
            monster.stunTime = 5000;
        }

        // increment Desperado ultimate meter
        if (firer.IsLocalPlayer && firer.Get_Ultimate() is DeathBlossom { IsActive: false } blossom)
        {
            blossom.ChargeValue += (this.DidBounce || this.DidPierce ? 18 : 12) - (10 * firer.health / firer.maxHealth);
        }
    }

    public override bool update(GameTime time, GameLocation location)
    {
        var didCollide = base.update(time, location);

        if (!this.damagesMonsters.Value || this.Overcharge <= 1f ||
            this.travelDistance < this.maxTravelDistance.Value || this.IsQuincy)
        {
            return didCollide;
        }

        // check if already collided
        if (didCollide)
        {
            if (!this.DidPierce)
            {
                return didCollide;
            }

            this.damageToFarmer.Value = (int)(this.damageToFarmer.Value * 0.6f);
            return false;
        }

        // get collision angle
        var velocity = new Vector2(this.xVelocity.Value, this.yVelocity.Value);
        var angle = velocity.AngleWithHorizontal();
        if (angle > 180)
        {
            angle -= 360;
        }

        // check for extended collision
        var originalHitbox = this.getBoundingBox();
        var newHitbox = new Rectangle(originalHitbox.X, originalHitbox.Y, originalHitbox.Width, originalHitbox.Height);
        var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
        if (isBulletTravelingVertically)
        {
            newHitbox.Inflate((int)(originalHitbox.Width * this.Overcharge), 0);
            if (newHitbox.Width <= originalHitbox.Width)
            {
                return didCollide;
            }
        }
        else
        {
            newHitbox.Inflate(0, (int)(originalHitbox.Height * this.Overcharge));
            if (newHitbox.Height <= originalHitbox.Height)
            {
                return didCollide;
            }
        }

        if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster monster)
        {
            return didCollide;
        }

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
        var firer = this.theOneWhoFiredMe.Get(Game1.currentLocation) as Farmer ?? Game1.player;
        var damage = (int)(this.damageToFarmer.Value * multiplier);
        var knockback = this.WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>() * (1f + firer.knockbackModifier) *
                        multiplier;
        var crate = ModEntry.ArsenalConfig?.Value<bool?>("EnableSlingshotCrits") == true
            ? (0.05f + (0.046f * this.WhatFiredMe.GetEnchantmentLevel<AmethystEnchantment>())) * (1f + firer.critChanceModifier)
            : 0;
        var cpower = (1f +
                      ((ModEntry.ArsenalConfig?.Value<bool?>("RebalanceEnchants") == true
                           ? 0.5f
                           : 0.1f) *
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
        return didCollide;
    }
}
