using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FarmerTakeDamagePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerTakeDamagePatch()
		{
			Original = typeof(Farmer).MethodNamed(nameof(Farmer.takeDamage));
			Transpiler = new(GetType(), nameof(FarmerTakeDamageTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to make Poacher untargetable during super mode + increment Brute Fury for damage taken + add Brute super mode immortality.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FarmerTakeDamageTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: else if (this.IsLocalPlayer && IsSuperModeActive && SuperModeIndex == <poacher_id>) monsterDamageCapable = false;

			var alreadyUndamageableOrNotAmbuscade = iLGenerator.DefineLabel();
			try
			{
				Helper
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
						// check if IsSuperModeActive
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brfalse_S, alreadyUndamageableOrNotAmbuscade),
						// check if SuperModeIndex == <poacher_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Util.Professions.IndexOf("Poacher")),
						new CodeInstruction(OpCodes.Bne_Un_S, alreadyUndamageableOrNotAmbuscade),
						// set monsterDamageCapable = false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_0)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Poacher untargetability during super mode.\nHelper returned {ex}");
				return null;
			}

			/// Injected: if (IsSuperModeActive && SuperModeIndex == <brute_id>) health = 1;
			/// After: if (health <= 0)
			/// Before: GetEffectsOfRingMultiplier(863)

			var isNotUndyingButMayHaveDailyRevive = iLGenerator.DefineLabel();
			try
			{
				Helper
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
						// check if IsSuperModeActive
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brfalse_S, isNotUndyingButMayHaveDailyRevive),
						// check if SuperModeIndex == <brute_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Util.Professions.IndexOf("Brute")),
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
				Helper.Error($"Failed while adding Brute super mode immortality.\nHelper returned {ex}");
				return null;
			}

			/// Injected: if (SuperModeIndex == <brute_id> && damage > 0) SuperModeCountry += 2;
			/// At: end of method (before return)

			var dontIncreaseBruteCounterForDamage = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindLast( // find index of final return
						new CodeInstruction(OpCodes.Ret)
					)
					.AddLabels(dontIncreaseBruteCounterForDamage) // branch here to skip counter increment
					.Insert(
						// check if SuperModeIndex == <brute_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Util.Professions.IndexOf("Brute")),
						new CodeInstruction(OpCodes.Bne_Un_S, dontIncreaseBruteCounterForDamage),
						// check if farmer received any damage
						new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Ble_S, dontIncreaseBruteCounterForDamage),
						// if so, increment counter by 2
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeCounter))),
						new CodeInstruction(OpCodes.Ldc_I4_2), // <-- increment amount
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertySetter(nameof(ModEntry.SuperModeCounter)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Brute Fury counter for damage taken.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}