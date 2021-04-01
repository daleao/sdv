using Harmony;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectGetMinutesForCrystalariumPatch : IPatch
	{
		/// <inheritdoc/>
		public void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getMinutesForCrystalarium"),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
		private static void ObjectGetMinutesForCrystalariumPostfix(ref int __result)
		{
			if (Utility.AnyFarmerHasProfession("gemologist", out int n)) __result = (int)(__result * Math.Pow(0.75, n));
		}

		#endregion harmony patches
	}
}