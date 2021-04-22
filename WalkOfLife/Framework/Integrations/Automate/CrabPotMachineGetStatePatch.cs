using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class CrabPotMachineGetStatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method("Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine:GetState"),
				transpiler: new HarmonyMethod(GetType(), nameof(CrabPotMachineGetStateTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
		private static IEnumerable<CodeInstruction> CrabPotMachineGetStateTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {AccessTools.Method("Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine:GetState")}.");

			/// Removed: || !this.PlayerNeedsBait()

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Brtrue)
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(
								"Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine:PlayerNeedsBait"))
					)
					.SetOpCode(OpCodes.Brfalse_S);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching bait conditions for Automate crab pots.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}