namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileExplosionAnimationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileExplosionAnimationPatcher"/> class.</summary>
    internal BasicProjectileExplosionAnimationPatcher()
    {
        this.Target = this.RequireMethod<BasicProjectile>("explosionAnimation");
    }

    #region harmony patches

    /// <summary>
    ///     Snowball collision animation, which prefers <see cref="Projectile.position"/> instead of the <see cref="Rectangle.Center"/>
    ///     of <see cref="Projectile.getBoundingBox"/>.
    /// </summary>
    [HarmonyPostfix]
    private static void BasicProjectileExplosionAnimationPostfix(BasicProjectile __instance, GameLocation location)
    {
        switch (__instance)
        {
            case SnowballProjectile snowball:
                snowball.ExplosionAnimation(location);
                break;
        }
    }

    #endregion harmony patches
}
