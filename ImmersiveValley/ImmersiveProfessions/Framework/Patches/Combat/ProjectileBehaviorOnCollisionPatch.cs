namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileBehaviorOnCollisionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileBehaviorOnCollisionPatch"/> class.</summary>
    internal ProjectileBehaviorOnCollisionPatch()
    {
        this.Target = this.RequireMethod<Projectile>("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Patch for Rascal chance to recover ammunition + Piper charge Ultimate with Slime ammo.</summary>
    [HarmonyPostfix]
    private static void ProjectileBehaviorOnCollisionPostfix(
        Projectile __instance,
        NetInt ___currentTileSheetIndex,
        NetPosition ___position,
        NetCharacterRef ___theOneWhoFiredMe,
        GameLocation location)
    {
        if (__instance is not ImmersiveProjectile { CanBeRecovered: true } projectile)
        {
            return;
        }

        var firer = ___theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player;
        if (projectile.IsSlimeAmmo && firer.Get_Ultimate() is Concerto { IsActive: false } concerto)
        {
            concerto.ChargeValue += Game1.random.Next(5);
        }

        if (firer.HasProfession(Profession.Rascal) && (
                (projectile.IsMineralAmmo && Game1.random.NextDouble() < (firer.HasProfession(Profession.Rascal, true) ? 0.7 : 0.3)) ||
                (projectile.IsWood && Game1.random.NextDouble() < (firer.HasProfession(Profession.Rascal, true) ? 0.2 : 0.1))))
        {
            location.debris.Add(
                new Debris(
                    projectile.WhatAmI!.ParentSheetIndex,
                    new Vector2((int)___position.X, (int)___position.Y),
                    firer.getStandingPosition()));
        }
    }

    #endregion harmony patches
}
