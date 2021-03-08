using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class HoeDirtApplySpeedIncreasesPatch : BasePatch
	{
		private const int _speedGroId = 465, _deluxeSpeedGroId = 466, _hyperSpeedGroId = 918;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal HoeDirtApplySpeedIncreasesPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(HoeDirt), name: "applySpeedIncreases"),
				prefix: new HarmonyMethod(GetType(), nameof(HoeDirtApplySpeedIncreasesPrefix))
			);
		}

		/// <summary>Patch to make Agriculturist crop growth speed bonus global.</summary>
		protected static bool HoeDirtApplySpeedIncreasesPrefix(ref HoeDirt __instance, Farmer who)
		{
			if (__instance.crop == null)
			{
				return false; // don't run original logic
			}

			bool isThereAnyAgriculturist = Utils.AnyPlayerHasProfession("agriculturist", out int n);
			bool shouldApplyPaddyBonus = __instance.currentLocation != null && __instance.paddyWaterCheck(__instance.currentLocation, __instance.currentTileLocation);
			
			if (!(__instance.fertilizer.Value.AnyOf(_speedGroId, _deluxeSpeedGroId, _hyperSpeedGroId) || isThereAnyAgriculturist || shouldApplyPaddyBonus))
			{
				return false; // don't run original logic
			}

			__instance.crop.ResetPhaseDays();
			int totalDaysOfCropGrowth = 0;
			for (int i = 0; i < __instance.crop.phaseDays.Count - 1; ++i)
			{
				totalDaysOfCropGrowth += __instance.crop.phaseDays[i];
			}

			float speedIncrease = __instance.fertilizer.Value switch
			{
				_speedGroId => 0.1f,
				_deluxeSpeedGroId => 0.25f,
				_hyperSpeedGroId => 0.33f,
				_ => 0f
			};

			if (shouldApplyPaddyBonus)
			{
				speedIncrease += 0.25f;
			}

			if (isThereAnyAgriculturist)
			{
				speedIncrease += 0.1f * n;
			}

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
					if (daysToRemove <= 0)
					{
						break;
					}
				}
				++tries;
			}

			return false; // don't run original logic
		}
	}
}
