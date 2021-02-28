using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using TheLion.Common.TileGeometry;
using SObject = StardewValley.Object;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationExplodePatch : BasePatch
	{
		private static ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		internal GameLocationExplodePatch(ModConfig config, IMonitor monitor, ITranslationHelper i18n)
		: base(config, monitor)
		{
			_i18n = i18n;
	}

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
		protected static void GameLocationExplodePrefix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who)
		{
			CircleTileGrid grid = new CircleTileGrid(tileLocation, radius);
			foreach (Vector2 tile in grid)
			{
				if (__instance.objects.TryGetValue(tile, out SObject tileObj) && TryGetResourceForStone(tileObj.ParentSheetIndex, out int resourceIndex)  && PlayerHasProfession("blaster", who))
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
		protected static void GameLocationExplodePostfix(Vector2 tileLocation, int radius, Farmer who, bool damageFarmers = true)
		{
			if (!PlayerHasProfession("demolitionist") || !damageFarmers)
			{
				return;
			}

			int distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
			if (distanceFromEpicenter < radius * 2 + 1)
			{
				ModEntry.DemolitionistBuffMagnitude = 4;
			}
			if (distanceFromEpicenter < radius + 1)
			{
				ModEntry.DemolitionistBuffMagnitude += 2;
			}
		}
	}
}
