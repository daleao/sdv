using Harmony;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class MineShaftCheckStoneForItemsPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal MineShaftCheckStoneForItemsPatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkStoneForItems)),
				transpiler: new HarmonyMethod(GetType(), nameof(MineShaftCheckStoneForItemsTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double geode chance + remove Prospetor double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(MineShaft)}::{nameof(MineShaft.checkStoneForItems)}.");

			/// Injected: if (who.professions.Contains(<spelunker_id>) chanceForLadderDown += GetBonusLadderDownChance()

			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_Helper
					.FindFirst(													// find ladder spawn segment
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MineShaft), name: "ladderHasSpawned"))
					)
					.Retreat()
					.GetLabels(out var labels)									// copy labels
					.StripLabels()
					.AddLabel(resumeExecution)									// branch here to resume execution
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4)	// arg 4 = Farmer who
					)
					.InsertProfessionCheckForSpecificPlayer(Utility.ProfessionMap.Forward["spelunker"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_3),					// local 3 = chanceForLadderDown
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.GetSpelunkerBonusLadderDownChance))),
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Stloc_3)
					)
					.Return(3)
					.AddLabels(labels);											// restore labels to inserted spelunker check
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while adding Spelunker bonus ladder down chance.\nHelper returned {ex}").Restore();
			}

			_Helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)) ...

			int i = 0;
			repeat1:
			try
			{
				_Helper											// find index of geologist check
					.FindProfessionCheck(Farmer.geologist, fromCurrentIndex: i != 0)
					.Retreat()
					.GetLabels(out var labels)					// copy labels
					.StripLabels()								// remove labels from here
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the false case branch
					)
					.GetOperand(out object isNotGeologist)		// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotGeologist)
					)
					.Retreat()
					.AddLabels(labels)
					.Advance(2);								// restore labels to inserted branch
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Geologist paired gem chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				_Helper.Backup();
				goto repeat1;
			}

			_Helper.Backup();

			/// From: random.NextDouble() < <value> * (1.0 + chanceModifier) * (double)(!who.professions.Contains(<excavator_id>) ? 1 : 2)
			/// To: random.NextDouble() < <value> * (1.0 + chanceModifier)

			i = 0;
			repeat2:
			try
			{
				_Helper										// find index of excavator check
					.FindProfessionCheck(Farmer.excavator, fromCurrentIndex: i != 0)
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Excavator double geode chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				_Helper.Backup();
				goto repeat2;
			}

			_Helper.Backup();

			/// From: if (random.NextDouble() < 0.25 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2))
			/// To: if (random.NextDouble() < 0.25)

			try
			{
				_Helper
					.FindProfessionCheck(Farmer.burrower, fromCurrentIndex: true)	// find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul)							// remove this check
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
