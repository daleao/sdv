using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationBreakStonePatch : BasePatch
	{
		private static ILHelper _helper;

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
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationBreakStoneTranspiler))
			);
		}

		/// <summary>Patch for Miner double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationBreakStoneTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::breakStone.");

			/// From: addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0)
			/// To: addedOres = 0

			try
			{
				_helper
					.FindProfessionCheck(ProfessionsMap.Forward["miner"])	// find index of miner check
					.Retreat()
					.Remove(5);												// remove miner check
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Miner remove extra ore.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: who.professions.Contains(<prospector_id>)
			/// To: who.professions.Contains(<miner_id>)

			try
			{
				_helper
					.FindProfessionCheck(ProfessionsMap.Forward["prospector"])	// find index of miner check
					.Advance()
					.SetOperand(ProfessionsMap.Forward["miner"]);				// remove miner check
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Miner double coal chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
