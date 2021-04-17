using Harmony;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions.Framework.Patches.Combat
{
	internal class GreenSlimeOnDealContactDamagePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenSlime), nameof(GreenSlime.onDealContactDamage)),
				transpiler: new HarmonyMethod(GetType(), nameof(GreenSlimeOnDealContactDamageTranspiler))
			);
		}

		/// <summary>Patch to make Slimecharmer immune to slimed debuff.</summary>
		private static IEnumerable<CodeInstruction> GreenSlimeOnDealContactDamageTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(GreenSlime)}::{nameof(GreenSlime.onDealContactDamage)}.");

			/// Injected: if (who.professions.Contains(<slimecharmer_id>)) return

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Bge_Un) // find index of first branch instruction
					)
					.GetOperand(out var returnLabel) // get return label
					.Return()
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Utility.ProfessionMap.Forward["Slimecharmer"], (Label)returnLabel, useBrtrue: true);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Slimecharmer slime debuff immunity.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}
	}
}