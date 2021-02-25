using Harmony;
using StardewModdingAPI;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Classes.Harmony;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class QuestionEventSetUpPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal QuestionEventSetUpPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
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

		/// <summary>Patch for Breeder to increase barn animal pregnancy chance.</summary>
		protected static IEnumerable<CodeInstruction> QuestionEventSetUpTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(QuestionEvent)}::{nameof(QuestionEvent.setUp)}.");

			/// From: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * 0.0055
			/// To: if (Game1.random.NextDouble() < (double)(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count * (Game1.player.professions.Contains(<breeder_id>) ? 0.011 : 0.0055)

			Label isNotBreeder = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_helper
					.Find(															// find the index of loading base pregnancy chance
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.0055)
					)
					.AddLabel(isNotBreeder)											// the destination if player is not breeder
					.Advance()
					.AddLabel(resumeExecution)										// the destination to resume execution
					.Retreat()
					.InsertProfessionCheck(ProfessionsMap.Forward["breeder"], branchDestination: isNotBreeder)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_R8, operand: 0.011),		// load double base pregancy chance
						new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)	// branch to resume execution
					);
			}
			catch (Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Breeder animal pregnancy chance.\nHelper returned {ex}");
			}

			return _helper.Log("Successful").Flush();
		}
	}
}
