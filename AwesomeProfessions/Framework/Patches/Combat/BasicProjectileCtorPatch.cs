namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Projectiles;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BasicProjectileCtorPatch()
    {
        Original = RequireConstructor<BasicProjectile>(typeof(int), typeof(int), typeof(int), typeof(int),
            typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string),
            typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool),
            typeof(BasicProjectile.onCollisionBehavior));
    }

    #region harmony patches

    /// <summary>Patch for all classes to eliminate travel grace period + add Rascal trick shot.</summary>
    [HarmonyPostfix]
    private static void BasicProjectileCtorPostfix(BasicProjectile __instance, NetInt ___bouncesLeft,
        bool damagesMonsters, Character firer)
    {
        if (!damagesMonsters || firer is not Farmer farmer) return;

        __instance.ignoreTravelGracePeriod.Value = true;

        if (!farmer.HasProfession(Profession.Rascal) || !ModEntry.Config.ModKey.IsDown()) return;
        ++___bouncesLeft.Value;
        __instance.damageToFarmer.Value = (int) (__instance.damageToFarmer.Value * 0.6);
    }

    #endregion harmony patches
}