using Harmony;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class FarmerHasOrWillReceiveMailPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Farmer), nameof(Farmer.hasOrWillReceiveMail)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmerHasOrWillReceiveMailPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to allow receiving multiple letters from the FRS and the SWA.</summary>
		private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
		{
			try
			{
				if (!id.Equals($"{AwesomeProfessions.UniqueID}/ConservationistTaxNotice"))
					return true; // run original logic

				__result = false;
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FarmerHasOrWillReceiveMailPrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}