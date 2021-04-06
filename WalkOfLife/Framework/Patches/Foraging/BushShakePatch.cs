using Harmony;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class BushShakePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Bush), name: "shake"),
				transpiler: new HarmonyMethod(GetType(), nameof(BushShakeTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to nerf Ecologist berry quality.</summary>
		private static IEnumerable<CodeInstruction> BushShakeTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(Bush)}::shake.");

			/// From: Game1.player.professions.Contains(16) ? 4 : 0
			/// To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)
					)
					.GetLabels(out var labels)
					.ReplaceWith( // replace with custom quality
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					)
					.SetLabels(labels);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}