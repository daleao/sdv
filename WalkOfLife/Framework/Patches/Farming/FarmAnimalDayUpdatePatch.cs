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
	internal class FarmAnimalDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalDayUpdatePatch()
		{
			Original = RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate));
			Transpiler = new(GetType().MethodNamed(nameof(FarmAnimalDayUpdateTranspiler)));
		}

		#region harmony patches

		/// <summary>
		///     Patch for Producer to double produce frequency at max animal happiness + remove Shepherd and Coopmaster hidden
		///     produce quality boosts.
		/// </summary>
		[HarmonyTranspiler]
		protected static IEnumerable<CodeInstruction> FarmAnimalDayUpdateTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
			/// To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200) && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>) ? 2 : 1

			try
			{
				helper
					.FindFirst( // find index of FarmAnimal.type.Value.Equals("Sheep")
						new CodeInstruction(OpCodes.Ldstr, "Sheep"),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(string).MethodNamed(nameof(string.Equals), new[] {typeof(string)}))
					)
					.Retreat(2)
					.SetOperand(typeof(FarmAnimal).Field(nameof(FarmAnimal.happiness))) // was FarmAnimal.type
					.Advance()
					.SetOperand(typeof(NetFieldBase<byte, NetByte>)
						.PropertyGetter(nameof(NetFieldBase<byte, NetByte>.Value))) // was <string, NetString>
					.Advance()
					.ReplaceWith(
						new(OpCodes.Ldc_I4_S, 200) // was Ldstr "Sheep"
					)
					.Advance()
					.Remove()
					.SetOpCode(OpCodes.Blt_S) // was Brfalse
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_0)
					)
					.SetOpCode(OpCodes.Ldc_I4_1) // was Ldc_I4_0
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_1)
					)
					.SetOpCode(OpCodes.Ldc_I4_2) // was Ldc_I4_1
					.Advance()
					.SetOpCode(OpCodes.Div) // was Sub
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Conv_U1)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching modded Producer produce frequency.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Skipped: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>))) chanceForQuality += 0.33

			try
			{
				helper
					.FindNext( // find index of first FarmAnimal.isCoopDweller check
						new CodeInstruction(OpCodes.Call,
							typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.isCoopDweller)))
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S) // the all cases false branch
					)
					.GetOperand(out var resumeExecution) // copy destination
					.Return()
					.Retreat()
					.Insert( // insert unconditional branch to skip this whole section
						new CodeInstruction(OpCodes.Br_S, (Label) resumeExecution)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while removing vanilla Coopmaster + Shepherd produce quality bonuses.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}