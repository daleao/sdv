using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectCheckForActionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
				transpiler: new HarmonyMethod(GetType(), nameof(ObjectCheckForActionTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectCheckForActionPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for gems collected from crystalarium.</summary>
		private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(SObject)}::{nameof(SObject.checkForAction)}.");

			/// Injected: if (who.professions.Contains(<gemologist_id>) && name.Equals("Crystalarium"))
			///		AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1)

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindLast(
						new CodeInstruction(OpCodes.Ldstr, "coin")
					)
					.Advance(2)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Utility.ProfessionMap.Forward["Gemologist"],
						dontIncreaseGemologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(SObject), nameof(SObject.name)).GetGetMethod()),
						new CodeInstruction(OpCodes.Ldstr, "Crystalarium"),
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Method(typeof(string), nameof(string.Equals), new[] { typeof(string) })),
						new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.Data))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.UniqueID))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Ldstr, operand: "/MineralsCollected"),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(string), nameof(string.Concat),
								new[] { typeof(string), typeof(string) })),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(ModDataDictionaryExtensions), name: "IncrementField",
								new[] { typeof(ModDataDictionary), typeof(string), typeof(int) })),
						new CodeInstruction(OpCodes.Pop)
					)
					.AddLabels(dontIncreaseGemologistCounter);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Gemologist counter increment.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		/// <summary>Patch to increase Gemologist mineral quality from crystalarium.</summary>
		private static void ObjectCheckForActionPostfix(SObject __instance, Farmer who)
		{
			try
			{
				if (__instance.heldObject.Value == null || !Utility.SpecificPlayerHasProfession("Gemologist", who) || !(__instance.owner.Value == who.UniqueMultiplayerID || !Game1.IsMultiplayer))
					return;
				
				if (__instance.name.Equals("Crystalarium")) __instance.heldObject.Value.Quality = Utility.GetGemologistMineralQuality();
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectCheckForActionPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}