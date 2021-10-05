using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class TreeUpdateTapperProductPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TreeUpdateTapperProductPatch()
		{
			Original = typeof(Tree).MethodNamed(nameof(Tree.UpdateTapperProduct));
			Postfix = new HarmonyMethod(GetType(), nameof(TreeUpdateTapperProductPostfix));
		}

		#region harmony patches

		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		[HarmonyPostfix]
		private static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance == null) return;

			try
			{
				var owner = Game1.getFarmer(tapper_instance.owner.Value);
				if (!owner.HasProfession("Tapper")) return;

				if (tapper_instance.MinutesUntilReady > 0)
					tapper_instance.MinutesUntilReady = (int) (tapper_instance.MinutesUntilReady * 0.75);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}