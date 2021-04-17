using Harmony;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class MineShaftCheckStoneForItemsPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkStoneForItems)),
				transpiler: new HarmonyMethod(GetType(), nameof(MineShaftCheckStoneForItemsTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double geode chance + remove Prospetor double coal chance.</summary>
		private static IEnumerable<CodeInstruction> MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(MineShaft)}::{nameof(MineShaft.checkStoneForItems)}.");

			/// Injected: if (who.professions.Contains(<spelunker_id>) chanceForLadderDown += GetBonusLadderDownChance()

			var resumeExecution = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindFirst( // find ladder spawn segment
						new CodeInstruction(OpCodes.Ldfld,
							AccessTools.Field(typeof(MineShaft), name: "ladderHasSpawned"))
					)
					.Retreat()
					.GetLabels(out var labels) // copy labels
					.StripLabels()
					.AddLabels(resumeExecution) // branch here to resume execution
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4) // arg 4 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Utility.ProfessionMap.Forward["Spelunker"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_3), // local 3 = chanceForLadderDown
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetSpelunkerBonusLadderDownChance))),
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Stloc_3)
					)
					.Return(3)
					.AddLabels(labels); // restore labels to inserted spelunker check
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Spelunker bonus ladder down chance.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)) ...

			var i = 0;
			repeat1:
			try
			{
				Helper // find index of geologist check
					.FindProfessionCheck(Farmer.geologist, fromCurrentIndex: i != 0)
					.Retreat()
					.GetLabels(out var labels) // copy labels
					.StripLabels() // remove labels from here
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the false case branch
					)
					.GetOperand(out var isNotGeologist) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotGeologist)
					)
					.Retreat()
					.AddLabels(labels)
					.Advance(2); // restore labels to inserted branch
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Geologist paired gem chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				Helper.Backup();
				goto repeat1;
			}

			Helper.Backup();

			/// From: random.NextDouble() < <value> * (1.0 + chanceModifier) * (double)(!who.professions.Contains(<excavator_id>) ? 1 : 2)
			/// To: random.NextDouble() < <value> * (1.0 + chanceModifier)

			i = 0;
			repeat2:
			try
			{
				Helper // find index of excavator check
					.FindProfessionCheck(Farmer.excavator, fromCurrentIndex: i != 0)
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul) // remove this check
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Excavator double geode chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				Helper.Backup();
				goto repeat2;
			}

			Helper.Backup();

			/// From: if (random.NextDouble() < 0.25 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2))
			/// To: if (random.NextDouble() < 0.25)

			try
			{
				Helper
					.FindProfessionCheck(Farmer.burrower, fromCurrentIndex: true) // find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul) // remove this check
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}