using Harmony;
using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

using static TheLion.AwesomeProfessions.Framework.Utils;

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

		/// <summary>Patch to add extra crop yield chance to Harvester.</summary>
		private static bool CropHarvestPrefix(ref Crop __instance, JunimoHarvester junimoHarvester = null)
		{
			if (junimoHarvester == null && PlayerHasProfession("harvester"))
			{
				__instance.chanceForExtraCrops.Value += 0.10;
			}

			return true; // run original logic
		}

		/// <summary>Patch to add dice roll to Ecologist forage quality + always allow iridum-quality crops for Agriculturist.</summary>
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Crop)}::{nameof(Crop.harvest)}.");

			/// From: @object.Quality = 4
			/// To: if (random.NextDouble() < (double)((float)Game1.player.ForagingLevel / 30f) @object.Quality = 4 else @object.Quality = 2

			Label rollFailed = iLGenerator.DefineLabel();
			try
			{
				_helper
					.FindProfessionCheck(Farmer.botanist)							// find the index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldloc_1)						// start of @object.Quality = 4
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldloc_3)						// start of dice roll
					)
					.ToBufferUntil(
						stripLabels: true,
						new CodeInstruction(OpCodes.Bge_Un)							// finish dice roll
					)
					.Return()
					.InsertBuffer()													// copy dice roll
					.Retreat()
					.SetOperand(rollFailed)											// set branch destination for failed roll
					.Advance()
					.ToBufferUntil(
						stripLabels: false,
						advance: true,
						new CodeInstruction(OpCodes.Br)								// finish setting quality level
					)
					.InsertBuffer()													// copy settings quality level
					.Return()
					.AddLabel(rollFailed)											// the destination for failed roll
					.Advance()
					.SetOpCode(OpCodes.Ldc_I4_2);									// change quality level to 2
			}
			catch(Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Ecologist forage quality.\nHelper returned {ex}");
			}

			_helper.Backup();

			/// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
			/// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

			Label isAgriculturist = iLGenerator.DefineLabel();
			try
			{
				_helper.
					AdvanceUntil(																// find the index of Crop.fertilizerQualityLevel >= 3
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(Int32)} (8)"),	// local 8 = Crop.fertilizerQualityLevel
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Blt)
					)
					.InsertProfessionCheck(ProfessionsMap.Forward["agriculturist"], branchDestination: isAgriculturist, branchIfTrue: true)
					.AdvanceUntil(																// find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: $"{typeof(Random)} (9)")	// local 9 = System.Random random2
					)
					.AddLabel(isAgriculturist);													// the destination if player is agriculturist
			}
			catch (Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Agriculturist crop harvest quality.\nHelper returned {ex}");
			}

			return _helper.Flush();
		}
	}
}
