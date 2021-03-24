using Harmony;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class QuestionEventSetUpPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal QuestionEventSetUpPatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
				transpiler: new HarmonyMethod(GetType(), nameof(QuestionEventSetUpTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
		protected static IEnumerable<CodeInstruction> QuestionEventSetUpTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(QuestionEvent)}::{nameof(QuestionEvent.setUp)}.");

			/// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * 0.0055
			/// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

			Label isNotBreeder = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_Helper
					.FindFirst(					// find index of loading base pregnancy chance
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.0055)
					)
					.AddLabel(isNotBreeder)		// branch here if player is not breeder
					.Advance()
					.AddLabel(resumeExecution)	// branch here to resume execution
					.Retreat()
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["breeder"], branchDestination: isNotBreeder)
					.Insert(					// if player is breeder load adjusted pregancy chance
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.011),
						new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
