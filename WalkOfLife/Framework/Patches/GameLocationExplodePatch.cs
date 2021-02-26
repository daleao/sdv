using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using TheLion.Common.Classes.Geometry;
using SObject = StardewValley.Object;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationExplodePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationExplodePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

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

		/// <summary>Patch for Blaster extra resource chance.</summary>
		protected static void GameLocationExplodePrefix(ref GameLocation __instance, bool __state, Vector2 tileLocation, int radius, Farmer who, bool damageFarmers = true, int damage_amount = -1)
		{
			foreach (Vector2 tile in Geometry.GetTilesAround(tileLocation, radius))
			{
				if (__instance.objects.TryGetValue(tile, out SObject tileObj) && TryGetResourceForStone(tileObj.ParentSheetIndex, out int resourceIndex)  && PlayerHasProfession("blaster"))
				{
					Random r = new Random(tile.GetHashCode());
					if (r.NextDouble() < 0.1)
					{
						Game1.createObjectDebris(resourceIndex, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
					}
				}
			}
		}

		/// <summary>Patch for Demolitionist speed burst.</summary>
		protected static void GameLocationExplodePostfix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who, bool damageFarmers = true, int damage_amount = -1)
		{
			foreach (Vector2 tile in Geometry.GetTilesAround(tileLocation, radius))
			{
				if (tile.Equals(who.getTileLocation()) && PlayerHasProfession("demolitionist") && damageFarmers)
				{
					// implement
				}
			}
		}

		/// <summary>Get whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		private static bool IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}
	}
}
