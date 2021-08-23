using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FarmerHasOrWillReceiveMailPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerHasOrWillReceiveMailPatch()
		{
			Original = typeof(Farmer).MethodNamed(nameof(Farmer.hasOrWillReceiveMail));
			Prefix = new HarmonyMethod(GetType(), nameof(FarmerHasOrWillReceiveMailPrefix));
		}

		#region harmony patches

		/// <summary>Patch to allow receiving multiple letters from the FRS and the SWA.</summary>
		[HarmonyPrefix]
		private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
		{
			try
			{
				if (!id.Equals($"{ModEntry.UniqueID}/ConservationistTaxNotice"))
					return true; // run original logic

				__result = false;
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}