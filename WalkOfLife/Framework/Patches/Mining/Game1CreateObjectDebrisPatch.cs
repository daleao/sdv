using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal Game1CreateObjectDebrisPatch()
		{
			Original = typeof(Game1).MethodNamed(nameof(Game1.createObjectDebris),
				new[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) });
			Prefix = new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix));
		}

		#region harmony patches

		/// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
		[HarmonyPrefix]
		private static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer,
			GameLocation location)
		{
			try
			{
				var who = Game1.getFarmer(whichPlayer);
				if (!who.HasProfession("Gemologist") || !new SObject(objectIndex, 1).IsGemOrMineral())
					return true; // run original logic

				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32),
					who.getStandingPosition())
				{
					itemQuality = Util.Professions.GetGemologistMineralQuality()
				});

				ModEntry.Data.IncrementField<uint>("MineralsCollected");
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}