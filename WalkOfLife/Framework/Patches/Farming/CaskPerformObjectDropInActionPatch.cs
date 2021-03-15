using Harmony;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CaskPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CaskPerformObjectDropInActionPatch(IMonitor monitor)
		: base(monitor) { }

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
			if (Globals.SpecificPlayerHasProfession("oenologist", who) && _IsWine(dropIn))
				__instance.agingRate.Value *= 2f;
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given object is wine.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWine(Item item)
		{
			return item?.ParentSheetIndex == 348;
		}
		#endregion private methods
	}
}
