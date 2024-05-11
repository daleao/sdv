namespace DaLion.Professions.Framework.Projectiles;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A <see cref="BasicProjectile"/> with extra useful properties.</summary>
internal sealed class ObjectProjectile : BasicProjectile
{
    private readonly Action<BasicProjectile, GameLocation> _explosionAnimation = Reflector
        .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(typeof(BasicProjectile), "explosionAnimation");

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <remarks>Explicit parameterless constructor is required for multiplayer synchronization.</remarks>
    public ObjectProjectile()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <param name="ammo">The <see cref="SObject"/> that was fired, if any.</param>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="damage">The un-mitigated damage this projectile will cause.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    /// <param name="isMusked">Whether the projectile is laced with Monster Musk.</param>
    public ObjectProjectile(
        SObject ammo,
        Slingshot source,
        Farmer firer,
        float damage,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        bool isMusked = false)
        : base(
            (int)damage,
            -1,
            0,
            0,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            source.GetAmmoCollisionSound(ammo),
            null,
            null,
            false,
            true,
            firer.currentLocation,
            firer,
            source.GetAmmoCollisionBehavior(ammo),
            ammo.ItemId)
    {
        this.Ammo = ammo;
        this.Source = source;
        this.Firer = firer;
        this.Damage = (int)(this.damageToFarmer.Value * (1f + firer.buffs.AttackMultiplier));
        this.Overcharge = overcharge;
        if (overcharge > 1f)
        {
            this.Damage = (int)(this.Damage * overcharge);
            this.xVelocity.Value *= overcharge;
            this.yVelocity.Value *= overcharge;
            this.tailLength.Value = (int)((overcharge - 1f) * 5f);
        }

        this.IsSquishyOrExplosive =
            this.Ammo.Category is (int)ObjectCategory.Eggs or (int)ObjectCategory.Fruits or (int)ObjectCategory.Vegetables ||
            this.Ammo.QualifiedItemId is QualifiedObjectIds.Slime or QualifiedObjectIds.ExplosiveAmmo;
        this.HasMonsterMusk = isMusked;
        if (this.Ammo.QualifiedItemId is QualifiedObjectIds.Slime)
        {
            this.localScale = 0.75f;
        }

        if (!this.IsSquishyOrExplosive && firer.HasProfession(Profession.Desperado, true) && overcharge <= 1f)
        {
            this.bouncesLeft.Value++;
        }
    }

    public Item Ammo { get; } = null!;

    public Farmer Firer { get; } = null!;

    public Slingshot Source { get; } = null!;

    public int Damage { get; private set; }

    public float Overcharge { get; private set; }

    public bool IsSquishyOrExplosive { get; }

    public bool HasMonsterMusk { get; }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        if (this.Ammo.QualifiedItemId != QualifiedObjectIds.ExplosiveAmmo)
        {
            if (this.HasMonsterMusk)
            {
                monster.Set_Musked(15);
            }
        }

        if (this.Ammo.QualifiedItemId == QualifiedObjectIds.Slime)
        {
            if (monster.IsSlime())
            {
                if (!this.Firer.HasProfession(Profession.Piper))
                {
                    return;
                }

                // do heal Slime
                var amount = Game1.random.Next(this.Damage - 2, this.Damage + 2);
                monster.Health = Math.Min(monster.Health + amount, monster.MaxHealth);
                location.debris.Add(new Debris(
                    amount,
                    new Vector2(monster.StandingPixel.X + 8, monster.StandingPixel.Y),
                    Color.Lime,
                    1f,
                    monster));
                //Game1.playSound("healSound");
                this._explosionAnimation(this, location);
                return;
            }

            if (!monster.IsSlime() && monster is not Ghost && Game1.random.NextBool(2d / 3d))
            {
                // do debuff
                monster.Slow(5123 + (Game1.random.Next(-2, 3) * 456),  1f / 3f);
                monster.startGlowing(Color.LimeGreen, false, 0.05f);
            }
        }

