namespace DaLion.Overhaul.Modules.Arsenal.Projectiles;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An <see cref="SObject"/> fired by a <see cref="Slingshot"/>.</summary>
internal sealed class ObjectProjectile : BasicProjectile
{
    private int _pierceCount;

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <remarks>Required for multiplayer syncing.</remarks>
    public ObjectProjectile()
        : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <param name="ammo">The <see cref="SObject"/> that was fired.</param>
    /// <param name="index">The index of the fired ammo (this may be different from the index of the <see cref="SObject"/>).</param>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="damage">The un-mitigated damage this projectile will cause.</param>
    /// <param name="knockback">The knockback this projectile will cause.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    /// <param name="canRecover">Whether the fired <paramref name="ammo"/> can be recovered.</param>
    public ObjectProjectile(
        Item ammo,
        int index,
        Slingshot source,
        Farmer firer,
        float damage,
        float knockback,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        bool canRecover)
        : base(
            (int)damage,
            index,
            0,
            0,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            ammo.ParentSheetIndex == Constants.ExplosiveAmmoIndex ? "explosion" : "hammer",
            string.Empty,
            ammo.ParentSheetIndex == Constants.ExplosiveAmmoIndex,
            true,
            firer.currentLocation,
            firer,
            true,
            ammo.ParentSheetIndex == Constants.ExplosiveAmmoIndex ? explodeOnImpact : null)
    {
        this.Ammo = ammo;
        this.Source = source;
        this.Firer = firer;
        this.Overcharge = overcharge;
        this.Damage = (int)(this.damageToFarmer.Value * source.Get_EffectiveDamageModifier() * (1f + firer.attackIncreaseModifier) * overcharge);
        this.Knockback = knockback * source.Get_EffectiveKnockbackModifer() * (1f + firer.knockbackModifier) * overcharge;

        var canCrit = ArsenalModule.Config.Slingshots.EnableCrits;
        this.CritChance = canCrit
            ? 0.025f * source.Get_EffectiveCritChanceModifier() * (1f + firer.critChanceModifier)
            : 0f;
        this.CritPower = canCrit
            ? 2f * source.Get_EffectiveCritPowerModifier() * (1f + firer.critPowerModifier)
            : 0f;

        this.CanBeRecovered = canRecover && !this.IsSquishy() && ammo.ParentSheetIndex != Constants.ExplosiveAmmoIndex;
        this.CanPierce = !this.IsSquishy() && ammo.ParentSheetIndex != Constants.ExplosiveAmmoIndex;
        if (this.IsSquishy())
        {
            Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "slimedead";
        }

        if (firer.professions.Contains(Farmer.scout) && !this.IsSquishy() && ProfessionsModule.Config.ModKey.IsDown() && !source.CanAutoFire())
        {
            this.Damage = (int)(this.Damage * 0.8f);
            this.Knockback *= 0.8f;
            this.Overcharge *= 0.8f;
            this.bouncesLeft.Value++;
        }

        if (ArsenalModule.Config.Slingshots.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public Item? Ammo { get; }

    public Farmer? Firer { get; }

    public Slingshot? Source { get; }

    public int Damage { get; private set; }

    public float Overcharge { get; private set; }

    public float Knockback { get; private set; }

    public float CritChance { get; }

    public float CritPower { get; }

    public bool DidBounce { get; private set; }

    public bool CanPierce { get; }

    public bool CanBeRecovered { get; }

    public bool DidPierce { get; private set; }

    public int Index => this.currentTileSheetIndex.Value;

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (this.Ammo is null || this.Firer is null || this.Source is null || n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        if (this.Ammo.ParentSheetIndex == Constants.SlimeIndex)
        {
            if (monster.IsSlime())
            {
                if (!this.Firer.professions.Contains(Farmer.acrobat))
                {
                    return;
                }

                // do heal Slime
                var amount = Game1.random.Next(this.Damage - 2, this.Damage + 2);
                monster.Health = Math.Min(monster.Health + amount, monster.MaxHealth);
                location.debris.Add(new Debris(
                    amount,
                    new Vector2(monster.getStandingX() + 8, monster.getStandingY()),
                    Color.Lime,
                    1f,
                    monster));
                //Game1.playSound("healSound");
                Reflector
                    .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                    .Invoke(this, location);

                return;
            }

            if (monster.CanBeSlowed() && Game1.random.NextDouble() < 2d / 3d)
            {
                // do debuff
                monster.Get_SlowIntensity().Value = 2;
                monster.Get_SlowTimer().Value = 5123 + (Game1.random.Next(-2, 3) * 456);
                monster.Set_Slower(this.Firer);
            }
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

        if (!ProfessionsModule.IsEnabled)
        {
            Reflector
                .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                .Invoke(this, location);
            return;
        }

        if (!this.Firer.professions.Contains(Farmer.desperado))
        {
            Reflector
                .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                .Invoke(this, location);
            return;
        }

        // check for piercing
        if (this.Firer.professions.Contains(Farmer.desperado + 100) && this.CanPierce &&
            Game1.random.NextDouble() < this.Overcharge - 1f)
        {
            this.Damage = (int)(this.Damage * 0.8f);
            this.Knockback *= 0.8f;
            this.Overcharge *= 0.8f;
            this.DidPierce = true;
            this._pierceCount++;
        }
        else
        {
            Reflector
                .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
                .Invoke(this, location);
        }

        // check for stun
        //if (this.Firer.professions.Contains(Farmer.scout + 100) && this.DidBounce)
        //{
        //    monster.stunTime = 2000;
        //}

        // increment Desperado ultimate meter
        if (this.Firer.IsLocalPlayer && this.Firer.Get_Ultimate() is DeathBlossom { IsActive: false } blossom &&
            ProfessionsModule.Config.EnableSpecials)
        {
            blossom.ChargeValue += (this.DidBounce || this.DidPierce ? 18 : 12) -
                                   (10 * this.Firer.health / this.Firer.maxHealth);
        }
    }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithOther(GameLocation location)
    {
        base.behaviorOnCollisionWithOther(location);
        if (this.Ammo is null || this.Firer is null || this.Source is null || !ProfessionsModule.IsEnabled)
        {
            return;
        }

        if (this.Ammo.ParentSheetIndex == Constants.SlimeIndex &&
            this.Firer.Get_Ultimate() is Concerto { IsActive: false } concerto)
        {
            concerto.ChargeValue += Game1.random.Next(5);
            return;
        }

        if (!this.CanBeRecovered || !this.Firer.professions.Contains(Farmer.scout))
        {
            return;
        }

        // try to recover
        var chance = this.Ammo.ParentSheetIndex is SObject.wood or SObject.coal ? 0.175 : 0.35;
        if (this.Firer.professions.Contains(Farmer.scout + 100))
        {
            chance *= 2d;
        }

        chance -= this._pierceCount * 0.2;
        if (chance < 0d || Game1.random.NextDouble() > chance)
        {
            return;
        }

        location.debris.Add(
            new Debris(
                this.Ammo.ParentSheetIndex,
                new Vector2((int)this.position.X, (int)this.position.Y),
                this.Firer.getStandingPosition()));
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        if (this.Ammo is null || this.Firer is null || this.Source is null)
        {
            return base.update(time, location);
        }

        var bounces = this.bouncesLeft.Value;
        var didCollide = base.update(time, location);
        if (bounces > this.bouncesLeft.Value)
        {
            this.DidBounce = true;
        }

        if (this.Overcharge <= 1f || (this.maxTravelDistance.Value > 0 && this.travelDistance >= this.maxTravelDistance.Value))
        {
            return didCollide;
        }

        // check if already collided
        if (didCollide)
        {
            return !this.DidPierce && didCollide;
        }

        this.DidPierce = false;

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

        // deal damage
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
