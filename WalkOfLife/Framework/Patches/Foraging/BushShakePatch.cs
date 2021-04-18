using Harmony;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class BushShakePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Bush), name: "shake"),
				transpiler: new HarmonyMethod(GetType(), nameof(BushShakeTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
		private static IEnumerable<CodeInstruction> BushShakeTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(Bush)}::shake.");

			/// From: Game1.player.professions.Contains(16) ? 4 : 0
			/// To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0

			try
			{
				Helper
					.FindProfessionCheck(Farmer.botanist) // find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)
					)
					.GetLabels(out var labels)
					.ReplaceWith( // replace with custom quality
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Utility), nameof(Utility.GetEcologistForageQuality)))
					)
					.SetLabels(labels);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}").Restore();
			}

			Helper.Backup();

			/// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
			///		AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: @object.Stack)
			
			var dontIncreaseEcologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.InsertProfessionCheckForLocalPlayer(Utility.ProfessionMap.Forward["Ecologist"], dontIncreaseEcologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.Data))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Property(typeof(AwesomeProfessions), nameof(AwesomeProfessions.UniqueID))
								.GetGetMethod()),
						new CodeInstruction(OpCodes.Ldstr, operand: "/ItemsForaged"),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(string), nameof(string.Concat),
								new[] {typeof(string), typeof(string)})),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(ModDataDictionaryExtensions), name: "IncrementField",
								new[] {typeof(ModDataDictionary), typeof(string), typeof(int)})),
						new CodeInstruction(OpCodes.Pop)
					)
					.AddLabels(dontIncreaseEcologistCounter);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Ecologist counter increment.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}