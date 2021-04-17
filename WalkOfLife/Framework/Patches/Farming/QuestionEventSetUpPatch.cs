using Harmony;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class QuestionEventSetUpPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
				transpiler: new HarmonyMethod(GetType(), nameof(QuestionEventSetUpTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
		private static IEnumerable<CodeInstruction> QuestionEventSetUpTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(QuestionEvent)}::{nameof(QuestionEvent.setUp)}.");

			/// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * 0.0055
			/// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

			var isNotBreeder = iLGenerator.DefineLabel();
			var resumeExecution = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindFirst( // find index of loading base pregnancy chance
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.0055)
					)
					.AddLabels(isNotBreeder) // branch here if player is not breeder
					.Advance()
					.AddLabels(resumeExecution) // branch here to resume execution
					.Retreat()
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Breeder"],
						branchDestination: isNotBreeder)
					.Insert( // if player is breeder load adjusted pregancy chance
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.011),
						new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}