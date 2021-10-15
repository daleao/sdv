using HarmonyLib;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class QuestionEventSetUpPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal QuestionEventSetUpPatch()
		{
			Original = typeof(QuestionEvent).MethodNamed(nameof(QuestionEvent.setUp));
			Transpiler = new(GetType(), nameof(QuestionEventSetUpTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> QuestionEventSetUpTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (0.0055 * 3)
			/// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

			var isNotBreeder = iLGenerator.DefineLabel();
			var resumeExecution = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindFirst( // find index of loading base pregnancy chance
						new CodeInstruction(OpCodes.Ldc_R8, 0.0055)
					)
					.AddLabels(isNotBreeder) // branch here if player is not breeder
					.Advance()
					.AddLabels(resumeExecution) // branch here to resume execution
					.Retreat()
					.InsertProfessionCheckForLocalPlayer(Util.Professions.IndexOf("Breeder"), isNotBreeder)
					.Insert( // if player is breeder load adjusted pregancy chance
						new CodeInstruction(OpCodes.Ldc_R8, 0.0165),
						new CodeInstruction(OpCodes.Br_S, resumeExecution)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Breeder bonus animal pregnancy chance.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}