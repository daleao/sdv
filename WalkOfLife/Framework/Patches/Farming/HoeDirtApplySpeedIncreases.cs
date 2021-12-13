using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches.Farming
{
	[UsedImplicitly]
	internal class HoeDirtApplySpeedIncreases : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal HoeDirtApplySpeedIncreases()
		{
			Original = RequireMethod<HoeDirt>("applySpeedIncreases");
		}

		#region harmony patches

		[HarmonyTranspiler]
		protected static IEnumerable<CodeInstruction> HoeDirtApplySpeedIncreasesTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: if (who.professions.Contains(100 + <agriculturist_id>)) speedIncrease += 0.1f;

			try
			{
				helper
					.FindProfessionCheck(Utility.Professions.IndexOf("Agriculturist"))
					.Advance()
					.FindProfessionCheck(Utility.Professions.IndexOf("Agriculturist"))
					.Retreat()
					.ToBufferUntil(
						true,
						true,
						new CodeInstruction(OpCodes.Stloc_2)
					)
					.InsertBuffer()
					.Return()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_5)
					)
					.ReplaceWith(
						new(OpCodes.Ldc_I4_S, 100 + Utility.Professions.IndexOf("Agriculturist"))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching prestiged Agriculturist bonus.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}