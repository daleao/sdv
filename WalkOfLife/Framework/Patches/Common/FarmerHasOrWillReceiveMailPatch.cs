using Harmony;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class FarmerHasOrWillReceiveMailPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerHasOrWillReceiveMailPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Farmer), nameof(Farmer.hasOrWillReceiveMail)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmerHasOrWillReceiveMailPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to allow receiving multiple letters from the FRS and the SWA.</summary>
		protected static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
		{
			if (id.Equals("ConservationistTaxNotice") || id.Equals("OenologistAwardNotice"))
			{
				__result = false;
				return false; // don't run original logic
			}

			return true; // run original logic
		}
		#endregion harmony patches
	}
}
