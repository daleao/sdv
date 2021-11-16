using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationDamageMonsterPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationDamageMonsterPatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster),
				new[]
				{
					typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
					typeof(float), typeof(float), typeof(bool), typeof(Farmer)
				});
		}

		#region harmony patches

		/// <summary>
		///     Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage
		///     bonus from Desperado to Poacher + increment Brute Fury and Poacher Cold Blood gauges + perform Poacher steal.
		/// </summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f
			/// To: if (who.professions.Contains(<poacher_id>) critChance += 0.1f

			try
			{
				helper
					.FindProfessionCheck(Farmer.scout) // find index of scout check
					.Advance()
					.SetOperand(Utility.Professions.IndexOf("Poacher")) // replace with Poacher check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_S) // start of critChance += critChance * 0.5f
					)
					.Advance()
					.Remove() // was Ldarg_S critChance
					.ReplaceWith( // was Ldc_R4 0.5
						new(OpCodes.Ldc_R4, 0.1f)
					)
					.Advance()
					.Remove(); // was Mul
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// From: if (who is not null && who.professions.Contains(<brute_id>) ... *= 1.15f
			/// To: if (who is not null && who.professions.Contains(<brute_id>) ... *= GetBruteBonusDamageMultiplier(who)

			try
			{
				helper
					.FindProfessionCheck(Utility.Professions.IndexOf("Brute"),
						true) // find index of brute check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
					)
					.ReplaceWith( // replace with custom multiplier
						new(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions
								.GetBruteBonusDamageMultiplier)))
					)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // arg 10 = Farmer who
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			/// From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f
			/// To: if (who is not null && crit && who.IsLocalPlayer && ModState.Index == <poacher_id>) ... *= GetPoacherCritDamageMultiplier

			try
			{
				helper
					.FindProfessionCheck(Farmer.desperado, true) // find index of desperado check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse_S)
					)
					.GetOperand(out var dontIncreaseCritPow)
					.Return()
					.ReplaceWith(
						new(OpCodes.Callvirt,
							typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))) // was Ldfld Farmer.professions
					)
					.Advance()
					.ReplaceWith(
						new(OpCodes.Brfalse_S, dontIncreaseCritPow) // was Ldc_I4_S <desperado id>
					)
					.Advance()
					.ReplaceWith(
						new(OpCodes.Call,
							typeof(ModState).PropertyGetter(
								nameof(ModState.SuperModeIndex))) // was Callvirt NetList.Contains
					)
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Ldc_I4_S, Utility.Professions.IndexOf("Poacher"))
					)
					.SetOpCode(OpCodes.Bne_Un_S) // was Brfalse_S
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 2f) // desperado critical damage multiplier
					)
					.ReplaceWith(
						new(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(
								nameof(Utility.Professions.GetPoacherCritDamageMultiplier)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while moving modded bonus crit damage from Desperado to Poacher.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// Injected: GameLocationSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who)
			///	Before: if (monster.Health <= 0)

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(bool)} (7)")
					)
					.GetOperand(out var didCrit) // copy reference to local 7 = Crit (whether player performed a crit)
					.FindFirst(
						new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (8)")
					)
					.GetOperand(out var damageAmount)
					.FindFirst( // monter.Health <= 0
						new CodeInstruction(OpCodes.Ldloc_2),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Monster).PropertyGetter(nameof(Monster.Health))),
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Bgt)
					)
					.StripLabels(out var labels) // backup and remove branch labels
					.Insert(
						// restore backed-up labels
						labels,
						// prepare arguments
						new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 4), // arg 4 = bool isBomb
						new CodeInstruction(OpCodes.Ldloc_S, didCrit),
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 8), // arg 8 = float critMultiplier
						new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 10), // arg 10 = Farmer who
						new CodeInstruction(OpCodes.Call,
							typeof(GameLocationDamageMonsterPatch).MethodNamed(nameof(DamageMonsterSubroutine)))
					)
					.Return()
					.AddLabels(labels);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void DamageMonsterSubroutine(int damageAmount, bool isBomb, bool didCrit, float critMultiplier,
			Monster monster, Farmer who)
		{
			if (damageAmount <= 0 || isBomb ||
			    who is not {IsLocalPlayer: true, CurrentTool: MeleeWeapon weapon}) return;

			// try to steal
			if (didCrit && ModState.SuperModeIndex == Utility.Professions.IndexOf("Poacher") &&
			    !ModState.MonstersStolenFrom.Contains(monster.GetHashCode()) && Game1.random.NextDouble() <
			    (weapon.type.Value == MeleeWeapon.dagger ? 0.6 : 0.3))
			{
				var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
					.Concat(monster.getExtraDropItems()).ToList();
				var stolen = drops.ElementAtOrDefault(Game1.random.Next(drops.Count))?.getOne();
				if (stolen is not null && who.addItemToInventoryBool(stolen))
				{
					ModState.MonstersStolenFrom.Add(monster.GetHashCode());

					// play sound effect
					try
					{
						if (ModEntry.SoundFX.SoundByName.TryGetValue("poacher_steal", out var sfx))
							sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
						else throw new ContentLoadException();
					}
					catch (Exception ex)
					{
						ModEntry.Log(
							$"Couldn't play file 'assets/sfx/poacher_steal.wav'. Make sure the file exists. {ex}",
							LogLevel.Error);
					}
				}
			}

			// try to increment Super Mode gauges
			if (ModState.IsSuperModeActive) return;

			var increment = 0;
			if (ModState.SuperModeIndex == Utility.Professions.IndexOf("Brute"))
			{
				increment = 2;
				if (monster.Health <= 0) increment *= 2;
				if (weapon.type.Value == MeleeWeapon.club) increment *= 2;
			}
			else if (ModState.SuperModeIndex == Utility.Professions.IndexOf("Poacher") && didCrit)
			{
				increment = (int) critMultiplier;
				if (weapon.type.Value == MeleeWeapon.dagger) increment *= 2;
			}

			ModState.SuperModeGaugeValue += (int) (increment * ((float) ModState.SuperModeGaugeMaxValue / 500));
		}

		#endregion private methods
	}
}