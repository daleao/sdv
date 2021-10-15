using HarmonyLib;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class CrabPotPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CrabPotPerformObjectDropInActionPatch()
		{
			Original = typeof(CrabPot).MethodNamed(nameof(CrabPot.performObjectDropInAction));
			Transpiler = new(GetType(), nameof(CrabPotPerformObjectDropInActionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to allow Conservationist to place bait.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> CrabPotPerformObjectDropInActionTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Removed: ... && (owner_farmer == null || !owner_farmer.professions.Contains(11)

			try
			{
				Helper
					.FindProfessionCheck(Util.Professions.IndexOf("Conservationist"))
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
				Helper.Error($"Failed while removing Conservationist bait restriction.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}