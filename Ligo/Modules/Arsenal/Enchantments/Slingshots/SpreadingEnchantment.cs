namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Ligo.Modules.Arsenal.Projectiles;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>Fire 2 additional projectiles.</summary>
[XmlType("Mods_DaLion_SpreadingEnchantment")]
public sealed class SpreadingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return i18n.Get("enchantments.spreading");
    }

    /// <inheritdoc />
    protected override void _OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        int damageBase,
        float damageMod,
        float knockback,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        GameLocation location,
        Farmer firer)
    {
        var velocity = new Vector2(xVelocity, yVelocity);
        damageBase = (int)(damageBase * 0.6f);
        var overcharge = Config.EnableProfessions && firer.professions.Contains(Farmer.desperado)
            ? slingshot.GetOvercharge(firer)
            : 1f;

        // do clockwise projectile
        this.FireRotatedProjectile(
            15f,
            projectile,
            slingshot,
            firer,
            damageBase,
            damageMod,
            knockback,
            overcharge,
            startingPosition,
            velocity);

        // do anti-clockwise projectile
        this.FireRotatedProjectile(
            -15f,
            projectile,
            slingshot,
            firer,
            damageBase,
            damageMod,
            knockback,
            overcharge,
            startingPosition,
            velocity);
    }

    private void FireRotatedProjectile(
        float angle,
        Projectile projectile,
        Slingshot slingshot,
        Farmer firer,
        int damageBase,
        float damageMod,
        float knockback,
        float overcharge,
        Vector2 startingPosition,
        Vector2 velocity)
    {
        velocity = velocity.Rotate(angle);
        var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
        var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
        BasicProjectile? rotated = projectile switch
        {
            SnowballProjectile => new SnowballProjectile(
                firer,
                overcharge,
                startingPosition,
                velocity.X,
                velocity.Y,
                rotationVelocity),
            QuincyProjectile => new QuincyProjectile(
                slingshot,
                firer,
                damage,
                overcharge,
                startingPosition,
                velocity.X,
                velocity.Y,
                rotationVelocity),
            ObjectProjectile @object => new ObjectProjectile(
                @object.Ammo,
                @object.Index,
                slingshot,
                firer,
                damage,
                knockback,
                overcharge,
                startingPosition,
                velocity.X,
                velocity.Y,
                rotationVelocity,
                false),
            _ => null,
        };

        if (rotated is null)
        {
            return;
        }

        if (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
        {
            rotated.IgnoreLocationCollision = true;
        }

        firer.currentLocation.projectiles.Add(rotated);
    }
}
