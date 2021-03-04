using Harmony;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TreeDayUpdatePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TreeDayUpdatePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
				transpiler: new HarmonyMethod(GetType(), nameof(TreeDayUpdateTranspiler))
			);
		}

		/// <summary>Patch to increase Abrorist tree growth speed.</summary>
		protected static IEnumerable<CodeInstruction> TreeDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Tree)}::{nameof(Tree.dayUpdate)}.");

			/// From: Game1.random.NextDouble() < 0.15
			/// To: Game1.random.NextDouble() < Game1.player.professions.Contains(<arborist_id>) ? 0.1875 : 0.15

			Label isNotArborist = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();

			try
			{
				_helper.Find(				// find index of loading tree growth chance
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.15)
				)
				.AddLabel(isNotArborist)	// branch here if player is not arborist
				.Advance()
				.AddLabel(resumeExecution)	// branch here to resume execution
				.Retreat()
				.InsertProfessionCheck(Utils.ProfessionsMap.Forward["arborist"], branchDestination: isNotArborist)
				.Insert(					// if player is arborist load adjusted constant
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.1875),
					new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
				);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Arborist tree growth speed.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: fertilized.Value && Game1.random.NextDouble() < 0.6)
			/// To: fertilized.Value && Game1.random.NextDouble() < Game1.player.professions.Contains(< arborist_id >) ? 0.9 : 0.6 

			isNotArborist = iLGenerator.DefineLabel();
			resumeExecution = iLGenerator.DefineLabel();

			try
			{
				_helper.AdvanceUntil(		// find index of loading tree growth chance
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.6)
				)
				.AddLabel(isNotArborist)	// branch here if player is not arborist
				.Advance()
				.AddLabel(resumeExecution)	// branch here to resume execution
				.Retreat()
				.InsertProfessionCheck(Utils.ProfessionsMap.Forward["arborist"], branchDestination: isNotArborist)
				.Insert(					// if player is arborist load adjusted constant
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.9),
					new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
				);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Arborist tree growth speed.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: Game1.random.NextDouble() < 0.2
			/// To: Game1.random.NextDouble() < Game1.player.professions.Contains(<arborist_id>) ? 0.25 : 0.2

			isNotArborist = iLGenerator.DefineLabel();
			resumeExecution = iLGenerator.DefineLabel();

			try
			{
				_helper.AdvanceUntil(		// find index of loading tree growth chance
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.2)
				)
				.AddLabel(isNotArborist)	// branch here if player is not arborist
				.Advance()
				.AddLabel(resumeExecution)	// branch here to resume execution
				.Retreat()
				.InsertProfessionCheck(Utils.ProfessionsMap.Forward["arborist"], branchDestination: isNotArborist)
				.Insert(					// if player is arborist load adjusted constant
					new CodeInstruction(OpCodes.Ldc_R8, operand: 0.25),
					new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
				);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Arborist tree growth speed.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
