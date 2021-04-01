using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new Type[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Gemologist mineral quality.</summary>
		private static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			Farmer who = Game1.getFarmer(whichPlayer);
			if (Utility.SpecificFarmerHasProfession("gemologist", who) && Utility.IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = Utility.GetGemologistMineralQuality()
				});

				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected");
				return false; // don't run original logic
			}

			return true; // run original logic
		}

		#endregion harmony patches
	}
}