        var ogDamage = this.Damage;
        var monsterResistanceModifier = 1f + (monster.resilience.Value / 10f);
        var isTargetArmored = false;
        if (monster is Bug { isArmoredBug.Value: true })
        {
            isTargetArmored = true;
            monsterResistanceModifier *= 2f;
        }

        var inverseResistanceModifer = 1f / monsterResistanceModifier;
        var didPierce = false;

        // check for quick shot
        if (this.Firer.IsLocalPlayer && State.LastDesperadoTarget is not null &&
            monster != State.LastDesperadoTarget)
        {
            Log.D("Did quick shot!");
            this.Damage = (int)(this.Damage * 1.5f);
            if (State.LimitBreak is DesperadoBlossom { IsActive: false } blossom)
            {
                blossom.ChargeValue += 12d;
            }
        }
        // check for pierce, which is mutually exclusive with quick shot
        else if (this.Firer.HasProfession(Profession.Desperado) && !this.IsSquishyOrExplosive &&
                 Game1.random.NextBool((this.Overcharge - 1f) * inverseResistanceModifer))
        {
            Log.D("Pierced!");

            // !!! COMBAT INTERVENTION NEEDED
            this.Damage += monster.resilience.Value; // ignore defense
            if (isTargetArmored)
            {
                ((Bug)monster).isArmoredBug.Value = false;
            }

            didPierce = true;
        }

