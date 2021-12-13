using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class FarmAnimalPetPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalPetPatch()
		{
			Original = RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet));
		}

		#region harmony patches

		/// <summary>Patch for Rancher to combine Shepherd and Coopmaster friendship bonus.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FarmAnimalPetTranspiler(IEnumerable<CodeInstruction> instructions,
			MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: if ((who.professions.Contains(<shepherd_id>) && !isCoopDweller()) || (who.professions.Contains(<coopmaster_id>) && isCoopDweller()))
			/// To: if (who.professions.Contains(<rancher_id>)

			try
			{
				helper
					.FindProfessionCheck(Farmer.shepherd) // find index of shepherd check
					.Advance()
					.SetOpCode(OpCodes.Ldc_I4_0) // replace with rancher check
					.Advance(2) // the false case branch instruction
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the true case branch instruction
					)
					.GetOperand(out var hasRancher) // copy destination
					.Return()
					.ReplaceWith(
						new(OpCodes.Brtrue_S,
							(Label) hasRancher) // replace false case branch with true case branch
					)
					.Advance()
					.FindProfessionCheck(Farmer.butcher, true) // find coopmaster check
					.Advance(3) // the branch to resume execution
					.GetOperand(out var resumeExecution) // copy destination
					.Return(2)
					.Insert(
						new CodeInstruction(OpCodes.Br_S, (Label) resumeExecution) // insert new false case branch
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while moving combined vanilla Coopmaster + Shepherd friendship bonuses to Rancher.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: if (who.professions.Contains(100 + <rancher_id>) repeat happiness and mood increase...

			try
			{
				helper
					.FindProfessionCheck(
						Utility.Professions.IndexOf("Rancher")) // go back and find the inserted rancher check
					.Retreat() // reatreat until Ldarg_1 Farmer who
					.ToBufferUntil( // copy to buffer the entire sections which increases happiness and mood
						new CodeInstruction(OpCodes.Callvirt,
							typeof(NetFieldBase<byte, NetByte>).MethodNamed("set_Value"))
					)
					.InsertBuffer() // paste it in-place
					.FindProfessionCheck(
						Utility.Professions.IndexOf("Rancher")) // advance until the second rancher check
					.Advance() // advance to Ldc_I4_S
					.SetOperand(100 + Utility.Professions.IndexOf("Rancher")); // replace rancher with prestiged rancher
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while adding prestiged Rancher friendship bonuses.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}