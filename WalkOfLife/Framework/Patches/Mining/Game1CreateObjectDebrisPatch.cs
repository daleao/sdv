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

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1CreateObjectDebrisPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new Type[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix))
			);
		}

		/// <summary>Patch for Gemologist mineral quality.</summary>
		protected static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			Farmer who = Game1.getFarmer(whichPlayer);
			if (Utils.SpecificPlayerHasProfession("gemologist", who) && _IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = _GetMineralQualityForGemologist()
				});

				++AwesomeProfessions.Data.MineralsCollectedAsGemologist;
				return false; // don't run original logic
			}

			return true; // run original logic
		}

		/// <summary>Whether a given object is a gem or mineral.</summary>
		/// <param name="objectIndex">The given object.</param>
		private static bool _IsMineral(int objectIndex)
		{
			return _gemIds.Contains(objectIndex) || (objectIndex > 537 && objectIndex < 579);
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		private static int _GetMineralQualityForGemologist()
		{
			return AwesomeProfessions.Data.MineralsCollectedAsGemologist < _config.Gemologist.MineralsNeededForBestQuality ? (AwesomeProfessions.Data.MineralsCollectedAsGemologist < _config.Gemologist.MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}
	}
}
