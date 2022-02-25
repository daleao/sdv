#nullable enable
namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;

using Stardew.Common.Extensions;
using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
{
    private static readonly MethodInfo _ExplosionAnimation = typeof(BasicProjectile).MethodNamed("explosionAnimation");

    /// <summary>Construct an instance.</summary>
    internal BasicProjectileBehaviorOnCollisionWithMonsterPatch()
    {
        Original = RequireMethod<BasicProjectile>(nameof(BasicProjectile.behaviorOnCollisionWithMonster));
    }

    #region harmony patches

    /// <summary>
    ///     Patch for Rascal slingshot damage increase with travel time + Desperado pierce shot + prestiged Rascal trick
    ///     shot.
    /// </summary>
    [HarmonyPrefix]
    private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance,
        NetBool ___damagesMonsters, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, NPC n,
        GameLocation location)
    {
        try
        {
            if (!___damagesMonsters.Value) return false; // don't run original logic

            if (n is not Monster monster) return true; // run original logic

            var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
            if (!firer.HasProfession(Profession.Rascal)) return true; // run original logic

            var damageToMonster =
                (int) (__instance.damageToFarmer.Value * GetRascalBonusDamageForTravelTime(___travelTime));

            var hasTemerity = firer.IsLocalPlayer && ModEntry.PlayerState.Value.SuperMode is DesperadoTemerity;
            var bulletPower = hasTemerity ? (ModEntry.PlayerState.Value.SuperMode as DesperadoTemerity)!.GetShootingPower() : 1f;
            if (hasTemerity && Game1.random.NextDouble() < (bulletPower - 1) / 2)
                ModEntry.PlayerState.Value.PiercedBullets.Add(__instance.GetHashCode());
            else
                _ExplosionAnimation.Invoke(__instance, new object?[] {location});

            location.damageMonster(monster.GetBoundingBox(), damageToMonster, damageToMonster + 1, false,
                bulletPower, 0, 0f, 1f, false, firer);

            // check for trick shot
            if (!ModEntry.PlayerState.Value.BouncedBullets.Remove(__instance.GetHashCode())) return false; // don't run original logic

            // give a bonus to Desperados
            if (hasTemerity)
                ModEntry.PlayerState.Value.SuperMode.ChargeValue += 6 * ModEntry.Config.SuperModeGainFactor *
                    SuperMode.MaxValue / SuperMode.INITIAL_MAX_VALUE_I;

            // stun if prestiged Rascal
            if (!firer.HasProfession(Profession.Rascal, true)) return false; // don't run original logic

            monster.stunTime = 5000;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region private methods

    public static float GetRascalBonusDamageForTravelTime(int travelTime)
    {
        const int MAX_TRAVEL_TIME_I = 800;
        if (travelTime > MAX_TRAVEL_TIME_I) return 1.5f;
        return 1f + 0.5f / MAX_TRAVEL_TIME_I * travelTime;
    }

    #endregion private methods
}