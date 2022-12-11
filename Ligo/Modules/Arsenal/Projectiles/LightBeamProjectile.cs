namespace DaLion.Ligo.Modules.Arsenal.Projectiles;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A beam of energy fired by <see cref="MeleeWeapon"/>s with the <see cref="InfinityEnchantment"/>.</summary>
internal sealed class LightBeamProjectile : BasicProjectile
{
    public const int TileSheetIndex = 11;

    /// <summary>Initializes a new instance of the <see cref="LightBeamProjectile"/> class.</summary>
    /// <param name="source">The <see cref="MeleeWeapon"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotation">The projectile's starting rotation.</param>
    public LightBeamProjectile(
        MeleeWeapon source,
        Farmer firer,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotation)
        : base(
            1,
            TileSheetIndex,
            0,
            3,
            0f,
            xVelocity,
            yVelocity,
            startingPosition,
            string.Empty,
            string.Empty,
            false,
            true,
            firer.currentLocation,
            firer)
    {
        this.Firer = firer;
        this.Damage = (int)(source.Get_MinDamage() * (1f + firer.attackIncreaseModifier) / 4f);
        this.rotation = rotation;
        this.ignoreTravelGracePeriod.Value = true;
        this.ignoreMeleeAttacks.Value = true;
        this.maxTravelDistance.Value = 256;
        this.height.Value = 32f;
    }

    public Farmer Firer { get; }

    public int Damage { get; }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        Reflector
            .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
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
    }

    /// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    public void ExplosionAnimation(GameLocation location)
    {
        //location.temporarySprites.Add(
        //    new TemporaryAnimatedSprite(
        //        $"{Manifest.UniqueID}/LightBeamCollisionAnimation",
        //        new Rectangle(0, 0, 128, 128),
        //        50f,
        //        5,
        //        1,
        //        this.position,
        //        false,
        //        Game1.random.NextBool()));
    }
}
