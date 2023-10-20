namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal class ProjectileIsCollidingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileIsCollidingPatcher"/> class.</summary>
    internal ProjectileIsCollidingPatcher()
    {
        this.Target = this.RequireMethod<Projectile>(nameof(Projectile.isColliding));
    }

    #region harmony patches

    /// <summary>Allows projectiles to keep traveling over water.</summary>
    [HarmonyPostfix]
    private static void ProjectileIsCollidingPostfix(
        Projectile __instance,
        ref bool __result,
        NetPosition ___position,
        NetFloat ___xVelocity,
        NetFloat ___yVelocity,
        GameLocation location)
    {
        if (!__result)
        {
            return;
        }

        var tile = new Vector2(___position.X / Game1.tileSize, ___position.Y / Game1.tileSize);
        var nextTile = tile.GetNextTile(new Vector2(___xVelocity, ___yVelocity));
        if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") == "T" ||
            location.doesTileHaveProperty((int)nextTile.X, (int)nextTile.Y, "Water", "Back") == "T")
        {
            __result = false;
            return;
        }

        if (location is not BuildableGameLocation buildable)
        {
            return;
        }

        var bb = __instance.getBoundingBox();
        foreach (var building in buildable.buildings)
        {
            if (building.intersects(bb))
            {
                __result = false;
                return;
            }
        }
    }

    #endregion harmony patches
}
