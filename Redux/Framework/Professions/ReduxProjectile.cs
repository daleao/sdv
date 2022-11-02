namespace DaLion.Redux.Framework.Professions;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Redux.Framework.Professions.Ultimates;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> with extra useful properties.</summary>
internal sealed class ReduxProjectile : BasicProjectile
{
    public ReduxProjectile(
        Item? ammo,
        Slingshot source,
        Farmer firer,
        GameLocation? location,
        float damage,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        bool canBeRecovered)
        : base(
            (int)damage,
            ammo?.ParentSheetIndex ?? 14, // quincy
            0,
            ammo is null ? 5 : 0, // quincy
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            ammo?.ParentSheetIndex == Constants.ExplosiveAmmoIndex ? "explosion" : "hammer",
            ammo is null ? "debuffSpell" : string.Empty, // quincy
            false,
            true,
            location,
            firer,
            ammo is not null, // quincy
            ammo?.ParentSheetIndex == Constants.ExplosiveAmmoIndex ? explodeOnImpact : null)
    {
        this.Ammo = ammo;
        this.Source = source;
        this.Firer = firer;
        this.Damage = (int)(this.damageToFarmer.Value *
                            (1f + (source.GetEnchantmentLevel<RubyEnchantment>() * 0.1f) +
                             source.Read<float>(DataFields.ResonantSlingshotDamage)) *
                            (1f + firer.attackIncreaseModifier));
        this.Overcharge = overcharge;
        this.Knockback = this.spriteFromObjectSheet.Value
            ? 0f
            : (0.75f + (source.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f) +
               source.Read<float>(DataFields.ResonantSlingshotKnockback)) * (1f + firer.knockbackModifier);

        var canCrit = ModEntry.Config.Arsenal.Slingshots.AllowCrits && !this.spriteFromObjectSheet.Value;
        this.CritChance = canCrit
            ? ((1f / 32f) + (source.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f) +
               source.Read<float>(DataFields.ResonantSlingshotCritChance)) * (1f + firer.critChanceModifier)
            : 0f;
        this.CritPower = canCrit
            ? (1f + ((ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f) *
                     source.GetEnchantmentLevel<JadeEnchantment>()) +
               source.Read<float>(DataFields.ResonantSlingshotCritPower)) * (1f + firer.critPowerModifier)
            : 0f;

        this.CanBeRecovered = ammo is not null && canBeRecovered;

        if (ammo is null) // quincy
        {
            ModEntry.Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "debuffHit";
        }
        else if (this.IsSquishy())
        {
            ModEntry.Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "slimedead";
        }

        if (firer.HasProfession(Profession.Rascal) && !this.IsSquishy() && ModEntry.Config.Professions.ModKey.IsDown())
        {
            ++this.bouncesLeft.Value;
            this.Damage = (int)(this.Damage * 0.6f);
        }

        if (ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.Slingshots.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public Item? Ammo { get; }

    public Farmer Firer { get; }

    public Slingshot Source { get; }

    public int Damage { get; }

    public float Overcharge { get; }

    public float Knockback { get; }

    public float CritChance { get; }

    public float CritPower { get; }

    public bool DidBounce { get; set; }

    public bool DidPierce { get; set; }

    public bool CanBeRecovered { get; set; }

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

        if (this.Ammo is SObject { ParentSheetIndex: Constants.SlimeIndex })
        {
            // piper slime
            if (monster.IsSlime())
            {
                // heal if slime
                var amount = Game1.random.Next(this.Damage - 2, this.Damage + 2);
                monster.Health = Math.Min(monster.Health + amount, monster.MaxHealth);
                location.debris.Add(new Debris(
                    amount,
                    new Vector2(monster.getStandingX() + 8, monster.getStandingY()),
                    Color.Lime,
                    1f,
                    monster));
                Game1.playSound("healSound");
                ModEntry.Reflector
                    .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                    .Invoke(this, location);
            }
            else
            {
                // debuff if not
                location.damageMonster(
                    monster.GetBoundingBox(),
                    this.Damage,
                    this.Damage + 1,
                    false,
                    this.Knockback,
                    0,
                    this.CritChance,
                    this.CritPower,
                    true,
                    this.Firer);
                if (!monster.CanBeSlowed() || !(Game1.random.NextDouble() < 2d / 3d))
                {
                    return;
                }

                monster.Get_SlowIntensity().Value = 2;
                monster.Get_SlowTimer().Value = 5123 + (Game1.random.Next(-2, 3) * 456);
                monster.Set_Slower(this.Firer);
            }

            return;
        }

        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            false,
            this.Knockback,
            0,
            this.CritChance,
            this.CritPower,
            true,
            this.Firer);

        // check for piercing
        if (!this.IsSquishy() && Game1.random.NextDouble() < (this.Overcharge - 1f) / 2f)
        {
            this.DidPierce = true;
        }
        else
        {
            ModEntry.Reflector
                .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                .Invoke(this, location);
        }

        // check for stun
        if (!this.DidPierce && this.Firer.professions.Contains(Farmer.scout + 100) && this.DidBounce)
        {
            monster.stunTime = 5000;
        }

        // increment Desperado ultimate meter
        if (this.Firer.IsLocalPlayer && this.Firer.Get_Ultimate() is DeathBlossom { IsActive: false } blossom &&
            ModEntry.Config.Professions.EnableSpecials)
        {
            blossom.ChargeValue += (this.DidBounce || this.DidPierce ? 18 : 12) -
                                   (10 * this.Firer.health / this.Firer.maxHealth);
        }
    }

    public override bool update(GameTime time, GameLocation location)
    {
        var didCollide = base.update(time, location);

        if (!this.damagesMonsters.Value || this.Overcharge <= 1f ||
            this.travelDistance < this.maxTravelDistance.Value || this.Ammo is null)
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
        var adjustedDamage = (int)(this.Damage * multiplier);
        location.damageMonster(
            monster.GetBoundingBox(),
            adjustedDamage,
            adjustedDamage + 1,
            false,
            this.Knockback,
            0,
            this.CritChance,
            this.CritPower,
            true,
            this.Firer);
        return didCollide;
    }

    /// <summary>Determines whether the projectile should pierce and bounce or make squishy noises upon collision.</summary>
    /// <returns><see langword="true"/> if the projectile is an egg, fruit, vegetable or slime, otherwise <see langword="false"/>.</returns>
    public bool IsSquishy()
    {
        return this.Ammo?.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory ||
               this.Ammo?.ParentSheetIndex == Constants.SlimeIndex;
    }
}
