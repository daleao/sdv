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
				original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
				transpiler: new HarmonyMethod(GetType(), nameof(ObjectPerformObjectDropInActionTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectPerformObjectDropInActionPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for geodes cracked by geode crusher.</summary>
		private static IEnumerable<CodeInstruction> ObjectPerformObjectDropInActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(SObject)}::{nameof(SObject.performObjectDropInAction)}.");

			/// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
			///		AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/MineralsCollected", amount: 1)

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
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

		/// <summary>Patch to increase Gemologist mineral quality from geode crusher and crystalarium + speed up Artisan production speed.</summary>
		private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool probe, Farmer who)
		{
			if (probe) return;

			try
			{
				if (__instance.name.AnyOf("Crystalarium", "Geode Crusher") && __instance.heldObject.Value != null
				&& Utility.SpecificPlayerHasProfession("Gemologist", who) && (Utility.IsForagedMineral(__instance.heldObject.Value) || Utility.IsGemOrMineral(__instance.heldObject.Value)))
					__instance.heldObject.Value.Quality = Utility.GetGemologistMineralQuality();
				else if (Utility.IsArtisanMachine(__instance) && (__instance.owner.Value == who.UniqueMultiplayerID || !Game1.IsMultiplayer) && Utility.SpecificPlayerHasProfession("Artisan", who))
					__instance.MinutesUntilReady -= (int)(__instance.MinutesUntilReady * 0.1);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectPerformObjectDropInActionPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}