using Harmony;
using StardewValley.Objects;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class CaskPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CaskPerformObjectDropInActionPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Cask), nameof(Cask.performObjectDropInAction)),
				postfix: new HarmonyMethod(GetType(), nameof(CaskPerformObjectDropInActionPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Oenologist faster wine aging.</summary>
		protected static void CaskPerformObjectDropInActionPostfix(ref Cask __instance, Item dropIn, Farmer who)
		{
			if (Utility.SpecificPlayerHasProfession("oenologist", who) && Utility.IsWine(dropIn))
				__instance.agingRate.Value *= 2f;
		}
		#endregion harmony patches
	}
}
