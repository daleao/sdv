using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GameLocationDamageMonsterPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationDamageMonsterPatch()
		{
			Original = typeof(GameLocation).MethodNamed(nameof(GameLocation.damageMonster), new[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) });
			Transpiler = new HarmonyMethod(GetType(), nameof(GameLocationDamageMonsterTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to move critical chance bonus from Scout to Hunter + patch Brute damage bonus + move critical damage bonus from Desperado to Hunter + increment Brute Fury and Hunter Cold Blood counters + perform Hunter steal.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f
			/// To: if (who.professions.Contains(<hunter_id>) critChance += GetHunterBonusCritChance()

			try
			{
				Helper
					.FindProfessionCheck(Farmer.scout) // find index of scout check
					.Advance()
					.SetOperand(Util.Professions.IndexOf("Hunter")) // replace with Hunter check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_S) // start of critChance += critChance * 0.5f
					)
					.Advance()
					//.ReplaceWith(
					//	new CodeInstruction(OpCodes.Ldarg_S,
					//		operand: (byte)10) // was Ldarg_S critChance (arg 10 = Farmer who)
					//)
					//.Advance()
					.Remove()
					.ReplaceWith( // was Ldc_R4 0.5
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetHunterBonusCritChance)))
					)
					.Advance()
					.Remove(); // was Mul
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while moving modded bonus crit chance from Scout to Hunter.\nHelper returned {ex}");
				return null;
			}

			/// From: if (who != null && who.professions.Contains(<brute_id>) ... *= 1.15f
			/// To: if (who != null && who.professions.Contains(<brute_id>) ... *= GetBruteBonusDamageMultiplier(who)

			try
			{
				Helper
					.FindProfessionCheck(Util.Professions.IndexOf("Brute"),
						fromCurrentIndex: true) // find index of brute check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 1.15f) // brute damage multiplier
					)
					.ReplaceWith( // replace with custom multiplier
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetBruteBonusDamageMultiplier)))
					)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, (byte)10) // arg 10 = Farmer who
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");
				return null;
			}

			/// From: if (who != null && who.professions.Contains(<desperado_id>) ... *= 2f
			/// To: if (who != null && who.professions.Contains(<hunter_id>) ... *= GetHunterCritDamageMultiplier

			try
			{
				Helper
					.FindProfessionCheck(Farmer.desperado, fromCurrentIndex: true) // find index of desperado check
					.Advance()
					.SetOperand(Util.Professions.IndexOf("Hunter")) // change to Hunter check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, 2f) // desperado critical damage multiplier
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldarg_S, (byte)10) // was Ldc_R4 2f (arg 10 = Farmer who)
					)
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetHunterCritDamageMultiplier)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while moving modded bonus crit damage from Desperado to Hunter.\nHelper returned {ex}");
				return null;
			}

			/// Before: damageAmount = monster.takeDamage(damageAmount, (int)trajectory.X, (int)trajectory.Y, isBomb, addedPrecision / 10.0, 

			/// Injected: tryToStealAndIncrementCountersOrEndHunterSuperMode(damageAmount, isBomb, crit, critMultiplier, monster, who)
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
						new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = bool isBomb
						new CodeInstruction(OpCodes.Ldloc_S, didCrit),
						new CodeInstruction(OpCodes.Ldarg_S, (byte)8), // arg 8 = float critMultiplier
						new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
						new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
						new CodeInstruction(OpCodes.Call,
							typeof(GameLocationDamageMonsterPatch).MethodNamed(nameof(GameLocationDamageMonsterPatch.TryToStealAndIncrementCounters)))
					)
					.Return()
					.AddLabels(labels);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while injecting modded Hunter snatch attempt plus Brute Fury and Hunter Cold Blood counters.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		/// <summary>Try to steal an item if the player is Hunter, and also increment Brute and Hunter counters if applicable. If the monster was not killed, end Hunter super mode.</summary>
		/// <param name="damageAmount">The amount of damage dealt to the monster.</param>
		/// <param name="isBomb">Whether the damgae source is a bomb.</param>
		/// <param name="didCrit">Whether the player scored a critical strike.</param>
		/// <param name="critMultiplier">The player's raw critical power before profession bonuses.</param>
		/// <param name="monster">The target monster.</param>
		/// <param name="who">The player.</param>
		private static void TryToStealAndIncrementCounters(int damageAmount, bool isBomb, bool didCrit, float critMultiplier, Monster monster, Farmer who)
		{
			if (damageAmount <= 0 || isBomb || who is not { IsLocalPlayer: true, CurrentTool: MeleeWeapon weapon }) return;
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Hunter")) TryToStealItem(monster, who);
			if (!ModEntry.IsSuperModeActive) TryToIncrementSuperModeCounter(didCrit, critMultiplier, weapon, monster, who);
		}

		/// <summary>Try to increment Brute or Hunter counters.</summary>
		/// <param name="didCrit">Whether the player scored a critical strike.</param>
		/// <param name="critMultiplier">The player's raw critical power before profession bonuses.</param>
		/// <param name="weapon">The player's current weapon.</param>
		/// <param name="monster">The target monster.</param>
		/// <param name="who">The player.</param>
		private static void TryToIncrementSuperModeCounter(bool didCrit, float critMultiplier, MeleeWeapon weapon, Monster monster, Farmer who)
		{
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Brute"))
			{
				++ModEntry.SuperModeCounter;
				if (monster.Health <= 0) ++ModEntry.SuperModeCounter;
				if (weapon.type.Value == MeleeWeapon.club) ++ModEntry.SuperModeCounter;
			}
			else if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Hunter") && didCrit)
			{
				ModEntry.SuperModeCounter += (int)Math.Round(critMultiplier * Util.Professions.GetHunterCritDamageMultiplier(who));
			}
		}

		/// <summary>Try to steal an item from the target monster.</summary>
		/// <param name="monster">The target monster.</param>
		/// <param name="who">The player.</param>
		private static void TryToStealItem(Monster monster, Farmer who)
		{
			if (Game1.random.NextDouble() > Util.Professions.GetHunterStealChance(who)) return;

			var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item).Concat(monster.getExtraDropItems()).ToList();
			var stolen = drops[Game1.random.Next(drops.Count)].getOne();
			if (stolen != null && who.addItemToInventoryBool(stolen) && ModEntry.SfxLoader.SfxByName.TryGetValue("hunter_steal", out var sfx))
				sfx.CreateInstance().Play();
		}

		#endregion private methods
	}
}