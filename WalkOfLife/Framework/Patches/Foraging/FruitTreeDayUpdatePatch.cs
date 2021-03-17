using Harmony;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace TheLion.AwesomeProfessions
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FruitTreeDayUpdatePatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FruitTree), nameof(FruitTree.dayUpdate)),
				postfix: new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		protected static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			if (Utility.AnyPlayerHasProfession("arborist", out int n) && __instance.daysUntilMature.Value % 7 == 0)
				__instance.daysUntilMature.Value -= n;
		}
		#endregion harmony patches
	}
}
