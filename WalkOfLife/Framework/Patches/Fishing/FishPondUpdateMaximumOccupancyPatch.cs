using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FishPondUpdateMaximumOccupancyPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FishPondUpdateMaximumOccupancyPatch()
		{
			Original = typeof(FishPond).MethodNamed(nameof(FishPond.UpdateMaximumOccupancy));
			Postfix = new(GetType(), nameof(FishPondUpdateMaximumOccupancyPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
		[HarmonyPostfix]
		private static void FishPondUpdateMaximumOccupancyPostfix(ref FishPond __instance,
			FishPondData ____fishPondData)
		{
			if (__instance == null || ____fishPondData == null) return;

			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (owner.HasProfession("Aquarist") && __instance.lastUnlockedPopulationGate.Value >=
					____fishPondData.PopulationGates.Keys.Max())
					__instance.maxOccupants.Set(12);
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}