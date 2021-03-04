using Harmony;
using StardewModdingAPI;
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

		/// Patch for Spelunker ladder down chance bonus
		protected static IEnumerable<CodeInstruction> MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(MineShaft)}::{nameof(MineShaft.checkStoneForItems)}.");

			/// Injected: if (Game1.player.professions.Contains(<spelunker_id>) chanceForLadderDown += GetBonusLadderDownChance()

			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_helper
					.Find(														// find the segment which spawns the ladder
						new CodeInstruction(OpCodes.Ldfld, operand: AccessTools.Field(typeof(MineShaft), name: "ladderHasSpawned"))
					)
					.Retreat()
					.GetLabel(out Label label)
					.StripLabels()
					.AddLabel(resumeExecution)
					.Insert(													// add bonus
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4)	// arg 4 = Farmer who
					)
					.InsertProfessionCheckForWho(Utils.ProfessionsMap.Forward["spelunker"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_3),					// local 3 = chanceForLadderDown
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(MineShaftCheckStoneForItemsPatch), nameof(MineShaftCheckStoneForItemsPatch.GetBonusLadderDownChance))),
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Stloc_3)
					)
					.Return(3)
					.AddLabel(label);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Spelunker ladder down chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		protected static double GetBonusLadderDownChance()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * ModEntry.Data.LowestLevelReached)) - 0.5;
		}
	}
}
