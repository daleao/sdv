namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using System;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Network;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileExplosionAnimationPatch : HarmonyPatch
{
    private static readonly Lazy<Func<BasicProjectile, NetPosition>> GetPosition = new(() =>
        typeof(Projectile)
        .RequireField("position")
        .CompileUnboundFieldGetterDelegate<BasicProjectile, NetPosition>());

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
        if (__instance is not ImmersiveProjectile { IsSnowball: true })
        {
            return;
        }

        location.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                52,
                GetPosition.Value(__instance),
                Color.White,
                8,
                Game1.random.NextDouble() < 0.5,
                50f));
    }

    #endregion harmony patches
}
