using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Locations;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Fishing
{
	[UsedImplicitly]
	internal class MountainGetFishPatch : BasePatch
	{
		private const int LEGEND_INDEX_I = 163;

		/// <summary>Construct an instance.</summary>
		internal MountainGetFishPatch()
		{
			Original = RequireMethod<Mountain>(nameof(Mountain.getFish));
		}

		#region harmony patches

		/// <summary>Patch for prestiged Angler to recatch Legend.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> MountainGetFishTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
			/// To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...

			var checkSeason = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4, LEGEND_INDEX_I)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brtrue_S)
					)
					.GetOperand(out var skipLegendary)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Brfalse_S, checkSeason))
					.Advance()
					.AddLabels(checkSeason)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, 4), // arg 4 = Farmer who
						new CodeInstruction(OpCodes.Ldstr, "Angler"),
						new CodeInstruction(OpCodes.Call,
							typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.HasPrestigedProfession))),
						new CodeInstruction(OpCodes.Brfalse_S, skipLegendary)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding prestiged Angler legendary fish recatch.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}
