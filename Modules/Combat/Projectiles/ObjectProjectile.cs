namespace DaLion.Overhaul.Modules.Combat.Projectiles;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An <see cref="SObject"/> fired by a <see cref="Slingshot"/>.</summary>
internal sealed class ObjectProjectile : BasicProjectile
{
    private readonly Action<BasicProjectile, GameLocation> _explosionAnimation = Reflector
        .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(typeof(BasicProjectile), "explosionAnimation");

    private int _energizedFrame;

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <remarks>Explicit parameterless constructor is required for multiplayer synchronization.</remarks>
    public ObjectProjectile()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ObjectProjectile"/> class.</summary>
    /// <param name="ammo">The <see cref="SObject"/> that was fired.</param>
    /// <param name="index">The index of the fired ammo (this may be different from the index of the <see cref="SObject"/>).</param>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public ObjectProjectile(
        Item ammo,
        int index,
        Slingshot source,
        Farmer firer,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity)
        : base(
            1,
            index,
            0,
            0,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo ? "explosion" : "hammer",
            string.Empty,
            ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo,
            true,
            firer.currentLocation,
            firer,
            true,
            ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo ? explodeOnImpact : null)
    {
        this.Ammo = ammo;
        if (ammo.ParentSheetIndex is not (ObjectIds.Wood or ObjectIds.Coal or ObjectIds.Stone or ObjectIds.CopperOre
            or ObjectIds.IronOre or ObjectIds.GoldOre or ObjectIds.IridiumOre))
        {
            this.startingScale.Value *= 0.8f;
        }

        this.Source = source;
        this.Firer = firer;

        var ammoDamage = ammo.GetAmmoDamage();
        this.Damage = (int)((ammoDamage + Game1.random.Next(-ammoDamage / 2, ammoDamage + 2)) *
            source.Get_EffectiveDamageModifier() * (1f + firer.attackIncreaseModifier) * overcharge);
        this.Knockback = source.Get_EffectiveKnockback() * (1f + firer.knockbackModifier) * overcharge;
        this.CritChance = 0f;
        this.CritPower = 0f;
        if (CombatModule.Config.WeaponsSlingshots.EnableRangedCriticalHits)
        {
            this.CritChance += source.Get_EffectiveCritChance();
            this.CritChance *= 1f + firer.critChanceModifier;
            this.CritPower += source.Get_EffectiveCritPower();
            this.CritPower *= 1f + firer.critPowerModifier;
        }

        this.Overcharge = overcharge;
        if (overcharge > 1f)
        {
            this.Damage = (int)(this.Damage * overcharge);
            this.Knockback *= overcharge;
            this.xVelocity.Value *= overcharge;
            this.yVelocity.Value *= overcharge;
            this.tailLength.Value = (int)((overcharge - 1f) * 5f);
        }

        this.IsSquishy = this.Ammo.Category is (int)ObjectCategory.Eggs or (int)ObjectCategory.Fruits
                             or (int)ObjectCategory.Vegetables ||
                         this.Ammo.ParentSheetIndex == ObjectIds.Slime;
        if (this.IsSquishy)
        {
            Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "slimedead";
        }

        this.ignoreTravelGracePeriod.Value = CombatModule.Config.WeaponsSlingshots.RemoveSlingshotGracePeriod;

        if (source.attachments.Length < 2 || source.attachments[1] is not
                { ParentSheetIndex: ObjectIds.MonsterMusk } musk)
        {
            return;
        }

        var uses = musk.Read<int>(Professions.DataKeys.MuskUses);
        if (uses >= 10)
        {
            source.attachments[1] = null;
            return;
        }

        this.Musked = true;
        if (++uses >= 10)
        {
            source.attachments[1] = null;
        }
        else
        {
            musk.Write(Professions.DataKeys.MuskUses, uses.ToString());
        }
    }

    public Item Ammo { get; } = null!;

    public Farmer Firer { get; } = null!;

    public Slingshot Source { get; } = null!;

    public int Damage { get; private set; }

    public float Overcharge { get; private set; }

    public float Knockback { get; private set; }

    public float CritChance { get; private set; }

    public float CritPower { get; private set; }

    public bool DidBounce { get; private set; }

    public bool DidPierce { get; private set; }

    public bool IsSquishy { get; }

    public bool Musked { get; }

    public bool Energized { get; set; }

    public int TileSheetIndex => this.currentTileSheetIndex.Value;

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMineWall(int tileX, int tileY)
    {
        this.DidPierce = false;
        base.behaviorOnCollisionWithMineWall(tileX, tileY);
    }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        if (this.Ammo.ParentSheetIndex != ObjectIds.ExplosiveAmmo)
        {
            if (this.Musked)
            {
                monster.Set_Musked(15);
            }
        }

        if (this.Ammo.ParentSheetIndex == ObjectIds.Slime)
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
                this._explosionAnimation(this, location);
                return;
            }

            if (!monster.IsSlime() && monster is not Ghost && CombatModule.ShouldEnable && Game1.random.NextDouble() < 2d / 3d)
            {
                // do debuff
                monster.Slow(5123 + (Game1.random.Next(-2, 3) * 456), 1f / 3f);
                monster.startGlowing(Color.LimeGreen, false, 0.05f);
            }
        }

        var ogDamage = this.Damage;
        var monsterResistanceModifier = 1f + (monster.resilience.Value / 10f);
        var inverseResistanceModifer = 1f / monsterResistanceModifier;
        IUltimate? ultimate = null;
        if (ProfessionsModule.ShouldEnable)
        {
            // check ultimate
            if (this.Firer.IsLocalPlayer && ProfessionsModule.Config.Limit.EnableLimitBreaks)
            {
                ultimate = this.Firer.Get_Ultimate();
            }

            // check for quick shot
            if (ProfessionsModule.State.LastDesperadoTarget is not null &&
                monster != ProfessionsModule.State.LastDesperadoTarget)
#pragma warning disable SA1513 // Closing brace should be followed by blank line
            {
                Log.D("Did quick shot!");
                this.Damage = (int)(this.Damage * 1.5f);
                if (ultimate is DeathBlossom { IsActive: false })
                {
                    ultimate.ChargeValue += 12d;
                }
            }
            // check for pierce, which is mutually exclusive with quick shot
            else if (this.Firer.professions.Contains(Farmer.desperado + 100) && !this.IsSquishy &&
                     this.Ammo.ParentSheetIndex != ObjectIds.ExplosiveAmmo &&
                     Game1.random.NextDouble() < (this.Overcharge - 1.5f) * inverseResistanceModifer)
            {
                Log.D("Pierced!");
                this.DidPierce = true;
                if (CombatModule.Config.NewResistanceFormula)
                {
                    this.Damage = (int)(this.Damage * monsterResistanceModifier);
                }
                else
                {
                    this.Damage += monster.resilience.Value;
                }
            }
#pragma warning restore SA1513 // Closing brace should be followed by blank line
        }

        this._explosionAnimation(this, location);
        if (this.Energized)
        {
            this.DoLightningStrike(monster, location);
            this.Energized = false;
        }

        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            this.Ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo,
            this.Knockback,
            0,
            this.CritChance,
            this.CritPower,
            true,
            this.Firer);

        this.Damage = ogDamage;
        if (this.DidPierce)
        {
            this.Damage = (int)(this.Damage * inverseResistanceModifer);
            this.Knockback *= inverseResistanceModifer;
            this.Overcharge *= inverseResistanceModifer;
            this.xVelocity.Value *= inverseResistanceModifer;
            this.yVelocity.Value *= inverseResistanceModifer;
        }

        if (!ProfessionsModule.ShouldEnable)
        {
            return;
        }

        // Desperado checks
        if (this.Firer.professions.Contains(Farmer.desperado))
        {
            ProfessionsModule.State.LastDesperadoTarget = monster;
        }

        // increment ultimate meter
        if (ultimate is { IsActive: false })
        {
            switch (ultimate)
            {
                case DeathBlossom when this.Overcharge >= 1f:
                    ultimate.ChargeValue += this.Overcharge * 8d;
                    break;
                case Concerto when this.Ammo.ParentSheetIndex == ObjectIds.Slime:
                    ultimate.ChargeValue += Game1.random.Next(8);
                    break;
            }
        }

        if (this.IsSquishy || this.Ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo ||
            !this.Firer.professions.Contains(Farmer.scout))
        {
            return;
        }

        // Rascal recovery
        var recoveryChance = this.Firer.professions.Contains(Farmer.scout + 100) ? 0.55 : 0.35;
        if (this.Ammo.ParentSheetIndex is ObjectIds.Wood or ObjectIds.Coal)
        {
            recoveryChance /= 2d;
        }

        if (recoveryChance > 0d && Game1.random.NextDouble() < recoveryChance)
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
        this.DidPierce = false;
        base.behaviorOnCollisionWithOther(location);
        if (!ProfessionsModule.ShouldEnable)
        {
            return;
        }

        if (this.Ammo.ParentSheetIndex != ObjectIds.ExplosiveAmmo)
        {
            if (this.Musked)
            {
                location.AddMusk(this.position.Value, 15);
            }
        }

        // increment ultimate meter
        if (this.Ammo.ParentSheetIndex == ObjectIds.Slime &&
            this.Firer.Get_Ultimate() is Concerto { IsActive: false } concerto)
        {
            concerto.ChargeValue += Game1.random.Next(5);
            return;
        }

        if (this.IsSquishy || this.Ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo ||
            !this.Firer.professions.Contains(Farmer.scout))
        {
            return;
        }

        // try to recover
        var recoveryChance = this.Firer.professions.Contains(Farmer.scout + 100) ? 0.55 : 0.35;
        if (this.Ammo.ParentSheetIndex is ObjectIds.Wood or ObjectIds.Coal)
        {
            recoveryChance /= 2d;
        }

        if (recoveryChance > 0d && Game1.random.NextDouble() < recoveryChance)
        {
            location.debris.Add(
                new Debris(
                    this.Ammo.ParentSheetIndex,
                    new Vector2((int)this.position.X, (int)this.position.Y),
                    this.Firer.getStandingPosition()));
        }
    }

    public override void draw(SpriteBatch b)
    {
        base.draw(b);
        if (this.Energized)
        {
            b.Draw(
                Textures.EnergizedTx,
                Game1.GlobalToLocal(
                    Game1.viewport,
                    this.position + new Vector2(0f, 0f - this.height.Value) + new Vector2(32f, 32f)),
                new Rectangle(this._energizedFrame * 32, 0, 32, 32),
                Color.White,
                this.rotation,
                new Vector2(16f, 16f),
                this.localScale * 2f,
                SpriteEffects.None,
                (this.position.Y + 96f) / 10000f);
        }
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        var bounces = this.bouncesLeft.Value;
        var didCollide = base.update(time, location);
        if (!didCollide)
        {
            var boundingBox = this.getBoundingBox();
            var tilePosition = new Vector2(boundingBox.Center.X / Game1.tileSize, boundingBox.Center.Y / Game1.tileSize);
            if (location.Objects.TryGetValue(tilePosition, out var @object))
            {
                didCollide = this.BehaviorOnCollisionWithObject(location, @object);
            }
        }

        if (bounces > this.bouncesLeft.Value)
        {
            this.DidBounce = true;
        }

        if (this.Energized && time.TotalGameTime.Ticks % 5 == 0)
        {
            this._energizedFrame = (this._energizedFrame + 1) % 4;
        }

        if (this.Overcharge <= 1f || (this.maxTravelDistance.Value > 0 && this.travelDistance >= this.maxTravelDistance.Value))
        {
            return didCollide;
        }

        // check if already collided
        if (didCollide)
        {
            return !this.DidPierce;
        }

        this.DidPierce = false;

        // get trajectory angle
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

        if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster { IsMonster: true } monster)
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
            this.Ammo.ParentSheetIndex == ObjectIds.ExplosiveAmmo,
            this.Knockback,
            0,
            this.CritChance,
            this.CritPower,
            true,
            this.Firer);
        return didCollide;
    }

    internal bool BehaviorOnCollisionWithObject(GameLocation location, SObject @object)
    {
        if (@object is not BreakableContainer container)
        {
            return false; // if the object is not a container, the projectile continues on its way
        }

        var dummyWeapon = new MeleeWeapon { BaseName = string.Empty };
        Reflector.GetUnboundFieldSetter<Tool, Farmer>("lastUser").Invoke(dummyWeapon, this.Firer);
        if (!container.performToolAction(dummyWeapon, location))
        {
            if (this.Musked)
            {
                location.AddMusk(this.position.Value, 15);
            }

            return true; // if the container did not break, the projectile is stopped
        }

        location.Objects.Remove(@object.TileLocation);
        return false; // if the container broke, then the projectile lives on
    }

    /// <summary>Trigger a lightning strike on the specified <paramref name="monster"/>'s position.</summary>
    /// <param name="monster">The target <see cref="Monster"/>.</param>
    /// <param name="location">The current <see cref="GameLocation"/>.</param>
    private void DoLightningStrike(Monster monster, GameLocation location)
    {
        var aoe = monster.GetBoundingBox();
        aoe.Inflate(12 * Game1.tileSize, 12 * Game1.tileSize);
        Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
        location.playSound("thunder");
        Utility.drawLightningBolt(monster.Position + new Vector2(32f, 32f), location);
        location.damageMonster(
            aoe,
            this.Damage * 3,
            (this.Damage * 3) + 3,
            false,
            1f,
            0,
            0f,
            1f,
            false,
            this.Firer);
    }
}
