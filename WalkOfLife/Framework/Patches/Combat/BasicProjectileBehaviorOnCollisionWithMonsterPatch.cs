using Harmony;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;

namespace TheLion.AwesomeProfessions
{
	internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster)),
				prefix: new HarmonyMethod(GetType(), nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Rascal slingshot damage increase with travel time.</summary>
		private static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(ref BasicProjectile __instance, ref NetBool ___damagesMonsters, ref NetCharacterRef ___theOneWhoFiredMe, ref int ___travelTime, NPC n, GameLocation location)
		{
			Farmer who = ___theOneWhoFiredMe.Get(location) is Farmer ? ___theOneWhoFiredMe.Get(location) as Farmer : Game1.player;
			if (!Utility.SpecificFarmerHasProfession("rascal", who)) return true; // run original logic

			if (!___damagesMonsters) return false; // don't run original logic

			AwesomeProfessions.Reflection.GetMethod(__instance, name: "explosionAnimation").Invoke(location);
			if (n is Monster)
			{
				int damageToMonster = (int)(__instance.damageToFarmer.Value * Utility.GetRascalBonusDamageForTravelTime(___travelTime));
				location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, isBomb: false, who);
			}

			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}