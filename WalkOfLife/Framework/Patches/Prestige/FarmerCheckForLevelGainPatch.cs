using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Patches.Prestige
{
	[UsedImplicitly]
	internal class FarmerCheckForLevelGainPatch : BasePatch
	{
		private const int EXP_PER_LEVEL_I = 4000;
		private const int PRESTIGE_GATE_I = 15000;

		/// <summary>Construct an instance.</summary>
		internal FarmerCheckForLevelGainPatch()
		{
			Original = RequireMethod<Farmer>(nameof(Farmer.checkForLevelGain));
		}

		#region harmony patches

		/// <summary>Patch to allow level increase up to 20.</summary>
		[HarmonyPrefix]
		private static void FarmerCheckForLevelGainPostfix(ref int __result, int oldXP, int newXP)
		{
			if (!ModEntry.Config.EnablePrestige || !ModEntry.Config.EnableExtendedProgression) return;
			
			for (var i = 1; i <= 10; ++i)
			{
				var requiredExpForThisLevel = PRESTIGE_GATE_I + EXP_PER_LEVEL_I * i;
				if (oldXP >= requiredExpForThisLevel) continue;
				if (newXP < requiredExpForThisLevel) return;

				__result = i + 10;
			}
		}

		#endregion harmony patches
	}
}