using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationBreakStonePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Look-up table for what resource should spawn from a given stone.</summary>
		private static readonly Dictionary<int, int> _resourceFromStoneId = new Dictionary<int, int>
		{
			// stone
			{ 668, 390 },
			{ 670, 390 },
			{ 845, 390 },
			{ 846, 390 },
			{ 847, 390 },

			// ores
			{ 751, 378 },
			{ 849, 378 },
			{ 290, 380 },
			{ 850, 380 },
			{ 764, 384 },
			{ 765, 386 },

			// geodes
			{ 75, 535 },
			{ 76, 536 },
			{ 77, 537 },

			// gems
			{ 8, 66 },
			{ 10, 68 },
			{ 12, 60 },
			{ 14, 62 },
			{ 6, 70 },
			{ 4, 64 },
			{ 2, 72 },

			// other
			{ 95, 909 },
			{ 843, 848 },
			{ 844, 848 },
			{ 25, 719 },
			{ 816, 881 },
			{ 817, 881 },
			{ 818, 330 },
			{ 819, 749 }
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationBreakStonePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), name: "breakStone"),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationBreakStoneTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationBreakStonePostfix))
			);
		}

		/// <summary>Patch to remove Miner extra ore + remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::breakStone.");

			/// From: addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0)
			/// To: addedOres = 0

			try
			{
				_helper
					.FindProfessionCheck(Farmer.miner)			// find index of miner check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Ldc_I4_0)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Miner remove extra ore.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.geologist)		// find index of geologist check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the branch to resume execution
					)
					.GetOperand(out object resumeExecution)		// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check and resume execution
						new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Blaster remove vanilla extra gems.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// Skipped: if (who.professions.Contains(<prospector_id>))...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.burrower)		// find index of prospector check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the branch to resume execution
					)
					.GetOperand(out object resumeExecution)		// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check and resume execution
						new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Prospector remove vanilla double coal chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch for Miner extra resources.</summary>
		protected static void GameLocationBreakStonePostfix(ref GameLocation __instance, int indexOfStone, int x, int y, Farmer who, Random r)
		{
			if (Utils.PlayerHasProfession("miner", who) && r.NextDouble() < 0.10)
			{
				if (_resourceFromStoneId.TryGetValue(indexOfStone, out int indexOfResource))
				{
					Game1.createObjectDebris(indexOfResource, x, y, who.UniqueMultiplayerID, __instance);
				}
			}
		}
	}
}
