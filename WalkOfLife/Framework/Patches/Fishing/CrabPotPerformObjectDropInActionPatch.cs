using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Objects;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class CrabPotPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CrabPotPerformObjectDropInActionPatch()
		{
			Original = RequireMethod<CrabPot>(nameof(CrabPot.performObjectDropInAction));
			Transpiler = new(AccessTools.Method(GetType(), nameof(CrabPotPerformObjectDropInActionTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to allow Conservationist to place bait.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> CrabPotPerformObjectDropInActionTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Removed: ... && (owner_farmer is null || !owner_farmer.professions.Contains(11)

			try
			{
				helper
					.FindProfessionCheck(Utility.Professions.IndexOf("Conservationist"))
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Brtrue_S)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing Conservationist bait restriction.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}