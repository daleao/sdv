using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectPerformObjectDropInActionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
				transpiler: new HarmonyMethod(GetType(), nameof(ObjectPerformObjectDropInActionTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectPerformObjectDropInActionPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for geodes cracked by geode crusher.</summary>
		private static IEnumerable<CodeInstruction> ObjectPerformObjectDropInActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(SObject)}::{nameof(SObject.performObjectDropInAction)}.");

			/// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
			///		AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1)

			Label dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(Stats), nameof(Stats.GeodesCracked)).GetSetMethod())
					)
					.Advance()
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Gemologist"],
						dontIncreaseGemologistCounter)
					.Insert(
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

		/// <summary>Patch to increase Gemologist mineral quality from geode crusher and crystalarium.</summary>
		private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, Farmer who)
		{
			if (Utility.SpecificPlayerHasProfession("Gemologist", who) && __instance.heldObject.Value != null
			&& (Utility.IsForagedMineral(__instance.heldObject.Value) || Utility.IsMineralIndex(__instance.heldObject.Value.ParentSheetIndex))
			&& (__instance.name.Equals("Geode Crusher") || __instance.name.Equals("Crystalarium")))
				__instance.heldObject.Value.Quality = Utility.GetGemologistMineralQuality();
		}

		#endregion harmony patches
	}
}