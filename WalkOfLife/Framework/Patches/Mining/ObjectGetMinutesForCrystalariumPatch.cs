using Harmony;
using StardewValley;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectGetMinutesForCrystalariumPatch : IPatch
	{
		/// <inheritdoc/>
		public void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), name: "getMinutesForCrystalarium"),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
		private static void ObjectGetMinutesForCrystalariumPostfix(ref SObject __instance, ref int __result)
		{
			var owner = Game1.getFarmer(__instance.owner.Value);
			if (Utility.SpecificPlayerHasProfession("Gemologist", owner)) __result = (int)(__result * 0.75);
		}

		#endregion harmony patches
	}
}