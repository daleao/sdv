namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Projectiles;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Projectiles;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileExplosionAnimationPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileExplosionAnimationPatch"/> class.</summary>
    internal BasicProjectileExplosionAnimationPatch()
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
            case QuincyProjectile quincy:
                quincy.ExplosionAnimation(location);
                break;
            case SnowballProjectile snowball:
                snowball.ExplosionAnimation(location);
                break;
            case InfinityProjectile infinity:
                infinity.ExplosionAnimation(location);
                break;
        }
    }

    #endregion harmony patches
}
