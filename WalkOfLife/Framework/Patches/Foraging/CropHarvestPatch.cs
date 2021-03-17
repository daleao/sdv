using Harmony;
using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class CropHarvestPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CropHarvestPatch(IMonitor monitor)
			: base(monitor)
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
				transpiler: new HarmonyMethod(GetType(), nameof(CropHarvestTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(CropHarvestPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Harvester extra crop yield.</summary>
		private static bool CropHarvestPrefix(ref Crop __instance, JunimoHarvester junimoHarvester = null)
		{
			if (junimoHarvester == null && Utility.LocalPlayerHasProfession("harvester"))
				__instance.chanceForExtraCrops.Value += 0.10;

			return true; // run original logic
		}

		/// <summary>Patch to nerf Ecologist spring onion quality + always allow iridum-quality crops for Agriculturist.</summary>
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Crop)}::{nameof(Crop.harvest)}.");

			/// From: @object.Quality = 4
			/// To: @object.Quality = _GetForageQualityForEcologist()

			try
			{
				_helper
					.FindProfessionCheck(Farmer.botanist)		// find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)	// start of @object.Quality = 4
					)
					.ReplaceWith(								// replace with custom quality
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					);
			}
			catch(Exception ex)
			{
				_helper.Error($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}").Restore();
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
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["agriculturist"], branchDestination: isAgriculturist, branchIfTrue: true)
					.AdvanceUntil(																// find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(Random)} (9)")	// local 9 = System.Random random2
					)
					.AddLabel(isAgriculturist);													// branch here if player is agriculturist
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch to count foraged spring onions for Ecologist.</summary>
		private static void CropHarvestPostfix(ref Crop __instance)
		{
			if (__instance.forageCrop.Value) ++AwesomeProfessions.Data.ItemsForaged;
		}
		#endregion harmony patches
	}
}
