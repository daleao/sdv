using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
		private static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			try
			{
				var who = Game1.getFarmer(whichPlayer);
				if (!Utility.SpecificPlayerHasProfession("Gemologist", who) || !Utility.IsGemOrMineral(new SObject(objectIndex, 1)))
					return true; // run original logic

				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = Utility.GetGemologistMineralQuality()
				});

				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(Game1CreateObjectDebrisPrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}