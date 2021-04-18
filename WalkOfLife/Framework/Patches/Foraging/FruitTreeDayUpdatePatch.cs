using Harmony;
using StardewValley.TerrainFeatures;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.dayUpdate)),
				postfix: new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			try
			{
				if (Utility.AnyPlayerHasProfession("Arborist", out _) && __instance.daysUntilMature.Value % 4 == 0)
					--__instance.daysUntilMature.Value;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FruitTreeDayUpdatePostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}