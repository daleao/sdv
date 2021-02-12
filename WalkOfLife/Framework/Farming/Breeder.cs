using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Farming
{
	internal class Breeder
	{
		// Combine shepherd and coopmaster farm animal friendship boost
		[HarmonyPatch(typeof(FarmAnimal), "pet")]
		internal class During_FarmAnimal_Pet
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < instructions.Count(); ++i)
				{
					if (l[i].opcode == OpCodes.Ldc_I4_3 && l[i - 1].operand.ToString().Contains("professions"))
					{
						// inject logic: if (who.professions.Contains(3) then branch to increase animal friendship, else skip over isCoopDweller checks
						l[i + 2].opcode = OpCodes.Brtrue_S;
						l[i + 2].operand = l[i + 5].operand;
						l.Insert(i + 3, new CodeInstruction(OpCodes.Br_S, l[i + 10].operand));
						break;
					}
				}
				return l.AsEnumerable();
			}
		}

		// Combine shepherd and coopmaster product quality boost
		[HarmonyPatch(typeof(FarmAnimal), "dayUpdate")]
		internal class During_FarmAnimal_DayUpdate
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < instructions.Count(); ++i)
				{
					if (l[i].opcode == OpCodes.Call && l[i].operand.ToString().Contains("isCoopDweller"))
					{
						// ignore initial isCoopDweller check
						l[i - 1] = new CodeInstruction(OpCodes.Nop);
						l[i] = new CodeInstruction(OpCodes.Nop);
						l[i + 1] = new CodeInstruction(OpCodes.Nop);

						// inject logic: skip over remaining isCoopDweller checks if !who.profession.Contains(3)
						l.Insert(i + 10, new CodeInstruction(OpCodes.Br_S, l[i + 12].operand));
						break;
					}
				}
				return l.AsEnumerable();
			}
		}

		// Double barn animal pregnancy chance
		[HarmonyPatch(typeof(QuestionEvent), "setUp")]
		internal class During_QuestionEvent_SetUp
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < instructions.Count(); ++i)
				{
					if (l[i].opcode == OpCodes.Ldc_R8 && l[i].operand.ToString().Equals("0.0055"))
					{
						l[i].operand = 0.011;
						break;
					}
				}
				return l.AsEnumerable();
			}
		}

		// Halve coop animal incubation time
		[HarmonyPatch(typeof(SObject), "performObjectDropInAction")]
		internal class During_Object_PerformObjectDropInAction
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < instructions.Count(); ++i)
				{
					if (l[i].opcode == OpCodes.Ldc_I4_2 && l[i - 1].operand.ToString().Contains("professions"))
					{
						l[i].opcode = OpCodes.Ldc_I4_3;
					}
				}
				return l.AsEnumerable();
			}
		}

		// Increase animal sell price adjusted by friendship
		[HarmonyPatch(typeof(FarmAnimal), "getSellPrice")]
		internal class After_FarmAnimal_GetSellPrice
		{
			protected static void Postfix(ref FarmAnimal __instance, ref int __result)
			{
				if (Game1.player.professions.Contains(ModEntry.ProfessionIds["breeder"]))
				{
					__result = (int)(0.1 / 390 * __result + 0.5 / 0.3);
				}
			}
		}
	}
}
