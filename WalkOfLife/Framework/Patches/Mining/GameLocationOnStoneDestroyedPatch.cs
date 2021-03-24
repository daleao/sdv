using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationOnStoneDestroyedPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal GameLocationOnStoneDestroyedPatch()
		{
			_Helper = new ILHelper(Monitor);
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

		#region harmony patches
		/// <summary>Patch to remove Prospector double coal chance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationOnStoneDestroyedTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.OnStoneDestroyed)}.");

			/// From: random.NextDouble() < 0.035 * (double)(!who.professions.Contains(<prospector_id>) ? 1 : 2)
			/// To: random.NextDouble() < 0.035

			try
			{
				_Helper
					.FindProfessionCheck(Farmer.burrower)	// find index of prospector check
					.Retreat()
					.RemoveUntil(
						new CodeInstruction(OpCodes.Mul)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Prospector double coal chance.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
