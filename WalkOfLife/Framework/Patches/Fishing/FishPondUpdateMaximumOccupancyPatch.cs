using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FishPond), nameof(FishPond.UpdateMaximumOccupancy)),
				postfix: new HarmonyMethod(GetType(), nameof(FishPondUpdateMaximumOccupancyPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
		private static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance, ref FishPondData ____fishPondData)
		{
			if (__instance == null || ____fishPondData == null) return;

			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (Utility.SpecificPlayerHasProfession("Aquarist", owner) && __instance.lastUnlockedPopulationGate.Value >= ____fishPondData.PopulationGates.Keys.Max())
					__instance.maxOccupants.Set(12);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FishPondUpdateMaximumOccupancyPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}