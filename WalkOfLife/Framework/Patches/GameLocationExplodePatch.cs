using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using TheLion.Common.Classes.Geometry;

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
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationExplodePostfix))
			);
		}

		// Patch for
		protected static void GameLocationExplodePostfix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who, bool damageFarmers = true, int damage_amount = -1)
		{
			Random r = new Random();
			foreach (Vector2 tile in Geometry.GetTilesAround(tileLocation, radius))
			{
				if (tile.Equals(who.getTileLocation()) && PlayerHasProfession("demolitionist") && damageFarmers)
				{
					// implement
				}

				if (PlayerHasProfession("blaster") && r.NextDouble() < 0.10)
				{
					//Game1.createRadialDebris(__instance, 14, (int)tileLocation.X, (int)tileLocation.Y, 1, true);
				}
			}
		}
	}
}
