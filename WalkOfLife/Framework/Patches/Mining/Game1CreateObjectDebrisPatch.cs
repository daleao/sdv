using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		#region private fields
		/// <summary>Set of item id's corresponding to gems or minerals.</summary>
		private static readonly IEnumerable<int> _gemIds = new HashSet<int>
		{
			SObject.emeraldIndex,
			SObject.aquamarineIndex,
			SObject.rubyIndex,
			SObject.amethystClusterIndex,
			SObject.topazIndex,
			SObject.sapphireIndex,
			SObject.diamondIndex,
			SObject.prismaticShardIndex
		};
		#endregion private fields

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
			if (Globals.SpecificPlayerHasProfession("gemologist", who) && _IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = Globals.GetMineralQualityForGemologist()
				});

				++AwesomeProfessions.Data.MineralsCollected;
				return false; // don't run original logic
			}

			return true; // run original logic
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given object is a gem or mineral.</summary>
		/// <param name="objectIndex">The given object.</param>
		private static bool _IsMineral(int objectIndex)
		{
			return _gemIds.Contains(objectIndex) || (objectIndex > 537 && objectIndex < 579);
		}
		#endregion private methods
	}
}
