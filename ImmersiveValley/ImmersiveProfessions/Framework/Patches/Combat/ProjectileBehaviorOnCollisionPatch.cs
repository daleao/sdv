namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using Netcode;
using StardewValley.Network;
using StardewValley.Projectiles;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileBehaviorOnCollisionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ProjectileBehaviorOnCollisionPatch()
    {
        Target = RequireMethod<Projectile>("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Patch for Rascal chance to recover ammunition + Piper charge Ultimate with Slime ammo.</summary>
    [HarmonyPostfix]
    private static void ProjectileBehaviorOnCollisionPostfix(Projectile __instance, NetInt ___currentTileSheetIndex,
        NetPosition ___position, NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
    {
        if (__instance is not ImmersiveProjectile projectile || projectile.IsBlossomPetal) return;

        var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
        if (projectile.IsSlimeAmmo() && firer.get_Ultimate() is Concerto { IsActive: false } concerto)
            concerto.ChargeValue += Game1.random.Next(5);

        if (firer.HasProfession(Profession.Rascal) && (projectile.IsMineralAmmo() && Game1.random.NextDouble() < 0.5 ||
            ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.25))
            location.debris.Add(new(___currentTileSheetIndex.Value - 1, new((int)___position.X, (int)___position.Y),
                firer.getStandingPosition()));
    }

    #endregion harmony patches
}