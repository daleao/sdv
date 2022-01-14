namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;

using Extensions;
using SuperMode;

using Professions = Utility.Professions;

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
            if (!firer.HasProfession("Rascal")) return true; // run original logic

            var damageToMonster = (int) (__instance.damageToFarmer.Value *
                                         Professions.GetRascalBonusDamageForTravelTime(___travelTime));

            var hasTemerity = ModEntry.State.Value.SuperMode?.Index == SuperModeIndex.Desperado;
            var bulletPower = Professions.GetDesperadoBulletPower();
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
            if (!firer.HasPrestigedProfession("Rascal")) return false; // don't run original logic

            monster.stunTime = 5000;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}