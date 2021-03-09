using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class MineShaftCheckStoneForItemsPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal MineShaftCheckStoneForItemsPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
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

		/// <summary>Patch for Spelunker ladder down chance bonus + remove Geologist paired gem chance + remove Excavator double geode chance + remove Prospetor double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(MineShaft)}::{nameof(MineShaft.checkStoneForItems)}.");

			/// Injected: if (who.professions.Contains(<spelunker_id>) chanceForLadderDown += GetBonusLadderDownChance()

			Label resumeExecution1 = iLGenerator.DefineLabel();
			try
			{
				_helper
					.FindFirst(													// find ladder spawn segment
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MineShaft), name: "ladderHasSpawned"))
					)
					.Retreat()
					.GetLabels(out var labels)
					.StripLabels()
					.AddLabel(resumeExecution1)									// branch here to resume execution
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4)	// arg 4 = Farmer who
					)
					.InsertProfessionCheckForSpecificPlayer(Utils.ProfessionsMap.Forward["spelunker"], resumeExecution1)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_3),					// local 3 = chanceForLadderDown
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MineShaftCheckStoneForItemsPatch), nameof(MineShaftCheckStoneForItemsPatch._GetBonusLadderDownChance))),
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Stloc_3)
					)
					.Return(3)
					.AddLabels(labels);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while adding Spelunker bonus ladder down chance.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)...

			object resumeExecution2;
			int i = 0;
			repeat1:
			try
			{
				_helper											// find index of geologist check
					.FindProfessionCheck(Farmer.geologist, fromCurrentIndex: i != 0)
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the branch to resume execution
					)
					.GetOperand(out resumeExecution2)			// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check and resume execution
						new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution2)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Geologist paired gem chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				_helper.Backup();
				goto repeat1;
			}

			_helper.Backup();

			/// Removed: !who.professions.Contains(<excavator_id>) ? ...

			i = 0;
			repeat2:
			try
			{
				_helper											// find index of excavator check
					.FindProfessionCheck(Farmer.excavator, fromCurrentIndex: i != 0)
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Ldc_I4_1)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing Excavator double geode chance.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 2)
			{
				_helper.Backup();
				goto repeat2;
			}

			_helper.Backup();

			/// Removed: who.professions.Contains(<prospector_id>)...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.burrower)		// find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Ldc_I4_1)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		private static double _GetBonusLadderDownChance()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * ModEntry.Data.LowestLevelReached)) - 0.5;
		}
	}
}
