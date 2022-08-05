namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Common.Extensions.Reflection;
using Common.Extensions.Xna;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Fire 2 additional projectiles.</summary>
[XmlType("Mods_DaLion_SpreadingEnchantment")]
public class SpreadingEnchantment : BaseSlingshotEnchantment
{
    private static readonly Lazy<Func<BasicProjectile, NetInt>> _GetCurrentTileSheetIndex = new(() =>
        typeof(Projectile).RequireField("currentTileSheetIndex")
            .CompileUnboundFieldGetterDelegate<BasicProjectile, NetInt>());

    private static readonly Lazy<Func<BasicProjectile, NetBool>> _GetSpriteFromObjectSheet = new(() =>
        typeof(Projectile).RequireField("spriteFromObjectSheet")
            .CompileUnboundFieldGetterDelegate<BasicProjectile, NetBool>());

    private static readonly Lazy<Func<BasicProjectile, NetFloat>> _GetXVelocity = new(() =>
        typeof(Projectile).RequireField("xVelocity").CompileUnboundFieldGetterDelegate<BasicProjectile, NetFloat>());

    private static readonly Lazy<Func<BasicProjectile, NetFloat>> _GetYVelocity = new(() =>
        typeof(Projectile).RequireField("yVelocity").CompileUnboundFieldGetterDelegate<BasicProjectile, NetFloat>());

    private static readonly Lazy<Func<BasicProjectile, NetString>> _GetCollisionSound = new(() =>
        typeof(BasicProjectile).RequireField("collisionSound")
            .CompileUnboundFieldGetterDelegate<BasicProjectile, NetString>());

    private static readonly Lazy<Func<BasicProjectile, BasicProjectile.onCollisionBehavior?>> _GetCollisionBehavior =
        new(() => typeof(BasicProjectile).RequireField("collisionBehavior")
            .CompileUnboundFieldGetterDelegate<BasicProjectile, BasicProjectile.onCollisionBehavior?>());

    protected override void _OnFire(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer who)
    {
        var velocity = new Vector2(_GetXVelocity.Value(projectile).Value, _GetYVelocity.Value(projectile).Value);
        var speed = velocity.Length();
        velocity.Normalize();
        float angle;
        if (ModEntry.ProfessionsApi is not null && who.professions.Contains(Farmer.desperado + 100))
        {
            var overcharge = Math.Clamp(
                (float)((Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime) /
                    slingshot.GetRequiredChargeTime() - 1f) / 6f, 0f, 1f);
            angle = MathHelper.Lerp(1f, 0.5f, (overcharge - 1.5f) * 2f) * 15f;
        }
        else
        {
            angle = 15f;
        }

        var shootOrigin = slingshot.GetShootOrigin(who);
        var startingPosition = shootOrigin - new Vector2(32f, 32f);
        var damage = (int)(projectile.damageToFarmer.Value * 0.4f);
        var index = _GetCurrentTileSheetIndex.Value(projectile).Value;
        var isObject = _GetSpriteFromObjectSheet.Value(projectile).Value;

        velocity = velocity.Rotate(angle);
        var rDamage = (int)(damage * (1d + Game1.random.Next(-2, 3) / 10d));
        var clockwise = new ImmersiveProjectile(slingshot, rDamage, index, 0, 0,
            (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed, velocity.Y * speed,
            startingPosition, _GetCollisionSound.Value(projectile).Value, string.Empty, false, true, location, who,
            isObject, _GetCollisionBehavior.Value(projectile))
        {
            IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                      Game1.currentMinigame is not null
        };

        location.projectiles.Add(clockwise);

        velocity = velocity.Rotate(-2 * angle);
        rDamage = (int)(damage * (1.0 + Game1.random.Next(-2, 3) / 10.0));
        var anticlockwise = new ImmersiveProjectile(slingshot, rDamage, index, 0, 0,
            (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed, velocity.Y * speed,
            startingPosition, _GetCollisionSound.Value(projectile).Value, string.Empty, false, true, location, who,
            isObject, _GetCollisionBehavior.Value(projectile))
        {
            IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                      Game1.currentMinigame is not null
        };

        location.projectiles.Add(anticlockwise);
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.spreading");
}