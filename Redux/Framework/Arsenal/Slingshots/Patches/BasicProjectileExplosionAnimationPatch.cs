namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
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
    private static void BasicProjectileExplosionAnimationPostfix(BasicProjectile __instance, NetInt ___currentTileSheetIndex, GameLocation location)
    {
        if (__instance is not ReduxProjectile { Ammo: null } || ___currentTileSheetIndex.Value != Constants.SnowballProjectileIndex)
        {
            return;
        }

        var position = ModEntry.Reflector
            .GetUnboundFieldGetter<BasicProjectile, NetPosition>(__instance, "position")
            .Invoke(__instance).Value;
        location.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                52,
                position,
                Color.White,
                8,
                Game1.random.NextDouble() < 0.5,
                50f));
    }

    #endregion harmony patches
}
