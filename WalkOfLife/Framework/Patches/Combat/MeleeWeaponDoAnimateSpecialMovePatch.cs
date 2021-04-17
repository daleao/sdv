using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(MeleeWeapon), name: "doAnimateSpecialMove"),
				transpiler: new HarmonyMethod(GetType(), nameof(MeleeWeaponDoAnimateSpecialMoveTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch remove Acrobat cooldown reduction.</summary>
		private static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(MeleeWeapon)}::doAnimateSpecialMove.");

			/// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

			var i = 0;
			repeat:
			try
			{
				Helper // find index of acrobat check
					.FindProfessionCheck(Farmer.acrobat, fromCurrentIndex: i != 0)
					.Retreat(2)
					.GetLabels(out var labels)
					.StripLabels()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse) // the false case branch
					)
					.GetOperand(out var isNotAcrobat) // copy destination
					.Return()
					.Insert( // insert unconditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat)
					)
					.Retreat()
					.AddLabels(labels) // restore labels to inserted branch
					.Advance(3);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 3)
			{
				Helper.Backup();
				goto repeat;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}