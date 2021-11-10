using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MeleeWeaponDoAnimateSpecialMovePatch()
		{
			Original = RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
			Postfix = new(GetType().MethodNamed(nameof(MeleeWeaponDoAnimateSpecialMovePostfix)));
			Transpiler = new(GetType().MethodNamed(nameof(MeleeWeaponDoAnimateSpecialMoveTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to reduce special move cooldown for Brute and Poacher.</summary>
		[HarmonyPostfix]
		private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
		{
			var who = __instance.getLastFarmerToUse();
			if (!who.IsLocalPlayer || ModEntry.SuperModeIndex < 0) return;

			switch (__instance.type.Value)
			{
				case MeleeWeapon.club when ModEntry.SuperModeIndex == Utility.Professions.IndexOf("Brute"):
					MeleeWeapon.clubCooldown =
						(int) (MeleeWeapon.clubCooldown * Utility.Professions.GetCooldownOrChargeTimeReduction());
					break;

				case MeleeWeapon.dagger when ModEntry.SuperModeIndex == Utility.Professions.IndexOf("Poacher"):
					MeleeWeapon.daggerCooldown = (int) (MeleeWeapon.daggerCooldown *
					                                    Utility.Professions.GetCooldownOrChargeTimeReduction());
					break;
			}
		}

		/// <summary>Patch to remove Acrobat cooldown reduction.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

			var i = 0;
			repeat:
			try
			{
				helper // find index of acrobat check
					.FindProfessionCheck(Farmer.acrobat, i != 0)
					.Retreat(2)
					.StripLabels(out var labels) // backup and remove branch labels
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the false case branch
					)
					.GetOperand(out var isNotAcrobat) // copy destination
					.Return()
					.Insert( // insert unconditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label) isNotAcrobat)
					)
					.Retreat()
					.AddLabels(labels) // restore bakced-up labels to inserted branch
					.Advance(3);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			// repeat injection three times
			if (++i < 3) goto repeat;

			return helper.Flush();
		}

		#endregion harmony patches
	}
}