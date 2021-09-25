using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileBehaviorOnCollisionWithMonsterPatch()
		{
			Original = typeof(BasicProjectile).MethodNamed(nameof(BasicProjectile.behaviorOnCollisionWithMonster));
			Prefix = new HarmonyMethod(GetType(), nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix));
		}

		#region harmony patches

		/// <summary>Patch for Rascal slingshot damage increase with travel time + apply Piper projectile slow.</summary>
		[HarmonyPrefix]
		private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance, ref NetString ___collisionSound, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, ref NPC n, GameLocation location)
		{
			try
			{
				if (n is not Monster) return true; // run original logic

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
				if (!firer.HasProfession("Rascal")) return true; // run original logic

				ModEntry.Reflection.GetMethod(__instance, name: "explosionAnimation")?.Invoke(location);
				var damageToMonster = (int)(__instance.damageToFarmer.Value * Util.Professions.GetRascalBonusDamageForTravelTime(___travelTime));
				var didAnyDamage = location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, isBomb: false, firer);

				//___collisionSound.Value = "slimeHit";
				//n.addedSpeed -= 4;
				//n.startGlowing(Color.Green, border: false, 0.05f);
				//n.glowingColor = Color.Green;
				//n.glowRate = 0.05f;
				//ModEntry.SlimedMonsterTimers[n.GetHashCode()] = 10 + (int)(10 * (float)ModEntry.SuperModeCounter / ModEntry.SuperModeCounterMax);

				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}