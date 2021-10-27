using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal AnimalHouseAddNewHatchedAnimalPatch()
		{
			Original = typeof(AnimalHouse).MethodNamed(nameof(AnimalHouse.addNewHatchedAnimal));
			Postfix = new(GetType(), nameof(AnimalHouseAddNewHatchedAnimalPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		[HarmonyPostfix]
		private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
		{
			try
			{
				var who = Game1.getFarmer(__instance.getBuilding().owner.Value);
				if (!who.HasProfession("Rancher")) return;

				var a = __instance.Animals?.Values.Last();
				if (a == null || a.age.Value != 0 || a.friendshipTowardFarmer.Value != 0) return;
				a.friendshipTowardFarmer.Value =
					200 + new Random(__instance.GetHashCode() + a.GetHashCode()).Next(-50, 51);
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}