        this._explosionAnimation(this, location);
        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            this.Ammo.QualifiedItemId == QualifiedObjectIds.ExplosiveAmmo,
            Math.Max(1f, this.Overcharge),
            0,
            0f,
            0f,
            true,
            this.Firer,
            true);

        if (didPierce)
        {
            this.piercesLeft.Value = 1;
            this.Damage = (int)(ogDamage * inverseResistanceModifer);
            this.Overcharge *= inverseResistanceModifer;
            this.xVelocity.Value *= inverseResistanceModifer;
            this.yVelocity.Value *= inverseResistanceModifer;
            if (isTargetArmored)
            {
                ((Bug)monster).isArmoredBug.Value = true;
            }
        }
        else
        {
            this.piercesLeft.Value--;
        }

        // Desperado checks
        if (this.Firer.IsLocalPlayer && this.Firer.HasProfession(Profession.Desperado))
        {
            State.LastDesperadoTarget = monster;
        }

        // increment Limit Break meters
        if (this.Firer.IsLocalPlayer && State.LimitBreak is { IsActive: false } limit)
        {
            switch (limit)
            {
                case DesperadoBlossom when this.Overcharge >= 1f:
                    limit.ChargeValue += this.Overcharge * 8d;
                    break;
                case PiperConcerto when this.Ammo.QualifiedItemId == QualifiedObjectIds.Slime:
                    limit.ChargeValue += Game1.random.Next(8);
                    break;
            }
        }

        if (this.IsSquishyOrExplosive || !this.Firer.HasProfession(Profession.Rascal))
        {
            return;
        }

        // try to recover
        var recoveryChance = this.Firer.HasProfession(Profession.Rascal, true) ? 0.55 : 0.35;
        if (this.Ammo.QualifiedItemId is QualifiedObjectIds.Wood or QualifiedObjectIds.Coal)
        {
            recoveryChance /= 2d;
        }

        if (recoveryChance > 0d && Game1.random.NextBool(recoveryChance))
        {
            location.debris.Add(
                new Debris(
                    this.Ammo.ParentSheetIndex,
                    new Vector2((int)this.position.X, (int)this.position.Y),
                    this.Firer.getStandingPosition()));
        }
    }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithOther(GameLocation location)
    {
        base.behaviorOnCollisionWithOther(location);

        // add musk
        if (this.Ammo.QualifiedItemId != QualifiedObjectIds.ExplosiveAmmo)
        {
            if (this.HasMonsterMusk)
            {
                location.AddMusk(this.position.Value, 15);
            }
        }

        // increment Piper Limit Break meter
        if (this.Ammo.QualifiedItemId == QualifiedObjectIds.Slime && this.Firer.IsLocalPlayer &&
            State.LimitBreak is PiperConcerto { IsActive: false } concerto)
        {
            concerto.ChargeValue += Game1.random.Next(5);
            return;
        }

        if (this.IsSquishyOrExplosive || !this.Firer.HasProfession(Profession.Rascal))
        {
            return;
        }

        // try to recover
        var recoveryChance = this.Firer.HasProfession(Profession.Rascal, true) ? 0.55 : 0.35;
        if (this.Ammo.QualifiedItemId is QualifiedObjectIds.Wood or QualifiedObjectIds.Coal)
        {
            recoveryChance /= 2d;
        }

        if (recoveryChance > 0d && Game1.random.NextBool(recoveryChance))
        {
            location.debris.Add(
                new Debris(
                    this.Ammo.ParentSheetIndex,
                    new Vector2((int)this.position.X, (int)this.position.Y),
                    this.Firer.getStandingPosition()));
        }
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        var bouncesBefore = this.bouncesLeft.Value;
        var piercesBefore = this.piercesLeft.Value;
        var didCollide = base.update(time, location);
        if (this.bouncesLeft.Value < bouncesBefore || this.piercesLeft.Value < piercesBefore)
        {
            this.Damage *= 2;
        }

        //// get trajectory angle
        //var velocity = new Vector2(this.xVelocity.Value, this.yVelocity.Value);
        //var angle = velocity.AngleWithHorizontal();
        //if (angle > 180)
        //{
        //    angle -= 360;
        //}

        //// check for extended collision
        //var originalHitbox = this.getBoundingBox();
        //var newHitbox = new Rectangle(originalHitbox.X, originalHitbox.Y, originalHitbox.Width, originalHitbox.Height);
        //var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
        //if (isBulletTravelingVertically)
        //{
        //    newHitbox.Inflate((int)(originalHitbox.Width * this.Overcharge), 0);
        //    if (newHitbox.Width <= originalHitbox.Width)
        //    {
        //        return didCollide;
        //    }
        //}
        //else
        //{
        //    newHitbox.Inflate(0, (int)(originalHitbox.Height * this.Overcharge));
        //    if (newHitbox.Height <= originalHitbox.Height)
        //    {
        //        return didCollide;
        //    }
        //}

        //if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster { IsMonster: true } monster)
        //{
        //    return didCollide;
        //}

        //// deal damage
        //int actualDistance, monsterRadius, actualBulletRadius, extendedBulletRadius;
        //if (isBulletTravelingVertically)
        //{
        //    actualDistance = Math.Abs(monster.StandingPixel.X - originalHitbox.Center.X);
        //    monsterRadius = monster.GetBoundingBox().Width / 2;
        //    actualBulletRadius = originalHitbox.Width / 2;
        //    extendedBulletRadius = newHitbox.Width / 2;
        //}
        //else
        //{
        //    actualDistance = Math.Abs(monster.StandingPixel.Y - originalHitbox.Center.Y);
        //    monsterRadius = monster.GetBoundingBox().Height / 2;
        //    actualBulletRadius = originalHitbox.Height / 2;
        //    extendedBulletRadius = newHitbox.Height / 2;
        //}

        //var lerpFactor = (actualDistance - (actualBulletRadius + monsterRadius)) /
        //                 (extendedBulletRadius - actualBulletRadius);
        //var multiplier = MathHelper.Lerp(1f, 0f, lerpFactor);
        //var adjustedDamage = (int)(this.Damage * multiplier);
        //location.damageMonster(
        //    monster.GetBoundingBox(),
        //    adjustedDamage,
        //    adjustedDamage + 1,
        //    this.Ammo.QualifiedItemId == QualifiedObjectIds.ExplosiveAmmo,
        //    Math.Max(1f, this.Overcharge),
        //    0,
        //    0f,
        //    1f,
        //    true,
        //    this.Firer,
        //    true);

        return didCollide;
    }
}
