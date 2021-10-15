using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationBreakStonePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationBreakStonePatch()
		{
			Original = typeof(GameLocation).MethodNamed("breakStone");
			Transpiler = new(GetType(), nameof(GameLocationBreakStoneTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Skipped: if (who.professions.Contains(<geologist_id>)...

			try
			{
				Helper
					.FindProfessionCheck(Farmer.geologist) // find index of geologist check
					.Retreat()
					.GetLabels(out var labels) // backup branch labels
					.StripLabels() // remove labels from here
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the false case branch
					)
					.GetOperand(out var isNotGeologist) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br, (Label)isNotGeologist)
					)
					.Retreat()
					.AddLabels(labels); // restore backed-up labels to inserted branch
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Geologist paired gems.\nHelper returned {ex}");
				return null;
			}

			/// Skipped: if (who.professions.Contains(<prospector_id>))...

			try
			{
				Helper
					.FindProfessionCheck(Farmer.burrower) // find index of prospector check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
					)
					.GetOperand(out var isNotProspector) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}