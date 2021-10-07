using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationDamageMonsterPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationDamageMonsterPatch()
		{
			Original = typeof(GameLocation).MethodNamed(nameof(GameLocation.damageMonster),
				new[]
				{
					typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
					typeof(float), typeof(float), typeof(bool), typeof(Farmer)
				});
			Transpiler = new HarmonyMethod(GetType(), nameof(GameLocationDamageMonsterTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage bonus from Desperado to Poacher + increment Brute Fury and Poacher Cold Blood counters + perform Poacher steal.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f
			/// To: if (who.professions.Contains(<poacher_id>) critChance += GetPoacherBonusCritChance()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.scout) // find index of scout check
					.Advance()
					.SetOperand(Util.Professions.IndexOf("Poacher")) // replace with Poacher check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_S) // start of critChance += critChance * 0.5f
					)
					.Advance()
					.Remove() // was Ldarg_S critChance
					.ReplaceWith( // was Ldc_R4 0.5
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetPoacherBonusCritChance)))
					)
					.Advance()
					.Remove(); // was Mul
			}
			catch (Exception ex)
			{
				Helper.Error(
					$"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
				return null;
			}

			/// From: if (who != null && who.professions.Contains(<brute_id>) ... *= 1.15f
			/// To: if (who != null && who.professions.Contains(<brute_id>) ... *= GetBruteBonusDamageMultiplier(who)

			try
			{
				Helper
					.FindProfessionCheck(Util.Professions.IndexOf("Brute"),
						true) // find index of brute check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
					)
					.ReplaceWith( // replace with custom multiplier
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions
								.GetBruteBonusDamageMultiplier)))
					)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // arg 10 = Farmer who
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");
				return null;
			}

			/// From: if (who != null && who.professions.Contains(<desperado_id>) ... *= 2f
			/// To: if (who != null && who.professions.Contains(<poacher_id>) ... *= GetPoacherCritDamageMultiplier

			try
			{
				Helper
					.FindProfessionCheck(Farmer.desperado, true) // find index of desperado check
					.Advance()
					.SetOperand(Util.Professions.IndexOf("Poacher")) // change to Poacher check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 2f) // desperado critical damage multiplier
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldarg_S, (byte) 10) // was Ldc_R4 2f (arg 10 = Farmer who)
					)
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(
								nameof(Util.Professions.GetPoacherCritDamageMultiplier)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error(
					$"Failed while moving modded bonus crit damage from Desperado to Poacher.\nHelper returned {ex}");
				return null;
			}

			/// Injected: tryToStealAndIncrementCountersOrEndPoacherSuperMode(damageAmount, isBomb, crit, critMultiplier, monster, who)
			///	Before: if (monster.Health <= 0)

			try
			{
				Helper
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
					.GetLabels(out var labels)
					.StripLabels()
					.Insert(
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
				Helper.Error(
					$"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood counters.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void DamageMonsterSubroutine(int damageAmount, bool isBomb, bool didCrit, float critMultiplier,
			Monster monster, Farmer who)
		{
			if (damageAmount <= 0 || isBomb ||
			    who is not {IsLocalPlayer: true, CurrentTool: MeleeWeapon weapon}) return;

			// try to steal
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Poacher") &&
			    !ModEntry.MonstersStolenFrom.Contains(monster.GetHashCode()))
			{
				if (Game1.random.NextDouble() > Util.Professions.GetPoacherStealChance(who)) return;

				var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
					.Concat(monster.getExtraDropItems()).ToList();
				var stolen = drops.ElementAtOrDefault(Game1.random.Next(drops.Count))?.getOne();
				if (stolen == null || !who.addItemToInventoryBool(stolen))
					return;

				ModEntry.MonstersStolenFrom.Add(monster.GetHashCode());
				
				// play sound effect
				try
				{
					if (ModEntry.SoundFX.SoundByName.TryGetValue("poacher_steal", out var sfx))
						sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
					else throw new ContentLoadException();
				}
				catch (Exception ex)
				{
					ModEntry.Log($"Couldn't play sound asset file 'poacher_steal'. Make sure the file exists. {ex}",
						LogLevel.Error);
				}
			}

			// try to increment super mode counters
			if (ModEntry.IsSuperModeActive) return;

			var increment = 0;
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Brute"))
			{
				increment = 2;
				if (monster.Health <= 0) increment *= 2;
				if (weapon.type.Value == MeleeWeapon.club) increment *= 2;
			}
			else if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Poacher") && didCrit)
			{
				increment = (int) Math.Round(critMultiplier * Util.Professions.GetPoacherCritDamageMultiplier(who));
				if (weapon.type.Value == MeleeWeapon.dagger) increment *= 2;
			}

			ModEntry.SuperModeCounter += increment;
		}

		#endregion private methods
	}
}