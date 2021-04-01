using Harmony;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class CropHarvestPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
				transpiler: new HarmonyMethod(GetType(), nameof(CropHarvestTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for Harvester extra crop yield.</summary>
		private static bool CropHarvestPrefix(ref Crop __instance, ref bool __state, JunimoHarvester junimoHarvester = null)
		{
			if (junimoHarvester == null && Utility.LocalFarmerHasProfession("harvester"))
			{
				__instance.chanceForExtraCrops.Value += 0.1;
				__state = true;
			}
			return true; // run original logic
		}

		/// <summary>Patch to nerf Ecologist spring onion quality + always allow iridium-quality crops for Agriculturist.</summary>
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(Crop)}::{nameof(Crop.harvest)}.");

			/// From: @object.Quality = 4
			/// To: @object.Quality = _GetForageQualityForEcologist()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist)       // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)   // start of @object.Quality = 4
					)
					.ReplaceWith(                               // replace with custom quality
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
			/// To: if (Game1.player.professions.Contains(<agriculturist_id>) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

			var fertilizerQualityLevel = original.GetMethodBody().LocalVariables[8];
			var random2 = original.GetMethodBody().LocalVariables[9];
			Label isAgriculturist = iLGenerator.DefineLabel();
			try
			{
				Helper.
					AdvanceUntil(                                               // find index of Crop.fertilizerQualityLevel >= 3
						new CodeInstruction(OpCodes.Ldloc_S, operand: fertilizerQualityLevel),
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Blt)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["agriculturist"], branchDestination: isAgriculturist, branchIfTrue: true)
					.AdvanceUntil(                                              // find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: random2)
					)
					.AddLabels(isAgriculturist);                                // branch here if player is agriculturist
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}").Restore();
			}

			/// Injected: if (junimoHarvester == null && Game1.player.professions.Contains(<harvester_id>) && r.NextDouble() < 0.1) numToHarvest++

			var numToHarvest = original.GetMethodBody().LocalVariables[6];
			Label dontIncreaseNumToHarvest = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, operand: numToHarvest)     // find index of numToHarvest++
					)
					.ToBufferUntil(                                                     // copy this segment
						stripLabels: true,
						advance: false,
						new CodeInstruction(OpCodes.Stloc_S, operand: numToHarvest)
					)
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, operand: random2)          // find an instance of accessing the rng
					)
					.GetOperand(out object r2)                                          // copy operand object
					.FindLast(                                                          // find end of chanceForExtraCrops while loop
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Crop), nameof(Crop.chanceForExtraCrops)))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0)                            // beginning of the next segment
					)
					.GetLabels(out var labels)                                          // copy existing labels
					.SetLabels(dontIncreaseNumToHarvest)                                // branch here if shouldn't apply Harvester bonus
					.Insert(                                                            // insert check if junimoHarvester == null
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4),
						new CodeInstruction(OpCodes.Brtrue_S, operand: dontIncreaseNumToHarvest)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["harvester"], dontIncreaseNumToHarvest)
					.Insert(                                                            // insert dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: (LocalBuilder)r2),
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Random), nameof(Random.NextDouble))),
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.1),
						new CodeInstruction(OpCodes.Bge_Un_S, operand: dontIncreaseNumToHarvest)
					)
					.InsertBuffer()                                                     // insert numToHarvest++
					.Return(3)                                                          // return to first inserted instruction
					.SetLabels(labels);                                                 // restore original labels to this segment
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Harvester extra crop yield.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}