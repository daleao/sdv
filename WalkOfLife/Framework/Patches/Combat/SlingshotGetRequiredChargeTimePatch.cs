using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Tools;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SlingshotGetRequiredChargeTimePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotGetRequiredChargeTimePatch()
		{
			Original = typeof(Slingshot).MethodNamed(nameof(Slingshot.GetRequiredChargeTime));
			Postfix = new(GetType(), nameof(SlingshotGetRequiredChargeTimePostfix));
		}

		#region harmony patches

		/// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
		[HarmonyPrefix]
		private static void SlingshotGetRequiredChargeTimePostfix(ref float __result)
		{
			try
			{
				if (ModEntry.SuperModeIndex != Util.Professions.IndexOf("Desperado")) return;

				__result *= Util.Professions.GetCooldownOrChargeTimeReduction();
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}