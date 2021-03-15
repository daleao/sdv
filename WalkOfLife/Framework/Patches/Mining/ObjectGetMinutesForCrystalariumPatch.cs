using Harmony;
using StardewModdingAPI;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectGetMinutesForCrystalariumPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getMinutesForCrystalarium"),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to speed up Gemologist crystalarium processing time.</summary>
		protected static void ObjectGetMinutesForCrystalariumPostfix(ref int __result)
		{
			if (Globals.AnyPlayerHasProfession("gemologist", out int n)) __result = (int)(__result * Math.Pow(0.75, n));
		}
		#endregion harmony patches
	}
}
