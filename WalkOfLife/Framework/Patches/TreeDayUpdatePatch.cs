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

			/// From: Game1.random.NextDouble() < <value>
			/// To: Game1.random.NextDouble() < Game1.player.professions.Contains(<arborist_id>) ? <value * 1.25f> : <value>

			Label isNotArborist = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();

			double[] baseValues = new double[] {0.15, 0.6, 0.2};
			int i = 0;
			repeat:
			try
			{
				_helper.Find(				// find index of base tree growth chance
					fromCurrentIndex: i != 0,
					new CodeInstruction(OpCodes.Ldc_R8, operand: baseValues[i])
				)
				.AddLabel(isNotArborist)	// branch here if player is not arborist
				.Advance()
				.AddLabel(resumeExecution)	// branch here to resume execution
				.Retreat()
				.InsertProfessionCheck(Utils.ProfessionsMap.Forward["arborist"], branchDestination: isNotArborist)
				.Insert(					// if player is arborist load adjusted constant
					new CodeInstruction(OpCodes.Ldc_R8, operand: baseValues[i] * 1.25f),
					new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
				);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Arborist tree growth speed.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 3)
			{
				_helper.Backup();
				isNotArborist = iLGenerator.DefineLabel();
				resumeExecution = iLGenerator.DefineLabel();
				goto repeat;
			}

			return _helper.Flush();
		}
	}
}
