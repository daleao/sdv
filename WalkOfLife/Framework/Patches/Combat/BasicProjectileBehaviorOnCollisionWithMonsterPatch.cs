using HarmonyLib;
using Netcode;
using StardewModdingAPI;
using StardewValley;
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

		/// <summary>Patch for Rascal slingshot damage increase with travel time.</summary>
		[HarmonyPrefix]
		private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(BasicProjectile __instance,
			ref NetBool ___damagesMonsters, NetCharacterRef ___theOneWhoFiredMe, int ___travelTime, ref NPC n,
			GameLocation location)
		{
			try
			{
				if (!___damagesMonsters.Value) return false; // don't run original logic

				if (!n.IsMonster) return true; // run original logic

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer farmer ? farmer : Game1.player;
				if (!firer.HasProfession("Rascal")) return true; // run original logic

				ModEntry.ModHelper.Reflection.GetMethod(__instance, "explosionAnimation")?.Invoke(location);
				var damageToMonster = (int)(__instance.damageToFarmer.Value *
											 Util.Professions.GetRascalBonusDamageForTravelTime(___travelTime));
				location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, false, firer);

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