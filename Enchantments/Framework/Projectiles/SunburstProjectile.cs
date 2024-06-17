namespace DaLion.Enchantments.Framework.Projectiles;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Enchantments.Framework.Enchantments;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A sunlight projectile fired by a <see cref="MeleeWeapon"/> which has the <see cref="SunburstEnchantment"/>.</summary>
internal sealed class SunburstProjectile : BasicProjectile
{
    /// <summary>Initializes a new instance of the <see cref="SunburstProjectile"/> class.</summary>
    /// <param name="firer">The <see cref="Farmer"/> who fired this projectile.</param>
    /// <param name="weapon">The <see cref="MeleeWeapon"/> from which this projectile was fired.</param>
    /// <param name="origin">The projectile's starting position.</param>
    /// <param name="velocity">The projectile's starting velocity.</param>
    /// <param name="rotationVelocity">The projectile's starting rotational velocity.</param>
    public SunburstProjectile(
        Farmer firer,
        MeleeWeapon weapon,
        Vector2 origin,
        Vector2 velocity,
        float rotationVelocity)
        : base(
            weapon.minDamage.Value,
            11,
            0,
            1,
            rotationVelocity * ((float)Math.PI / 180f),
            velocity.X,
            velocity.Y,
            origin,
            null,
            null,
            null,
            explode: false,
            damagesMonsters: true,
            firer.currentLocation,
            firer)
    {
        this.ignoreTravelGracePeriod.Value = true;
        this.ignoreMeleeAttacks.Value = true;
        this.maxTravelDistance.Value = 384;
        this.height.Value = 32f;
    }

    /// <inheritdoc />
    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (n is not Monster m)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        if (m is Ghost or Skeleton or Mummy or ShadowBrute or ShadowShaman or ShadowGirl or ShadowGuy or Shooter)
        {
            this.damageToFarmer.Value *= 4;
            base.behaviorOnCollisionWithMonster(n, location);
            m.Blind(10000);
        }
        else
        {
            this.damageToFarmer.Value = (int)Math.Ceiling(this.damageToFarmer.Value / 4f);
            base.behaviorOnCollisionWithMonster(n, location);
            m.Blind(2500);
        }
    }
}
