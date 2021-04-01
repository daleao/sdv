using Harmony;
using StardewValley;
using StardewValley.Objects;

namespace TheLion.AwesomeProfessions
{
	internal class CaskPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Cask), nameof(Cask.performObjectDropInAction)),
				postfix: new HarmonyMethod(GetType(), nameof(CaskPerformObjectDropInActionPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Oenologist faster wine aging.</summary>
		private static void CaskPerformObjectDropInActionPostfix(ref Cask __instance, Item dropIn, Farmer who)
		{
			if (Utility.SpecificFarmerHasProfession("oenologist", who) && Utility.IsWine(dropIn))
				__instance.agingRate.Value *= 2f;
		}

		#endregion harmony patches
	}
}