using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GreenSlimeOnDealContactDamagePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GreenSlimeOnDealContactDamagePatch()
		{
			Original = typeof(GreenSlime).MethodNamed(nameof(GreenSlime.onDealContactDamage));
			Transpiler = new HarmonyMethod(GetType(), nameof(GreenSlimeOnDealContactDamageTranspiler));
		}

		/// <summary>Patch to make Piper immune to slimed debuff.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GreenSlimeOnDealContactDamageTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: if (who.professions.Contains(<piper_id>)) return

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(ModEntry.GameFramework.Equals(GameFramework.Xna)
							? OpCodes.Bge_Un_S
							: OpCodes.Bge_Un) // find index of first branch instruction
					)
					.GetOperand(out var returnLabel) // get return label
					.Return()
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Util.Professions.IndexOf("Piper"), (Label)returnLabel,
						true);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Piper slime debuff immunity.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}
	}
}