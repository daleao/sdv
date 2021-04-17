using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationBreakStonePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), name: "breakStone"),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationBreakStoneTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to remove Miner extra ore + remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
		private static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(GameLocation)}::breakStone.");

			//Helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)...

			try
			{
				Helper
					.FindProfessionCheck(Farmer.geologist) // find index of geologist check
					.Retreat()
					.GetLabels(out var labels) // copy labels
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
					.AddLabels(labels); // restore labels to inserted branch
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Geologist paired gems.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Skipped: if (who.professions.Contains(<prospector_id>))...

			try
			{
				Helper
					.FindProfessionCheck(Farmer.burrower) // find index of prospector check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the false case branch
					)
					.GetOperand(out var isNotProspector) // copy destination
					.Return()
					.Insert( // insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector)
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