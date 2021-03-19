using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FishPondUpdateMaximumOccupancyPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FishPond), nameof(FishPond.UpdateMaximumOccupancy)),
				postfix: new HarmonyMethod(GetType(), nameof(FishPondUpdateMaximumOccupancyPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
		protected static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance, ref FishPondData ____fishPondData)
		{
			Farmer who = Game1.getFarmer(__instance.owner.Value);
			if (Utility.SpecificPlayerHasProfession("aquarist", who) && __instance.lastUnlockedPopulationGate.Value >= ____fishPondData.PopulationGates.Keys.Max())
				__instance.maxOccupants.Set(12);
		}
		#endregion harmony patches
	}
}
