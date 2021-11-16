using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationOnStoneDestroyedPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationOnStoneDestroyedPatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.OnStoneDestroyed));
		}

		#region harmony patches

		/// <summary>Patch to remove Prospector double coal chance.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationOnStoneDestroyedTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: random.NextDouble() < 0.035 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2)
			/// To: random.NextDouble() < 0.035

			try
			{
				helper
					.FindProfessionCheck(Farmer.burrower) // find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul) // remove this check
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}