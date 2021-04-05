using Harmony;
using StardewValley.TerrainFeatures;
using System;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class HoeDirtApplySpeedIncreasesPatch : BasePatch
	{
		private const int _SpeedGroId = 465, _DeluxeSpeedGroId = 466, _HyperSpeedGroId = 918;

		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(HoeDirt), name: "applySpeedIncreases"),
				prefix: new HarmonyMethod(GetType(), nameof(HoeDirtApplySpeedIncreasesPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to globalize Agriculturist crop growth speed bonus.</summary>
		private static bool HoeDirtApplySpeedIncreasesPrefix(ref HoeDirt __instance)
		{
			if (__instance.crop == null)
				return false; // don't run original logic

			bool anyPlayerIsAgriculturist = Utility.AnyPlayerHasProfession("Agriculturist", out int n);
			bool shouldApplyPaddyBonus = __instance.currentLocation != null && __instance.paddyWaterCheck(__instance.currentLocation, __instance.currentTileLocation);

			if (!(__instance.fertilizer.Value.AnyOf(_SpeedGroId, _DeluxeSpeedGroId, _HyperSpeedGroId) || anyPlayerIsAgriculturist || shouldApplyPaddyBonus))
				return false; // don't run original logic

			__instance.crop.ResetPhaseDays();
			int totalDaysOfCropGrowth = 0;
			for (int i = 0; i < __instance.crop.phaseDays.Count - 1; ++i)
				totalDaysOfCropGrowth += __instance.crop.phaseDays[i];

			float speedIncrease = __instance.fertilizer.Value switch
			{
				_SpeedGroId => 0.1f,
				_DeluxeSpeedGroId => 0.25f,
				_HyperSpeedGroId => 0.33f,
				_ => 0f
			};

			if (shouldApplyPaddyBonus) speedIncrease += 0.25f;

			if (anyPlayerIsAgriculturist) speedIncrease += 0.1f * n;

			int daysToRemove = (int)Math.Ceiling(totalDaysOfCropGrowth * speedIncrease);
			int tries = 0;
			while (daysToRemove > 0 && tries < 3)
			{
				for (int i = 0; i < __instance.crop.phaseDays.Count; ++i)
				{
					if ((i > 0 || __instance.crop.phaseDays[i] > 1) && __instance.crop.phaseDays[i] != 99999)
					{
						--__instance.crop.phaseDays[i];
						--daysToRemove;
					}

					if (daysToRemove <= 0) break;
				}
				++tries;
			}

			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}