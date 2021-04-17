using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Common;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationCheckActionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: _TargetMethod(),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationCheckActionTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective mod data fields.</summary>
		private static IEnumerable<CodeInstruction> GameLocationCheckActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.checkAction)}.");

			/// From: if (who.professions.Contains(<botanist_id>) && objects[key].isForage()) objects[key].Quality = 4
			/// To: if (who.professions.Contains(<ecologist_id>) && objects[key].isForage() && !IsForagedMineral(objects[key]) objects[key].Quality = GetEcologistForageQuality()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // start of objects[key].isForage() check
					)
					.ToBufferUntil( // copy objects[key]
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(OverlaidDictionary), name: "Item").GetGetMethod())
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // end of check
					)
					.GetOperand(out var shouldntSetCustomQuality) // copy failed check branch destination
					.Advance()
					.InsertBuffer() // insert objects[key]
					.Insert( // check if is foraged mineral and branch if true
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.IsForagedMineral))),
						new CodeInstruction(OpCodes.Brtrue_S, operand: (Label)shouldntSetCustomQuality)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // start of objects[key].Quality = 4
					)
					.ReplaceWith( // replace with custom quality
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist forage quality.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Injected: else if (who.professions.Contains(<gemologist_id>) && IsForagedMineral(objects[key])) objects[key].Quality = GetMineralQualityForGemologist()

			var gemologistCheck = iLGenerator.DefineLabel();
			try
			{
				Helper

					.FindProfessionCheck(Farmer.botanist) // return to botanist check
					.Retreat(2) // retreat to start of check
					.ToBufferUntil( // copy entire section until done setting quality
						stripLabels: true,
						advance: false,
						new CodeInstruction(OpCodes.Br)
					)
					.AdvanceUntil( // change previous section branch destinations to injected section
						new CodeInstruction(OpCodes.Brfalse)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brtrue_S)
					)
					.SetOperand(gemologistCheck)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Br)
					)
					.Advance()
					.InsertBuffer() // insert copy
					.Return()
					.AddLabels(gemologistCheck) // add destination label for branches from previous section
					.AdvanceUntil( // find repeated botanist check
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 16)
					)
					.SetOperand(Utility.ProfessionMap.Forward["Gemologist"]) // replace with gemologist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)
					)
					.GetOperand(out var shouldntSetCustomQuality) // copy next section branch destination
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldarg_0) // start of call to isForage()
					)
					.RemoveUntil( // right before call to IsForagedMineral()
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(OverlaidDictionary), name: "Item").GetGetMethod())
					)
					.Advance()
					.ReplaceWith( // remove 'not' and set correct branch destination
						new CodeInstruction(OpCodes.Brfalse_S, operand: (Label)shouldntSetCustomQuality)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					)
					.SetOperand(AccessTools.Method(typeof(Utility),
						nameof(Utility.GetGemologistMineralQuality))); // set correct custom quality method call
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Gemologist foraged mineral quality.\nHelper returned {ex}").Restore();
			}

			/// Injected: IncrementModData(objects[key], this, who)

			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(Stats), nameof(Stats.ItemsForaged)).GetSetMethod())
					)
					.Advance()
					.InsertBuffer(index: 6, length: 5) // SObject objects[key]
					.InsertBuffer(index: 6, length: 2) // GameLocation this
					.InsertBuffer(index: 0, length: 2) // Farmer who
					.Insert(
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(GameLocationCheckActionPatch), nameof(_IncrementModData)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Ecologist and Gemologist counter increment.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		/// <summary>Get the inner method to patch.</summary>
		private static MethodBase _TargetMethod()
		{
			var targetMethod = typeof(GameLocation).InnerMethodsStartingWith("<checkAction>b__0").First();
			if (targetMethod == null)
				throw new MissingMethodException("Target method '<checkAction>b__0' was not found.");

			return targetMethod;
		}

		private static void _IncrementModData(SObject obj, GameLocation location, Farmer who)
		{
			if (Utility.SpecificPlayerHasProfession("Ecologist", who) && obj.isForage(location) && !Utility.IsForagedMineral(obj))
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: 1);
			else if (Utility.SpecificPlayerHasProfession("Gemologist", who) && Utility.IsForagedMineral(obj))
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1);
		}

		#endregion private methods
	}
}