using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class SlingshotPerformFirePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire)),
				transpiler: new HarmonyMethod(GetType(), nameof(SlingshotPerformFireTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to add Desperado quick fire damage bonus.</summary>
		private static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(Slingshot)}::{nameof(Slingshot.PerformFire)}.");

			/// Injected: if (who.professions.Contains(<desperado_id>) && this.getSlingshotChargeTime() <= 0.5f) damage *= 3

			var resumeExecution = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
					)
					.GetOperand(out var local5) // copy reference to local 5 = damage
					.FindNext( // find index of num = ammunition.Category
						new CodeInstruction(OpCodes.Callvirt,
							AccessTools.Property(typeof(Item), nameof(Item.Category)).GetGetMethod())
					)
					.Retreat()
					.GetLabels(out var labels)
					.StripLabels()
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_2) // arg 2 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Utility.ProfessionMap.Forward["Desperado"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 0.5f),
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(Slingshot), nameof(Slingshot.GetSlingshotChargeTime))),
						new CodeInstruction(OpCodes.Bge_S, resumeExecution),
						new CodeInstruction(OpCodes.Ldloc_S, operand: local5),
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stloc_S, operand: local5)
					)
					.AddLabels(resumeExecution) // branch here if is not desperado or didn't quick fire
					.Return(3)
					.AddLabels(labels); // restore labels to inserted check
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Desperado quick fire damage.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}