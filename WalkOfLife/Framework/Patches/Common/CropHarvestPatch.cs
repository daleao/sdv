using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class CropHarvestPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
				transpiler: new HarmonyMethod(GetType(), nameof(CropHarvestTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to nerf Ecologist spring onion quality and increment forage counter + always allow iridium-quality crops for Agriculturist + Harvester bonus crop yield.</summary>
		private static IEnumerable<CodeInstruction> CropHarvestTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(Crop)}::{nameof(Crop.harvest)}.");

			var mb = original.GetMethodBody();
			if (mb == null) throw new ArgumentNullException($"{original.Name} method body returned null.");

			/// From: @object.Quality = 4
			/// To: @object.Quality = GetEcologistForageQuality()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // start of @object.Quality = 4
					)
					.ReplaceWith( // replace with custom quality
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist spring onion quality.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
			///		AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: @object.Stack)

			var dontIncreaseEcologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(Stats), nameof(Stats.ItemsForaged)).GetSetMethod())
					)
					.Advance()
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Ecologist"], dontIncreaseEcologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.Data))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.UniqueID))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Ldstr, operand: "/ItemsForaged"),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(string), nameof(string.Concat),
								new[] { typeof(string), typeof(string) })),
						new CodeInstruction(OpCodes.Ldloc_1), // loc 1 = @object
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(SObject), nameof(SObject.Stack)).GetGetMethod()),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(ModDataDictionaryExtensions), name: "IncrementField",
								new[] { typeof(ModDataDictionary), typeof(string), typeof(int) })),
						new CodeInstruction(OpCodes.Pop)
					)
					.AddLabels(dontIncreaseEcologistCounter);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Ecologist counter increment.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// From: if (fertilizerQualityLevel >= 3 && random2.NextDouble() < chanceForGoldQuality / 2.0)
			/// To: if (Game1.player.professions.Contains(< agriculturist_id >) || fertilizerQualityLevel >= 3) && random2.NextDouble() < chanceForGoldQuality / 2.0)

			var fertilizerQualityLevel = mb.LocalVariables[8];
			var random2 = mb.LocalVariables[9];
			var isAgriculturist = iLGenerator.DefineLabel();
			try
			{
				Helper.AdvanceUntil( // find index of Crop.fertilizerQualityLevel >= 3
						new CodeInstruction(OpCodes.Ldloc_S, operand: fertilizerQualityLevel),
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Blt)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Agriculturist"],
						branchDestination: isAgriculturist, useBrtrue: true)
					.AdvanceUntil( // find start of dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: random2)
					)
					.AddLabels(isAgriculturist); // branch here if player is agriculturist
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Agriculturist crop harvest quality.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Injected: if (junimoHarvester == null && Game1.player.professions.Contains(<harvester_id>) && r.NextDouble() < 0.1) numToHarvest++

			var numToHarvest = mb.LocalVariables[6];
			var dontIncreaseNumToHarvest = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, operand: numToHarvest) // find index of numToHarvest++
					)
					.ToBufferUntil( // copy this segment
						stripLabels: true,
						advance: false,
						new CodeInstruction(OpCodes.Stloc_S, operand: numToHarvest)
					)
					.FindNext(
						new CodeInstruction(OpCodes.Ldloc_S, operand: random2) // find an instance of accessing the rng
					)
					.GetOperand(out var r2) // copy operand object
					.FindLast( // find end of chanceForExtraCrops while loop
						new CodeInstruction(OpCodes.Ldfld,
							AccessTools.Field(typeof(Crop), nameof(Crop.chanceForExtraCrops)))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // beginning of the next segment
					)
					.GetLabels(out var labels) // copy existing labels
					.SetLabels(dontIncreaseNumToHarvest) // branch here if shouldn't apply Harvester bonus
					.Insert( // insert check if junimoHarvester == null
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4),
						new CodeInstruction(OpCodes.Brtrue_S, operand: dontIncreaseNumToHarvest)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Harvester"],
						dontIncreaseNumToHarvest)
					.Insert( // insert dice roll
						new CodeInstruction(OpCodes.Ldloc_S, operand: (LocalBuilder)r2),
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Method(typeof(Random), nameof(Random.NextDouble))),
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.1),
						new CodeInstruction(OpCodes.Bge_Un_S, operand: dontIncreaseNumToHarvest)
					)
					.InsertBuffer() // insert numToHarvest++
					.Return(3) // return to first inserted instruction
					.SetLabels(labels); // restore original labels to this segment
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