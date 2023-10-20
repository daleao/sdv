namespace DaLion.Overhaul.Modules.Combat.Projectiles;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An <see cref="SObject"/> fired by a <see cref="Slingshot"/>.</summary>
internal sealed class RunaanProjectile : BasicProjectile
{
    private readonly Action<BasicProjectile, GameLocation> _explosionAnimation = Reflector
        .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(typeof(BasicProjectile), "explosionAnimation");

    private readonly float _finalSpeed;
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="RunaanProjectile"/> class.</summary>
    /// <param name="ammo">The <see cref="SObject"/> that was fired.</param>
    /// <param name="index">The index of the fired ammo (this may be different from the index of the <see cref="SObject"/>).</param>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="finalSpeed">The projectile's speed once it starts moving.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public RunaanProjectile(
        Item ammo,
        int index,
        Slingshot source,
        Farmer firer,
        float overcharge,
        Vector2 startingPosition,
        float finalSpeed,
        float rotationVelocity)
        : base(
            1,
            index,
            0,
            0,
            rotationVelocity,
            0f,
            0f,
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
            source.Get_EffectiveDamageModifier() * (1f + firer.attackIncreaseModifier) * overcharge * 0.4f);
        this.Knockback = source.Get_EffectiveKnockback() * (1f + firer.knockbackModifier) * overcharge;
        this.CritChance = 0.025f;
        this.CritPower = 1.5f;
        if (CombatModule.Config.EnableRangedCriticalHits)
        {
            this.CritChance += source.Get_EffectiveCritChance();
            this.CritChance *= 1f + firer.critChanceModifier;
            this.CritPower += source.Get_EffectiveCritPower();
            this.CritPower *= 1f + firer.critPowerModifier;
        }

        if (overcharge > 1f)
        {
            this.Damage = (int)(this.Damage * overcharge);
            this.Knockback *= overcharge;
            this.xVelocity.Value *= overcharge;
            this.yVelocity.Value *= overcharge;
            this.tailLength.Value = (int)((overcharge - 1f) * 5f);
        }

        var isSquishy = this.Ammo.Category is (int)ObjectCategory.Eggs or (int)ObjectCategory.Fruits
                             or (int)ObjectCategory.Vegetables ||
                         this.Ammo.ParentSheetIndex == ObjectIds.Slime;
        if (isSquishy)
        {
            Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "slimedead";
        }

        this.color.Value *= 0.5f;
        this._finalSpeed = finalSpeed;
        this._timer = 750 + (Game1.random.Next(-25, 26) * 10); // delay before motion
        this.ignoreTravelGracePeriod.Value = CombatModule.Config.RemoveSlingshotGracePeriod;
    }

    public Item Ammo { get; }

    public Farmer Firer { get; }

    public Slingshot Source { get; }

    public int Damage { get; private set; }

    public float Knockback { get; private set; }

    public float CritChance { get; private set; }

    public float CritPower { get; private set; }

    public int TileSheetIndex => this.currentTileSheetIndex.Value;

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
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

        this._explosionAnimation(this, location);
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
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        if (this._timer > 0)
        {
            this._timer -= time.ElapsedGameTime.Milliseconds;
            if (this._timer <= 0)
            {
                var targets = location.characters.OfType<Monster>().ToList();

                var target = this.position.Value.GetClosest(targets, monster => monster.Position, out _);
                var targetDirection = target?.GetBoundingBox().Center.ToVector2() - this.position.Value - new Vector2(32f, 32f) ??
                                      Utility.getVelocityTowardPoint(
                                          this.position.Value,
                                          this.Source.AdjustForHeight(this.Source.aimPos.Value.ToVector2()),
                                          this._finalSpeed);
                targetDirection.Normalize();

                var velocity = targetDirection * this._finalSpeed;
                this.xVelocity.Value = velocity.X;
                this.yVelocity.Value = velocity.Y;
            }
        }

        var didCollide = base.update(time, location);
        if (!didCollide && this._timer <= 0)
        {
            var boundingBox = this.getBoundingBox();
            var tilePosition = new Vector2(boundingBox.Center.X / Game1.tileSize, boundingBox.Center.Y / Game1.tileSize);
            if (location.Objects.TryGetValue(tilePosition, out var @object))
            {
                didCollide = this.BehaviorOnCollisionWithObject(location, @object);
            }
        }

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
            return true; // if the container did not break, the projectile is stopped
        }

        location.Objects.Remove(@object.TileLocation);
        return false; // if the container broke, then the projectile lives on
    }
}
