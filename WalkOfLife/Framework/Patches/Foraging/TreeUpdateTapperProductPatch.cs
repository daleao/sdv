using Harmony;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class TreeUpdateTapperProductPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeUpdateTapperProductPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		private static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance == null) return;

			try
			{
				var owner = Game1.getFarmer(tapper_instance.owner.Value);
				if (!Utility.SpecificPlayerHasProfession("Tapper", owner)) return;

				if (tapper_instance.MinutesUntilReady > 0)
					tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady * 0.75);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(TreeUpdateTapperProductPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}