using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Combat;

[UsedImplicitly]
internal class ProjectileBehaviorOnCollisionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ProjectileBehaviorOnCollisionPatch()
    {
        Original = RequireMethod<Projectile>("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Patch for Rascal chance to recover ammunition.</summary>
    [HarmonyPostfix]
    private static void ProjectileBehaviorOnCollisionPostfix(Projectile __instance, NetInt ___currentTileSheetIndex,
        NetPosition ___position, NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
    {
        if (__instance is not BasicProjectile basic) return;

        var hashCode = basic.GetHashCode();
        ModEntry.State.Value.BouncedBullets.Remove(hashCode);
        if (ModEntry.State.Value.AuxiliaryBullets.Remove(hashCode)) return;

        var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        if (!firer.HasProfession("Rascal")) return;

        if ((___currentTileSheetIndex.Value - 1).IsAnyOf(SObject.copper, SObject.iron, SObject.gold,
                SObject.iridium, SObject.stone) && Game1.random.NextDouble() < 0.6
            || ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.3)
            location.debris.Add(new(___currentTileSheetIndex.Value - 1,
                new((int) ___position.X, (int) ___position.Y), firer.getStandingPosition()));
    }

    #endregion harmony patches
}