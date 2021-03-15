using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using TheLion.Common.Classes;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationExplodePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationExplodePatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
				prefix: new HarmonyMethod(GetType(), nameof(GameLocationExplodePrefix)),
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationExplodePostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Demolitionist explosion resistance.</summary>
		protected static bool GameLocationExplodePrefix(Farmer who, ref bool damageFarmers)
		{
			if (damageFarmers && Globals.SpecificPlayerHasProfession("demolitionist", who)) damageFarmers = false;

			return true; // run original logic
		}

		/// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
		protected static void GameLocationExplodePostfix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who)
		{
			if (Globals.SpecificPlayerHasProfession("blaster", who))
			{
				double chanceModifier = who.DailyLuck / 2.0 + who.LuckLevel * 0.001 + who.MiningLevel * 0.005;
				CircleTileGrid grid = new CircleTileGrid(tileLocation, radius);
				foreach (Vector2 tile in grid)
				{
					if (__instance.objects.TryGetValue(tile, out SObject tileObj) && _IsStone(tileObj))
					{
						Random r = new Random(tile.GetHashCode());
						if (tileObj.ParentSheetIndex == 343 || tileObj.ParentSheetIndex == 450)
						{
							if (r.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1)
								Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
						}
						else if (r.NextDouble() < 0.05 * chanceModifier)
							Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
					}
				}
			}

			if (Globals.LocalPlayerHasProfession("demolitionist"))
			{
				int distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
				if (distanceFromEpicenter < radius * 2 + 1)
					Globals.DemolitionistBuffMagnitude = 4;
				if (distanceFromEpicenter < radius + 1)
					Globals.DemolitionistBuffMagnitude += 2;
			}
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		private static bool _IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}
		#endregion private methods
	}
}
