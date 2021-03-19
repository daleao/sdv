using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationDamageMonsterPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		internal GameLocationDamageMonsterPatch()
		{
			_helper = new ILHelper(_monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) }),
				prefix: new HarmonyMethod(GetType(), nameof(GameLocationDamageMonsterPrefix)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationDamageMonsterTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationDamageMonsterPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to count Brute kill streak.</summary>
		protected static bool GameLocationDamageMonsterPrefix(ref uint __state)
		{
			__state = Game1.stats.MonstersKilled;
			return true; // run original logic
		}

		/// <summary>Patch to move crit chance bonus from Scout to Gambit + patch Brute damage bonus + move crit damage bonus from Desperado to Gambit.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationDamageMonsterTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.damageMonster)}.");

			/// From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f
			/// To: if (who.professions.Contains(<gambit_id>) critChance += _GetBonusCritChanceForGambit()

			try
			{
				_helper
					.FindProfessionCheck(Farmer.scout)							// find index of scout check
					.Advance()
					.SetOperand(Utility.ProfessionMap.Forward["gambit"])		// replace with gambit check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_S)					// start of critChance += critChance * 0.5f
					)
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)10)	// arg 10 = Farmer who
					)
					.Advance()
					.ReplaceWith(												// was Ldc_R4 0.5
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.GetGambitBonusCritChance)))
					)
					.Advance()
					.Remove();													// was Mul
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while moving modded bonus crit chance from Scout to Gambit.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: if (who != null && who.professions.Contains(<brute_id>) ... *= 1.15f
			/// To: if (who != null && who.professions.Contains(<brute_id>) ... *= _GetBonusDamageMultiplierForBrute()

			try
			{
				_helper
					.FindProfessionCheck(Utility.ProfessionMap.Forward["brute"], fromCurrentIndex: true)	// find index of brute check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 1.15f)									// brute damage multiplier
					)
					.ReplaceWith(																			// replace with custom multiplier
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.GetBruteBonusDamageMultiplier)))
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: if (who != null && who.professions.Contains(<desperado_id>) ... *= 2f
			/// To: if (who != null && who.professions.Contains(<gambit_id>) ... *= 3f

			try
			{
				_helper
					.FindProfessionCheck(Farmer.desperado, fromCurrentIndex: true)	// find index of desperado check
					.Advance()
					.SetOperand(Utility.ProfessionMap.Forward["gambit"])			// change to gambit check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 2f)			// desperado crit damage multiplier
					)
					.SetOperand(10f);												// replace with custom multiplier
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while moving modded bonus crit damage from Desperado to Gambit.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch to count Brute kill streak.</summary>
		protected static void GameLocationDamageMonsterPostfix(ref uint __state, Farmer who)
		{
			if (who.IsLocalPlayer && Utility.LocalPlayerHasProfession("brute") && Game1.stats.MonstersKilled > __state)
				AwesomeProfessions.BruteKillStreak += Game1.stats.MonstersKilled - __state;
		}
		#endregion harmony patches
	}
}
