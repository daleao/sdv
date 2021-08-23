using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MeleeWeaponDoAnimateSpecialMovePatch()
		{
			Original = typeof(MeleeWeapon).MethodNamed(name: "doAnimateSpecialMove");
			Postfix = new HarmonyMethod(GetType(), nameof(MeleeWeaponDoAnimateSpecialMovePostfix));
			Transpiler = new HarmonyMethod(GetType(), nameof(MeleeWeaponDoAnimateSpecialMoveTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to reduce special move cooldown for Brute and Hunter.</summary>
		[HarmonyPostfix]
		private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
		{
			var who = __instance.getLastFarmerToUse();
			if (!who.IsLocalPlayer || ModEntry.SuperModeIndex < 0) return;

			switch (__instance.type.Value)
			{
				case MeleeWeapon.club when ModEntry.SuperModeIndex == Util.Professions.IndexOf("Brute"):
					MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * Util.Professions.GetCooldownOrChargeTimeReduction());
					break;
				case MeleeWeapon.dagger when ModEntry.SuperModeIndex == Util.Professions.IndexOf("Hunter"):
					MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * Util.Professions.GetCooldownOrChargeTimeReduction());
					break;
			}
		}

		/// <summary>Patch to remove Acrobat cooldown reduction.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

			int i = 0;
			repeat:
			try
			{
				Helper // find index of acrobat check
					.FindProfessionCheck(Farmer.acrobat, fromCurrentIndex: i != 0)
					.Retreat(2)
					.GetLabels(out var labels) // backup branch labels
					.StripLabels() // remove labels from here
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
					)
					.GetOperand(out var isNotAcrobat) // copy destination
					.Return()
					.Insert( // insert unconditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat)
					)
					.Retreat()
					.AddLabels(labels) // restore bakced-up labels to inserted branch
					.Advance(3);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}");
				return null;
			}

			// repeat injection
			if (++i < 3) goto repeat;

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}