namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ArrowProjectileUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ArrowProjectileUpdatePatcher"/> class.</summary>
    internal ArrowProjectileUpdatePatcher()
    {
        this.Target = "Archery.Framework.Objects.Projectiles.ArrowProjectile"
            .ToType()
            .RequireMethod("update");
    }

    #region harmony patches

    /// <summary>Adds overcharged hitbox effect to arrows.</summary>
    [HarmonyPostfix]
    private static void ArrowProjectileUpdatePostfix(
        BasicProjectile __instance,
        bool __result,
        int ____collectiveDamage,
        float ____criticalChance,
        float ____criticalDamageMultiplier,
        float ____knockback,
        Farmer ____owner,
        GameLocation location)
    {
        if (!____owner.HasProfession(Profession.Desperado))
        {
            return;
        }

        var overcharge = __instance.Get_Overcharge();
        if (overcharge <= 1f || (__instance.maxTravelDistance.Value > 0 &&
                                 __instance.travelDistance >= __instance.maxTravelDistance.Value))
        {
            return;
        }

        if (__result)
        {
            return;
        }

        // get trajectory angle
        var xVelocity = Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "xVelocity")
            .Invoke(__instance);
        var yVelocity = Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "yVelocity")
            .Invoke(__instance);
        var velocity = new Vector2(xVelocity.Value, yVelocity.Value);
        var angle = velocity.AngleWithHorizontal();
        if (angle > 180)
        {
            angle -= 360;
        }

        // check for extended collision
        var originalHitbox = __instance.getBoundingBox();
        var newHitbox = new Rectangle(originalHitbox.X, originalHitbox.Y, originalHitbox.Width, originalHitbox.Height);
        var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
        if (isBulletTravelingVertically)
        {
            newHitbox.Inflate((int)(originalHitbox.Width * overcharge), 0);
            if (newHitbox.Width <= originalHitbox.Width)
            {
                return;
            }
        }
        else
        {
            newHitbox.Inflate(0, (int)(originalHitbox.Height * overcharge));
            if (newHitbox.Height <= originalHitbox.Height)
            {
                return;
            }
        }

        if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster monster)
        {
            return;
        }

        // deal damage
        int actualDistance, monsterRadius, actualBulletRadius, extendedBulletRadius;
        if (isBulletTravelingVertically)
        {
            actualDistance = Math.Abs(monster.getStandingX() - originalHitbox.Center.X);
            monsterRadius = monster.GetBoundingBox().Width / 2;
            actualBulletRadius = originalHitbox.Width / 2;
            extendedBulletRadius = newHitbox.Width / 2;
        }
        else
        {
            actualDistance = Math.Abs(monster.getStandingY() - originalHitbox.Center.Y);
            monsterRadius = monster.GetBoundingBox().Height / 2;
            actualBulletRadius = originalHitbox.Height / 2;
            extendedBulletRadius = newHitbox.Height / 2;
        }

        var lerpFactor = (actualDistance - (actualBulletRadius + monsterRadius)) /
                         (extendedBulletRadius - actualBulletRadius);
        var multiplier = MathHelper.Lerp(1f, 0f, lerpFactor);
        var adjustedDamage = (int)(____collectiveDamage * multiplier);
        location.damageMonster(
            monster.GetBoundingBox(),
            adjustedDamage,
            adjustedDamage,
            false,
            ____knockback * multiplier,
            0,
            ____criticalChance,
            ____criticalDamageMultiplier,
            true,
            ____owner);
    }

    #endregion harmony patches
}
