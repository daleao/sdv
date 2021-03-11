using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class AnimalHouseAddNewHatchedAnimalPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal AnimalHouseAddNewHatchedAnimalPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(AnimalHouse), nameof(AnimalHouse.addNewHatchedAnimal)),
				postfix: new HarmonyMethod(GetType(), nameof(AnimalHouseAddNewHatchedAnimalPostfix))
			);
		}

		/// <summary>Patch for Breeder newborn animals to have random starting friendship.</summary>
		protected static void AnimalHouseAddNewHatchedAnimalPostfix(ref AnimalHouse __instance)
		{
			Farmer who = Game1.getFarmer(__instance.getBuilding().owner.Value);
			if (!Utils.SpecificPlayerHasProfession("breeder", who)) return;

			FarmAnimal a = __instance.Animals[__instance.animalsThatLiveHere.Last()];
			if (a.age.Value == 0 && a.friendshipTowardFarmer.Value == 0)
			{
				Random r = new Random(__instance.GetHashCode() + a.GetHashCode());
				a.friendshipTowardFarmer.Value = r.Next(0, 200);
			}
		}
	}
}
