using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ProjectileBehaviorOnCollisionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Projectile), name: "behaviorOnCollision"),
				postfix: new HarmonyMethod(GetType(), nameof(ProjectileBehaviorOnCollisionPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Rascal chance to recover ammunition.</summary>
		private static void ProjectileBehaviorOnCollisionPostfix(ref Projectile __instance, ref NetInt ___currentTileSheetIndex, ref NetPosition ___position, ref NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
		{
			try
			{
				if (__instance is not BasicProjectile) return;

				var firer = ___theOneWhoFiredMe.Get(location) is Farmer ? (Farmer)___theOneWhoFiredMe.Get(location) : Game1.player;
				if (!Utility.SpecificPlayerHasProfession("Rascal", firer)) return;

				if (Utility.IsMineralAmmunition(___currentTileSheetIndex.Value) && Game1.random.NextDouble() < 0.6
				|| ___currentTileSheetIndex.Value == SObject.wood + 1 && Game1.random.NextDouble() < 0.3)
					location.debris.Add(new Debris(___currentTileSheetIndex.Value - 1, new Vector2((int)___position.X, (int)___position.Y), firer.getStandingPosition()));
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ProjectileBehaviorOnCollisionPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}