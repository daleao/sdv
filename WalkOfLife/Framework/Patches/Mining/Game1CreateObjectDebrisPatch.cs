using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1CreateObjectDebrisPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new Type[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Gemologist mineral quality.</summary>
		protected static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			Farmer who = Game1.getFarmer(whichPlayer);
			if (Utility.SpecificPlayerHasProfession("gemologist", who) && Utility.IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = Utility.GetGemologistMineralQuality()
				});

				++AwesomeProfessions.Data.MineralsCollected;
				return false; // don't run original logic
			}

			return true; // run original logic
		}
		#endregion harmony patches
	}
}
