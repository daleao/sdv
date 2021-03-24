using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class SlingshotPerformFirePatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal SlingshotPerformFirePatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire)),
				transpiler: new HarmonyMethod(GetType(), nameof(SlingshotPerformFireTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch to add Desperado quick fire damage bonus.</summary>
		protected static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(Slingshot)}::{nameof(Slingshot.PerformFire)}.");

			/// Injected: if (who.professions.Contains(<desperado_id>) && this.getSlingshotChargeTime() <= 0.5f) damage *= 3

			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
					)
					.GetOperand(out var local5)										// copy reference to local 5 = damage
					.FindNext(														// find index of num = ammunition.Category
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Item), nameof(Item.Category)).GetGetMethod())
					)
					.Retreat()
					.GetLabels(out var labels)
					.StripLabels()
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_2)						// arg 2 = Farmer who
					)
					.InsertProfessionCheckForSpecificPlayer(Utility.ProfessionMap.Forward["desperado"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 0.5f),
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Slingshot), nameof(Slingshot.GetSlingshotChargeTime))),
						new CodeInstruction(OpCodes.Bge_S, resumeExecution),
						new CodeInstruction(OpCodes.Ldloc_S, operand: local5),
						new CodeInstruction(OpCodes.Ldc_I4_3),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stloc_S, operand: local5)
					)
					.AddLabel(resumeExecution)										// branch here if is not desperado or didn't quick fire
					.Return(3)
					.AddLabels(labels);												// restore labels to inserted check
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while adding Desperado quick fire damage.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
