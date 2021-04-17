using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class FishingRodStartMinigameEndFunctionPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
				transpiler: new HarmonyMethod(GetType(), nameof(FishingRodStartMinigameEndFunctionTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch to remove Pirate bonus treasure chance.</summary>
		private static IEnumerable<CodeInstruction> FishingRodStartMinigameEndFunctionTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(FishingRod)}::{nameof(FishingRod.startMinigameEndFunction)}.");

			/// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

			try
			{
				Helper // find index of pirate check
					.FindProfessionCheck(Farmer.pirate)
					.Retreat(2)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Add) // remove this check
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}