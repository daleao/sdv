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
	internal class Game1CreateItemDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1CreateItemDebrisPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createItemDebris)),
				postfix: new HarmonyMethod(GetType(), nameof(Game1CreateItemDebrisPostfix))
			);
		}

		/// <summary>Patch to count foraged berries as Ecologist.</summary>
		protected static void Game1CreateItemDebrisPostfix(Item item)
		{
			if (_IsWildBerry(item) && Utils.LocalPlayerHasProfession("ecologist"))
				++AwesomeProfessions.Data.ForageablesCollectedAsEcologist;
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWildBerry(Item item)
		{
			return item?.ParentSheetIndex == 296 || item?.ParentSheetIndex == 410;
		}
	}
}
