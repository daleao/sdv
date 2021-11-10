using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class BushMachineGetOutputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BushMachineGetOutputPatch()
		{
			try
			{
				Original = "BushMachine".ToType().MethodNamed("GetOutput");
			}
			catch
			{
				// ignored
			}

			Transpiler = new(AccessTools.Method(GetType(), nameof(BushMachineGetOutputTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch for automated Berry Bush quality.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> BushMachineGetOutputTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
			/// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologist : 0);

			try
			{
				helper
					.FindProfessionCheck(Utility.Professions.IndexOf("Ecologist")) // find index of ecologist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
					)
					.GetLabels(out var labels) // backup branch labels
					.ReplaceWith( // replace with custom quality
						new(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(
								nameof(Utility.Professions.GetEcologistForageQuality)))
					)
					.AddLabels(labels); // restore backed-up labels
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching automated Berry Bush quality.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}