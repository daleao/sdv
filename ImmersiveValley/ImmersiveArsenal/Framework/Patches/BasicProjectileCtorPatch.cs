namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal sealed class BasicProjectileCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BasicProjectileCtorPatch()
    {
        Target = RequireConstructor<BasicProjectile>(typeof(int), typeof(int), typeof(int), typeof(int),
            typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool),
            typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool),
            typeof(BasicProjectile.onCollisionBehavior));
    }

    #region harmony patches

    /// <summary>Removes slingshot grace period.</summary>
    [HarmonyPostfix]
    private static void BasicProjectileCtorPostfix(BasicProjectile __instance, bool damagesMonsters, Character firer)
    {
        if (damagesMonsters && firer is Farmer && ModEntry.Config.RemoveSlingshotGracePeriod)
            __instance.ignoreTravelGracePeriod.Value = true;
    }

    #endregion harmony patches
}