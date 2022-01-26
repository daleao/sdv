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

using Extensions;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
{
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
        ref NetBool ___damagesMonsters, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, ref NPC n,
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

            var hasTemerity = ModEntry.State.Value.SuperMode?.Index == SuperModeIndex.Desperado;
            var bulletPower = firer.GetDesperadoBulletPower();
            if (hasTemerity && Game1.random.NextDouble() < (bulletPower - 1) / 2)
                ModEntry.State.Value.PiercedBullets.Add(__instance.GetHashCode());
            else
                ModEntry.ModHelper.Reflection.GetMethod(__instance, "explosionAnimation")?.Invoke(location);

            var knockbackModifier =
                firer.IsLocalPlayer && hasTemerity
                    ? bulletPower
                    : 1f;
            location.damageMonster(monster.GetBoundingBox(), damageToMonster, damageToMonster + 1, false,
                knockbackModifier, 0, 0f, 1f, false, firer);

            // check for trick shot
            if (!ModEntry.State.Value.BouncedBullets.Remove(__instance.GetHashCode())) return false; // don't run original logic

            // give a bonus to Desperados
            if (hasTemerity)
                ModEntry.State.Value.SuperMode.Gauge.CurrentValue += 8 * ModEntry.Config.SuperModeGainFactor *
                    (double) SuperModeGauge.MaxValue / 500;

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