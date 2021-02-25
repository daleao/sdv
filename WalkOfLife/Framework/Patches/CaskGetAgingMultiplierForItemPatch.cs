using Harmony;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using SObject = StardewValley.Object;

using static TheLion.AwesomeProfessions.Framework.Utils;

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

		/// <summary>Patch to speed up Oenologist wine aging.</summary>
		protected static void CaskGetAgingMultiplierForItemPostfix(ref float __result, Item item)
		{
			if (PlayerHasProfession("oenologist") && IsWine(item as SObject))
			{
				__result *= 2;
			}
		}
	}
}
