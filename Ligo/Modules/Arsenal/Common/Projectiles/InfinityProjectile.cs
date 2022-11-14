namespace DaLion.Ligo.Modules.Arsenal.Common.Projectiles;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A beam of energy fired by <see cref="MeleeWeapon"/>s with the <see cref="InfinityEnchantment"/>.</summary>
internal sealed class InfinityProjectile : BasicProjectile
{
    private static readonly Color[] InfinityColors =
    {
        new(255, 69, 182),
        new(209, 69, 182),
        new(178, 69, 247),
        new(136, 69, 247),
        new(122, 69, 247),
    };

    private int _index;

    /// <summary>Initializes a new instance of the <see cref="InfinityProjectile"/> class.</summary>
    /// <param name="source">The <see cref="MeleeWeapon"/> which fired this projectile.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotation">The projectile's starting rotation.</param>
    public InfinityProjectile(
        MeleeWeapon source,
        Farmer firer,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotation)
        : base(
            1,
            Constants.InfinityBeamIndex,
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
        this.Damage = (int)(Math.Ceiling(source.minDamage.Value / 4f) *
                      (1f + (source.GetEnchantmentLevel<RubyEnchantment>() * 0.1f) + source.Read<float>(DataFields.ResonantDamage)) *
                      (1f + firer.attackIncreaseModifier));
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

        ModEntry.Reflector
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

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        var result = base.update(time, location);
        this._index = ++this._index % 5;
        this.color.Value = InfinityColors[this._index];
        return result;
    }

    /// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    public void ExplosionAnimation(GameLocation location)
    {
        //location.temporarySprites.Add(
        //    new TemporaryAnimatedSprite(
        //        $"{ModEntry.Manifest.UniqueID}/InfinityCollisionAnimation",
        //        new Rectangle(0, 0, 128, 128),
        //        50f,
        //        5,
        //        1,
        //        this.position,
        //        false,
        //        Game1.random.NextBool()));
    }
}
