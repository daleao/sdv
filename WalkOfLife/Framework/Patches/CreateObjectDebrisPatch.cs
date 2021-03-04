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
	internal class CreateObjectDebrisPatch : BasePatch
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
		internal CreateObjectDebrisPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new Type[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(CreateObjectDebrisPrefix))
			);
		}

		/// <summary>Patch for Gemologist mineral quality.</summary>
		protected static bool CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			if (Utils.PlayerHasProfession("gemologist", Game1.getFarmer(whichPlayer)) && IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), Game1.getFarmer(whichPlayer).getStandingPosition())
				{
					itemQuality = GetMineralQualityForGemologist()
				});
				
				return false; // don't run original logic
			}

			return true; // run original logic
		}

		/// <summary>Whether a given object is a gem or mineral.</summary>
		/// <param name="obj">The given object.</param>
		protected static bool IsMineral(int objectIndex)
		{
			return _gemIds.Contains(objectIndex) || (objectIndex > 537 && objectIndex < 579);
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		protected static int GetMineralQualityForGemologist()
		{
			return ModEntry.Data.MineralsCollected < _config.GemologistConfig.MineralsNeededForBestQuality ? (ModEntry.Data.ItemsForaged < _config.GemologistConfig.MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}
	}
}
