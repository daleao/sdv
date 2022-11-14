namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal class ProjectileIsCollidingPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileIsCollidingPatch"/> class.</summary>
    internal ProjectileIsCollidingPatch()
    {
        this.Target = this.RequireMethod<Projectile>(nameof(Projectile.isColliding));
    }

    #region harmony patches

    [HarmonyPostfix]
    private static void ProjectileIsCollidingPostfix(
        ref bool __result,
        NetPosition ___position,
        GameLocation location)
    {
        if (!__result)
        {
            return;
        }

        if (location.doesTileHaveProperty(
                (int)___position.X / Game1.tileSize,
                (int)___position.Y / Game1.tileSize,
                "Water",
                "Back") == "T")
        {
            __result = false;
        }
    }

    #endregion harmony patches
}
