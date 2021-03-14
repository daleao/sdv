using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Network;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ProjectileBehaviorOnCollisionPatch : BasePatch
	{
		/// <summary>Set of ammunition id's.</summary>
		private static readonly IEnumerable<int> _ammunitionIds = new HashSet<int>
		{
			SObject.copper + 1,
			SObject.iron + 1,
			SObject.coal + 1,
			SObject.gold + 1,
			SObject.iridium + 1,
			SObject.wood + 1,
			SObject.stone + 1,
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ProjectileBehaviorOnCollisionPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Projectile), name: "behaviorOnCollision"),
				postfix: new HarmonyMethod(GetType(), nameof(ProjectileBehaviorOnCollisionPostfix))
			);
		}

		/// <summary>Patch for Rascal chance to recover ammunition.</summary>
		protected static void ProjectileBehaviorOnCollisionPostfix(ref Projectile __instance, ref NetInt ___currentTileSheetIndex, ref NetPosition ___position, ref NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
		{
			if (!(__instance is BasicProjectile)) return;

			Farmer who = ___theOneWhoFiredMe.Get(location) is Farmer ? ___theOneWhoFiredMe.Get(location) as Farmer : Game1.player;
			if (!Utils.SpecificPlayerHasProfession("rascal", who)) return;

			if (Game1.random.NextDouble() < 0.6 && _ammunitionIds.Contains(___currentTileSheetIndex.Value))
				location.debris.Add(new Debris(___currentTileSheetIndex.Value - 1, new Vector2((int)___position.X, (int)___position.Y), who.getStandingPosition()));
		}
	}
}
