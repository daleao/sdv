using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster)),
				prefix: new HarmonyMethod(GetType(), nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Rascal slingshot damage increase with travel time.</summary>
		private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(ref BasicProjectile __instance, ref NetBool ___damagesMonsters, ref NetCharacterRef ___theOneWhoFiredMe, ref int ___travelTime, NPC n, GameLocation location)
		{
			try
			{
				if (!___damagesMonsters || n is not Monster) return true; // run original logic
				
				var who = ___theOneWhoFiredMe.Get(location) is Farmer
					? ___theOneWhoFiredMe.Get(location) as Farmer
					: Game1.player;
				if (!Utility.SpecificPlayerHasProfession("Rascal", who)) return true; // run original logic

				AwesomeProfessions.Reflection.GetMethod(__instance, name: "explosionAnimation")?.Invoke(location);
				var damageToMonster = (int) (__instance.damageToFarmer.Value * Utility.GetRascalBonusDamageForTravelTime(___travelTime));
				location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, isBomb: false, who);

				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix)}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}