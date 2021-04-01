using Harmony;
using StardewValley.TerrainFeatures;

namespace TheLion.AwesomeProfessions
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FruitTree), nameof(FruitTree.dayUpdate)),
				postfix: new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			if (Utility.AnyFarmerHasProfession("arborist", out int n) && __instance.daysUntilMature.Value % 7 == 0)
				__instance.daysUntilMature.Value -= n;
		}

		#endregion harmony patches
	}
}