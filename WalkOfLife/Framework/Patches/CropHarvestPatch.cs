using Harmony;
using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CropHarvestPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CropHarvestPatch(ModConfig config, IMonitor monitor)
			: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
				prefix: new HarmonyMethod(GetType(), nameof(CropHarvestPrefix)),
				transpiler: new HarmonyMethod(GetType(), nameof(CropHarvestTranspiler))
			);
		}

		/// <summary>Patch for Harvester extra crop yield.</summary>
		private static bool CropHarvestPrefix(ref Crop __instance, JunimoHarvester junimoHarvester = null)
		{
			if (junimoHarvester == null && Utils.PlayerHasProfession("harvester"))
			{
				__instance.chanceForExtraCrops.Value += 0.10;
			}

			return true; // run original logic
		}

		/// <summary>Patch to nerf Ecologist forage quality + always allow iridum-quality crops for Agriculturist.</summary>
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Crop)}::{nameof(Crop.harvest)}.");

			/// From: @object.Quality = 4
			/// To: @object.Quality = GetForageQuality()

			Label rollFailed = iLGenerator.DefineLabel();
			try
			{
				_helper
					.FindProfessionCheck(Farmer.botanist)							// find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)						// start of @object.Quality = 4
					)
					.ReplaceWith(													// replace with custom quality
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(CropHarvestPatch), nameof(CropHarvestPatch.GetForageQualityForEcologist)))
					);
			}
			catch(Exception ex)
			{
				_helper.Error($"Failed while patching Ecologist forage quality.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
			/// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

			Label isAgriculturist = iLGenerator.DefineLabel();
			try
			{
				_helper.
					AdvanceUntil(																// find index of Crop.fertilizerQualityLevel >= 3
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(int)} (8)"),	// local 8 = Crop.fertilizerQualityLevel
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Blt)
					)
					.InsertProfessionCheck(Utils.ProfessionsMap.Forward["agriculturist"], branchDestination: isAgriculturist, branchIfTrue: true)
					.AdvanceUntil(																// find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(Random)} (9)")	// local 9 = System.Random random2
					)
					.AddLabel(isAgriculturist);													// branch here if player is agriculturist
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Agriculturist crop harvest quality.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		protected static int GetForageQualityForEcologist()
		{
			return ModEntry.Data.MineralsCollected < _config.EcologistConfig.ForagesNeededForBestQuality ? (ModEntry.Data.ItemsForaged < _config.EcologistConfig.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}
	}
}
