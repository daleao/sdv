using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Network;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class BasicProjectileBehaviorOnCollisionWithMonsterPatch : BasePatch
	{
		private static IReflectionHelper _reflection;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BasicProjectileBehaviorOnCollisionWithMonsterPatch(ProfessionsConfig config, IMonitor monitor, IReflectionHelper reflection)
		: base(config, monitor)
		{
			_reflection = reflection;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(BasicProjectile), nameof(BasicProjectile.behaviorOnCollisionWithMonster)),
				prefix: new HarmonyMethod(GetType(), nameof(BasicProjectileBehaviorOnCollisionWithMonsterPrefix))
			);
		}

		/// <summary>Patch for Rascal slingshot damage increase with travel time.</summary>
		protected static bool BasicProjectileBehaviorOnCollisionWithMonsterPrefix(ref BasicProjectile __instance, ref NetBool ___damagesMonsters, ref NetCharacterRef ___theOneWhoFiredMe, ref int ___travelTime, NPC n, GameLocation location)
		{
			Farmer who = ___theOneWhoFiredMe.Get(location) is Farmer ? ___theOneWhoFiredMe.Get(location) as Farmer : Game1.player;
			if (!Utils.SpecificPlayerHasProfession("rascal", who)) return true; // run original logic

			if (!___damagesMonsters) return false; // don't run original logic

			_reflection.GetMethod(__instance, name: "explosionAnimation").Invoke(location);
			if (n is Monster)
			{
				int damageToMonster = (int)(__instance.damageToFarmer.Value * _GetBonusDamageForTravelTime(___travelTime));
				location.damageMonster(n.GetBoundingBox(), damageToMonster, damageToMonster + 1, isBomb: false, who);
			}
			
			return false; // don't run original logic
		}

		/// <summary>Get bonus slingshot damage as function of projectile travel distance.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		private static float _GetBonusDamageForTravelTime(int travelDistance)
		{
			int maxDistance = 800;
			if (travelDistance > maxDistance) return 1.5f;
			return 0.5f / maxDistance * travelDistance + 1f;
		}
	}
}
