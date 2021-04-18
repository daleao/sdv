using Harmony;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
				postfix: new HarmonyMethod(GetType(), nameof(AnimalHouseAddNewHatchedAnimalPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		private static void AnimalHouseAddNewHatchedAnimalPostfix(ref AnimalHouse __instance)
		{
			try
			{
				var who = Game1.getFarmer(__instance.getBuilding().owner.Value);
				if (!Utility.SpecificPlayerHasProfession("Rancher", who)) return;

				var a = __instance.Animals?.Values.Last();
				if (a == null || a.age.Value != 0 || a.friendshipTowardFarmer.Value != 0) return;
				a.friendshipTowardFarmer.Value = new Random(__instance.GetHashCode() + a.GetHashCode()).Next(0, 200);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(AnimalHouseAddNewHatchedAnimalPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}