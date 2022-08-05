namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileExplosionAnimationPatch : Common.Harmony.HarmonyPatch
{
    private static Lazy<Func<BasicProjectile, NetPosition>> _GetPosition = new(() => typeof(Projectile).RequireField("position")
        .CompileUnboundFieldGetterDelegate<BasicProjectile, NetPosition>());

    /// <summary>Construct an instance.</summary>
    internal BasicProjectileExplosionAnimationPatch()
    {
        Target = RequireMethod<BasicProjectile>("explosionAnimation");
    }

    #region harmony patches

    /// <summary>Snowball collision animation, which prefers <see cref="Projectile.position"/> over <see cref="Projectile.getBoundingBox.Center"/>.</summary>
    [HarmonyPostfix]
    private static void BasicProjectileExplosionAnimationPostfix(BasicProjectile __instance, GameLocation location)
    {
        if (__instance is not ImmersiveProjectile {IsSnowball: true}) return;

        location.temporarySprites.Add(new(52, _GetPosition.Value(__instance), Color.White, 8,
            Game1.random.NextDouble() < 0.5, 50f));
    }

    #endregion harmony patches
}