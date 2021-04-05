using Harmony;
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
				AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeUpdateTapperProductPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		private static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance.heldObject.Value != null && Utility.AnyPlayerHasProfession("Tapper", out int n))
			{
				if (tapper_instance.MinutesUntilReady > 0)
					tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady * Math.Pow(0.75, n));
			}
		}

		#endregion harmony patches
	}
}