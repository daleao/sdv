using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class FarmerTakeDamagePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerTakeDamagePatch()
		{
			Original = RequireMethod<Farmer>(nameof(Farmer.takeDamage));
		}

		#region harmony patches

		/// <summary>
		///     Patch to make Poacher untargetable during Super Mode + increment Brute Fury for damage taken + add Brute super
		///     mode immortality.
		/// </summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FarmerTakeDamageTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: else if (this.IsLocalPlayer && IsModStateActive && ModStateIndex == <poacher_id>) monsterDamageCapable = false;

			var alreadyUndamageableOrNotAmbuscade = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Stloc_0)
					)
					.Advance()
					.AddLabels(alreadyUndamageableOrNotAmbuscade)
					.Insert(
						// check if monsterDamageCapable is already false
						new CodeInstruction(OpCodes.Ldloc_0),
						new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
						// check if this.IsLocalPlayer
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call,
							typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
						new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
						// check if IsModStateActive
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
						// check if ModStateIndex == <poacher_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Utility.Professions.IndexOf("Poacher")),
						new CodeInstruction(OpCodes.Bne_Un_S, alreadyUndamageableOrNotAmbuscade),
						// set monsterDamageCapable = false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_0)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Poacher untargetability during Super Mode.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: if (IsModStateActive && ModStateIndex == <brute_id>) health = 1;
			/// After: if (health <= 0)
			/// Before: GetEffectsOfRingMultiplier(863)

			var isNotUndyingButMayHaveDailyRevive = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindNext( // find index of health <= 0 (start of revive ring effect)
						new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
						new CodeInstruction(OpCodes.Ldfld,
							typeof(Farmer).Field(nameof(Farmer.health))),
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Bgt)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Bgt)
					)
					.GetOperand(out var resumeExecution) // copy branch label to resume normal execution
					.Advance()
					.AddLabels(isNotUndyingButMayHaveDailyRevive)
					.Insert(
						// check if IsModStateActive
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
						// check if ModStateIndex == <brute_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Utility.Professions.IndexOf("Brute")),
						new CodeInstruction(OpCodes.Bne_Un_S, isNotUndyingButMayHaveDailyRevive),
						// set health back to 1
						new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Farmer this
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Stfld,
							typeof(Farmer).Field(nameof(Farmer.health))),
						// resume execution (skip revive ring effect)
						new CodeInstruction(OpCodes.Br, resumeExecution)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Brute Super Mode immortality.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: if (ModStateIndex == <brute_id> && damage > 0) ModStateCountry += 2;
			/// At: end of method (before return)

			var dontIncreaseBruteCounterForDamage = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindLast( // find index of final return
						new CodeInstruction(OpCodes.Ret)
					)
					.AddLabels(dontIncreaseBruteCounterForDamage) // branch here to skip gauge increment
					.Insert(
						// check if ModStateIndex == <brute_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Utility.Professions.IndexOf("Brute")),
						new CodeInstruction(OpCodes.Bne_Un_S, dontIncreaseBruteCounterForDamage),
						// check if farmer received any damage
						new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Ble_S, dontIncreaseBruteCounterForDamage),
						// if so, increment gauge by 2
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.SuperModeGaugeValue))),
						new CodeInstruction(OpCodes.Ldc_R8, 2.0), // <-- increment amount
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertyGetter(nameof(ModState.SuperModeGaugeMaxValue))),
						new CodeInstruction(OpCodes.Conv_R8),
						new CodeInstruction(OpCodes.Ldc_R8, 500.0),
						new CodeInstruction(OpCodes.Div),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Conv_I4),
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Call,
							typeof(ModState).PropertySetter(nameof(ModState.SuperModeGaugeValue)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Brute Fury gauge for damage taken.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}