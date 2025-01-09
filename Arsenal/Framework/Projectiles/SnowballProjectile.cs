namespace DaLion.Arsenal.Framework.Projectiles;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A harmless ball of snow fired by a <see cref="Slingshot"/>.</summary>
internal sealed class SnowballProjectile : BasicProjectile
{
    /// <summary>Initializes a new instance of the <see cref="SnowballProjectile"/> class.</summary>
    /// <remarks>Explicit parameterless constructor is required for multiplayer synchronization.</remarks>
    public SnowballProjectile()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SnowballProjectile"/> class.</summary>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="overcharge">The amount of overcharge with which the projectile was fired.</param>
    /// <param name="startingPosition">The projectile's starting position.</param>
    /// <param name="xVelocity">The projectile's starting velocity in the horizontal direction.</param>
    /// <param name="yVelocity">The projectile's starting velocity in the vertical direction.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public SnowballProjectile(
        Farmer firer,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity)
        : base(
            1,
            snowBall,
            0,
            0,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            "snowyStep",
            damagesMonsters: true)
    {
        this.Overcharge = overcharge;
        this.startingScale.Value *= overcharge;
    }

    public float Overcharge { get; }

    /// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    public void ExplosionAnimation(GameLocation location)
    {
        location.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                $"{Manifest.UniqueID}_SnowballCollisionAnimation",
                new Rectangle(0, 0, 64, 64),
                50f,
                10,
                1,
                this.position,
                false,
                Game1.random.NextBool())
            {
                scale = this.Overcharge,
            });
    }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        base.behaviorOnCollisionWithMonster(n, location);
        if (n is Monster { Health: > 0 } monster && Game1.random.NextDouble() < 0.1)
        {
            monster.Chill();
        }
    }

    public override void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player)
    {
        base.behaviorOnCollisionWithPlayer(location, player);
        if (Game1.random.NextDouble() < 0.1)
        {
            // chill the player
        }
    }

    /// <inheritdoc />
    public override bool update(GameTime time, GameLocation location)
    {
        var didCollide = base.update(time, location);
        if (didCollide)
        {
            return true;
        }

        var boundingBox = this.getBoundingBox();
        var tilePosition = new Vector2(boundingBox.Center.X / Game1.tileSize, boundingBox.Center.Y / Game1.tileSize);
        if (location.Objects.TryGetValue(tilePosition, out var @object))
        {
            didCollide = this.BehaviorOnCollisionWithObject(location, @object);
        }

        return didCollide;
    }

    internal bool BehaviorOnCollisionWithObject(GameLocation location, SObject @object)
    {
        if (@object is not BreakableContainer container)
        {
            return false; // if the object is not a container, the projectile continues on its way
        }

        var dummyWeapon = new MeleeWeapon();
        Reflector.GetUnboundFieldSetter<Tool, Farmer>("lastUser")
            .Invoke(dummyWeapon, this.theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player);
        if (!container.performToolAction(dummyWeapon))
        {
            return true; // if the container did not break, the projectile is stopped
        }

        location.Objects.Remove(@object.TileLocation);
        return false; // if the container broke, then the projectile lives on
    }
}
