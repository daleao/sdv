using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class FishingRodStartMinigameEndFunctionPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal FishingRodStartMinigameEndFunctionPatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
				transpiler: new HarmonyMethod(GetType(), nameof(FishingRodStartMinigameEndFunctionTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch to remove Pirate bonus treasure chance.</summary>
		protected static IEnumerable<CodeInstruction> FishingRodStartMinigameEndFunctionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(FishingRod)}::{nameof(FishingRod.startMinigameEndFunction)}.");

			/// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

			try
			{
				_Helper										// find index of pirate check
					.FindProfessionCheck(Farmer.pirate)
					.Retreat(2)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Add)	// remove this check
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}
		#endregion harmony patches
	}
}
