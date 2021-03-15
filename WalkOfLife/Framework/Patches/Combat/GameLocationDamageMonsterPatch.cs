using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationDamageMonsterPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationDamageMonsterPatch(IMonitor monitor)
		: base(monitor)
		{
			_helper = new ILHelper(monitor);
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
					.SetOperand(Globals.ProfessionMap.Forward["gambit"])			// replace with gambit check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldarg_S)					// start of critChance += critChance * 0.5f
					)
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)10)	// arg 10 = Farmer who
					)
					.Advance()
					.ReplaceWith(												// was Ldc_R4 0.5
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationDamageMonsterPatch), nameof(_GetBonusCritChanceForGambit)))
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
					.FindProfessionCheck(Globals.ProfessionMap.Forward["brute"], fromCurrentIndex: true)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 1.15f)
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationDamageMonsterPatch), nameof(_GetBonusDamageMultiplierForBrute)))
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
					.FindProfessionCheck(Farmer.desperado, fromCurrentIndex: true)
					.Advance()
					.SetOperand(Globals.ProfessionMap.Forward["gambit"])
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_R4, operand: 2f)
					)
					.SetOperand(10f);
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
			if (who.IsLocalPlayer && Globals.LocalPlayerHasProfession("brute") && Game1.stats.MonstersKilled > __state)
				Globals.BruteKillStreak += Game1.stats.MonstersKilled - __state;
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		private static float _GetBonusDamageMultiplierForBrute()
		{
			return (float)(1.0 + Globals.BruteKillStreak * 0.005);
		}

		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		/// <param name="who">The player.</param>
		private static float _GetBonusCritChanceForGambit(Farmer who)
		{
			double healthPercent = (double)who.health / who.maxHealth;
			return (float)(0.2 / (healthPercent + 0.2) - 0.2 / 1.2);
		}
		#endregion private methods
	}
}
