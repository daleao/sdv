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

		#region private fields
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
			{ 95, 909 },

			// geodes
			{ 75, 535 },
			{ 76, 536 },
			{ 77, 537 },
			{ 819, 749 },

			// gems
			{ 8, 66 },
			{ 10, 68 },
			{ 12, 60 },
			{ 14, 62 },
			{ 6, 70 },
			{ 4, 64 },
			{ 2, 72 },

			// other
			{ 843, 848 },
			{ 844, 848 },
			{ 25, 719 },
			{ 816, 881 },
			{ 817, 881 },
			{ 818, 330 }
		};
		#endregion private fields

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationBreakStonePatch(IMonitor monitor)
		: base(monitor)
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

		#region harmony patches
		/// <summary>Patch to remove Miner extra ore + remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::breakStone.");

			/// From: addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0)
			/// To: addedOres = 0

			try
			{
				_helper
					.FindProfessionCheck(Farmer.miner)		// find index of miner check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Brtrue)	// remove this check
					)
					.Advance()
					.Remove(2)								// remove true case
					.StripLabels();
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Miner extra ore.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// Skipped: if (who.professions.Contains(<geologist_id>)...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.geologist)		// find index of geologist check
					.Retreat()
					.GetLabels(out var labels)					// copy labels
					.StripLabels()								// remove labels from here
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the false case branch
					)
					.GetOperand(out object isNotGeologist)		// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br, (Label)isNotGeologist)
					)
					.Retreat()
					.AddLabels(labels);							// restore labels to inserted branch
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Geologist paired gems.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// Skipped: if (who.professions.Contains(<prospector_id>))...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.burrower)		// find index of prospector check
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)	// the false case branch
					)
					.GetOperand(out object isNotProspector)		// copy destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check
						new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch for Miner extra resources.</summary>
		protected static void GameLocationBreakStonePostfix(ref GameLocation __instance, int indexOfStone, int x, int y, Farmer who, Random r)
		{
			if (Globals.SpecificPlayerHasProfession("miner", who) && r.NextDouble() < 0.10)
			{
				if (_resourceFromStoneId.TryGetValue(indexOfStone, out int indexOfResource))
					Game1.createObjectDebris(indexOfResource, x, y, who.UniqueMultiplayerID, __instance);
				else if (indexOfStone == 44)	// gem node
					Game1.createObjectDebris(Game1.random.Next(1, 8) * 2, x, y, who.UniqueMultiplayerID, __instance);
				else if (indexOfStone == 46)	// mystic stone
				{
					double rolled = r.NextDouble();
					if (rolled < 0.25)
						Game1.createMultipleObjectDebris(74, x, y, 1, who.UniqueMultiplayerID, __instance);	// drop prismatic shard
					else if (rolled < 0.6)
						Game1.createObjectDebris(765, x, y, who.UniqueMultiplayerID, __instance);			// drop iridium ore
					else
						Game1.createObjectDebris(764, x, y, who.UniqueMultiplayerID, __instance);			// drop gold ore
				}
			}
		}
		#endregion harmony patches
	}
}
