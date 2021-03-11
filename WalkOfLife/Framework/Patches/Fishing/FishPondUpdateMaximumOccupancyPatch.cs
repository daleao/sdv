using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System.Linq;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FishPondUpdateMaximumOccupancyPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FishPond), nameof(FishPond.UpdateMaximumOccupancy)),
				postfix: new HarmonyMethod(GetType(), nameof(FishPondUpdateMaximumOccupancyPostfix))
			);
		}

		/// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
		protected static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance, ref FishPondData ____fishPondData)
		{
			Farmer who = Game1.getFarmer(__instance.owner.Value);
			if (__instance.lastUnlockedPopulationGate.Value >= ____fishPondData.PopulationGates.Keys.Max() && Utils.SpecificPlayerHasProfession("aquarist", who))
				__instance.maxOccupants.Set(12);
		}
	}
}
