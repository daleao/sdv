using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationOnStoneDestroyedPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationOnStoneDestroyedPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.OnStoneDestroyed)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationOnStoneDestroyedTranspiler))
			);
		}

		/// <summary>Patch for remove Prospector double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationOnStoneDestroyedTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.OnStoneDestroyed)}.");

			/// Removed: who.professions.Contains(<prospector_id>)...

			try
			{
				_helper
					.FindProfessionCheck(Farmer.burrower)	// find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Bge_Un)	// remove section
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Miner double coal chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
