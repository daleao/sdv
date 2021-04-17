using Harmony;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class GeodeMenuUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.update)),
				transpiler: new HarmonyMethod(GetType(), nameof(GeodeMenuUpdateTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for geodes cracked at Clint's.</summary>
		private static IEnumerable<CodeInstruction> GeodeMenuUpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(GeodeMenu)}::{nameof(GeodeMenu.update)}.");

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

		#endregion harmony patches
	}
}