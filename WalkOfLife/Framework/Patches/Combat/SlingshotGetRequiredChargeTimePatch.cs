using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SlingshotGetRequiredChargeTimePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotGetRequiredChargeTimePatch()
		{
			Original = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
			Postfix = new(AccessTools.Method(GetType(), nameof(SlingshotGetRequiredChargeTimePostfix)));
		}

		#region harmony patches

		/// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
		[HarmonyPostfix]
		private static void SlingshotGetRequiredChargeTimePostfix(ref float __result)
		{
			if (ModEntry.SuperModeIndex != Utility.Professions.IndexOf("Desperado")) return;
			__result *= Utility.Professions.GetCooldownOrChargeTimeReduction();
		}

		#endregion harmony patches
	}
}