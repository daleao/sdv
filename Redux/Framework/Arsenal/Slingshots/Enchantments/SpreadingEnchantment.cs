namespace DaLion.Redux.Framework.Arsenal.Slingshots.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Netcode;
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
        return ModEntry.i18n.Get("enchantments.spreading");
    }

    /// <inheritdoc />
    protected override void _OnFire(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer firer)
    {
        var xVelocty = ModEntry.Reflector
            .GetUnboundFieldGetter<BasicProjectile, NetFloat>(projectile, "xVelocity")
            .Invoke(projectile).Value;
        var yVelocty = ModEntry.Reflector
            .GetUnboundFieldGetter<BasicProjectile, NetFloat>(projectile, "yVelocity")
            .Invoke(projectile).Value;
        var velocity = new Vector2(xVelocty, yVelocty);
        var speed = velocity.Length();
        velocity.Normalize();
        float angle;
        if (ModEntry.Config.EnableProfessions && firer.professions.Contains(Farmer.desperado))
        {
            var overcharge = slingshot.GetOvercharge(firer);
            angle = MathHelper.Lerp(1f, 0.5f, (overcharge - 1.5f) * 2f) * 15f;
        }
        else
        {
            angle = 15f;
        }

        var shootOrigin = slingshot.GetShootOrigin(firer);
        var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
        var startingPosition = shootOrigin - new Vector2(32f, 32f);
        var damage = (int)(projectile.damageToFarmer.Value * 0.4f);
        var index = ModEntry.Reflector
            .GetUnboundFieldGetter<BasicProjectile, NetInt>(projectile, "currentTileSheetIndex")
            .Invoke(projectile).Value;
        var spriteFromObjectSheet = ModEntry.Reflector
            .GetUnboundFieldGetter<BasicProjectile, NetBool>(projectile, "spriteFromObjectSheet")
            .Invoke(projectile).Value;
        var ammo = spriteFromObjectSheet ? new SObject(index, 1) : null;

        velocity = velocity.Rotate(angle);
        var rDamage = (int)(damage * (1d + (Game1.random.Next(-2, 3) / 10d)));
        var clockwise = new ReduxProjectile(
            ammo,
            index,
            slingshot,
            firer,
            location,
            rDamage,
            startingPosition,
            velocity.X * speed,
            velocity.Y * speed,
            rotationVelocity,
            spriteFromObjectSheet)
        {
            IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                      Game1.currentMinigame is not null,
        };

        location.projectiles.Add(clockwise);

        velocity = velocity.Rotate(-2 * angle);
        rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
        rDamage = (int)(damage * (1.0 + (Game1.random.Next(-2, 3) / 10.0)));
        var anticlockwise = new ReduxProjectile(
            ammo,
            index,
            slingshot,
            firer,
            location,
            rDamage,
            startingPosition,
            velocity.X * speed,
            velocity.Y * speed,
            rotationVelocity,
            spriteFromObjectSheet)
        {
            IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                      Game1.currentMinigame is not null,
        };

        location.projectiles.Add(anticlockwise);
    }
}
