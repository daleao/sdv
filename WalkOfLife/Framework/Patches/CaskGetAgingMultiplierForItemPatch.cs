using Harmony;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CaskGetAgingMultiplierForItemPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CaskGetAgingMultiplierForItemPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Cask), nameof(Cask.GetAgingMultiplierForItem)),
				postfix: new HarmonyMethod(GetType(), nameof(CaskGetAgingMultiplierForItemPostfix))
			);
		}

		/// <summary>Patch for Oenologist faster wine aging.</summary>
		protected static void CaskGetAgingMultiplierForItemPostfix(ref float __result, Item item)
		{
			if (Utils.PlayerHasProfession("oenologist") && _IsWine(item as SObject))
			{
				__result *= 2;
			}
		}

		/// <summary>Whether a given object is wine.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWine(SObject obj)
		{
			return obj.ParentSheetIndex == 348;
		}
	}
}
