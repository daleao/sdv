namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Projectiles;

#region using directives

using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A harmless ball of snow fired by a <see cref="Slingshot"/>.</summary>
internal sealed class SnowballProjectile : BasicProjectile
{
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
            string.Empty,
            false,
            false,
            firer.currentLocation,
            firer)
    {
        this.Overcharge = overcharge;
        this.startingScale.Value *= overcharge;
        if (ModEntry.Config.Arsenal.Slingshots.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public float Overcharge { get; }

    /// <summary>Replaces BasicProjectile.explosionAnimation.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    public void ExplosionAnimation(GameLocation location)
    {
        location.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                $"{ModEntry.Manifest.UniqueID}/SnowballCollisionAnimation",
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
}
