using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class MeleeWeaponDoAnimateSpecialMovePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal MeleeWeaponDoAnimateSpecialMovePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(MeleeWeapon), name: "doAnimateSpecialMove"),
				transpiler: new HarmonyMethod(GetType(), nameof(MeleeWeaponDoAnimateSpecialMoveTranspiler))
			);
		}

		/// <summary>Patch remove Acrobat cooldown reduction.</summary>
		protected static IEnumerable<CodeInstruction> MeleeWeaponDoAnimateSpecialMoveTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(MeleeWeapon)}::doAnimateSpecialMove.");

			/// Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2

			object isNotAcrobat;
			int i = 0;
			repeat:
			try
			{
				_helper											// find index of acrobat check
					.FindProfessionCheck(Farmer.acrobat, fromCurrentIndex: i != 0)
					.Retreat(2)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the branch to resume execution
					)
					.GetOperand(out isNotAcrobat)				// copy destination
					.Return()
					.Insert(									// insert unconditional branch to skip this check and resume execution
						new CodeInstruction(OpCodes.Br_S, (Label)isNotAcrobat)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Acrobat cooldown reduction.\nHelper returned {ex}").Restore();
			}

			// repeat injection
			if (++i < 3)
			{
				_helper.Backup();
				goto repeat;
			}

			return _helper.Flush();
		}
	}
}
