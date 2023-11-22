namespace DaLion.Overhaul.Modules.Combat.Projectiles;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>An energy projectile fired by a <see cref="Slingshot"/> which has the <see cref="QuincyEnchantment"/>.</summary>
internal sealed class QuincyProjectile : BasicProjectile
{
    public const int BlueTileSheetIndex = 14;
    public const int YellowTileSheetIndex = 17;
    public const int RedTileSheetIndex = 18;

    /// <summary>Initializes a new instance of the <see cref="QuincyProjectile"/> class.</summary>
    /// <remarks>Explicit parameterless constructor is required for multiplayer synchronization.</remarks>
    public QuincyProjectile()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QuincyProjectile"/> class.</summary>
    /// <param name="source">The <see cref="Slingshot"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public QuincyProjectile(
        Slingshot source,
        Farmer firer,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity)
        : base(
            1,
            BlueTileSheetIndex,
            0,
            5,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            "debuffHit",
            string.Empty,
            false,
            true,
            firer.currentLocation,
            firer)
    {
        this.Firer = firer;

        var ammoDamage = 30;
        if (firer.health < firer.maxHealth * 1f / 3f)
        {
            this.currentTileSheetIndex.Value = RedTileSheetIndex;
            ammoDamage *= 3;
        }
        else if (firer.health < firer.maxHealth * 2f / 3f)
        {
            this.currentTileSheetIndex.Value = YellowTileSheetIndex;
            ammoDamage *= 2;
        }

        this.Damage = (int)((ammoDamage + Game1.random.Next(-ammoDamage / 2, ammoDamage + 2)) *
            source.Get_EffectiveDamageModifier() * (1f + firer.attackIncreaseModifier) * overcharge);
        this.Overcharge = overcharge;
        this.startingScale.Value *= overcharge * overcharge;
        this.IgnoreLocationCollision = true;
        this.ignoreTravelGracePeriod.Value = CombatModule.Config.RemoveSlingshotGracePeriod;
    }

    public Farmer Firer { get; } = null!;

    public int Damage { get; private set; }

    public float Overcharge { get; }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        DeathBlossom? blossom = null;
        if (ProfessionsModule.ShouldEnable)
        {
            // check ultimate
            if (this.Firer.IsLocalPlayer && ProfessionsModule.Config.EnableLimitBreaks)
            {
                blossom = this.Firer.Get_Ultimate() as DeathBlossom;
            }

            // check for quick shot
            if (ProfessionsModule.State.LastDesperadoTarget is not null &&
                monster != ProfessionsModule.State.LastDesperadoTarget)
            {
                Log.D("Did quick shot!");
                this.Damage = (int)(this.Damage * 1.5f);
                if (blossom is { IsActive: false })
                {
                    blossom.ChargeValue += 12d;
                }
            }
        }

        Reflector.GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
            .Invoke(this, location);
        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            false,
            0f,
            0,
            0f,
            0f,
            true,
            this.Firer);

        if (!this.Firer.professions.Contains(Farmer.desperado))
        {
            return;
        }

        ProfessionsModule.State.LastDesperadoTarget = monster;

        // increment ultimate meter
        if (blossom is { IsActive: false } && this.Overcharge >= 1f)
        {
            blossom.ChargeValue += this.Overcharge * 8d;
        }
    }

    ///// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    ///// <param name="location">The <see cref="GameLocation"/>.</param>
    //public void ExplosionAnimation(GameLocation location)
    //{
    //    location.temporarySprites.Add(
    //        new TemporaryAnimatedSprite(
    //            $"{Manifest.UniqueID}/QuincyCollisionAnimation",
    //            new Rectangle(0, 0, 64, 64),
    //            50f,
    //            1,
    //            1,
    //            this.position,
    //            false,
    //            Game1.random.NextBool())
    //        {
    //            scale = this.Overcharge,
    //        });
    //}
}
