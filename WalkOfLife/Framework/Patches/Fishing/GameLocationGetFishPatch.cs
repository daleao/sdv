using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common;
using SObject = StardewValley.Object;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationGetFishPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationGetFishTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
		private static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.getFish)}.");

			/// Injected: if (!hasRerolled && (whichFish > 166 && whichFish < 173 || whichFish == 152 || whichFish == 153 || whichFish == 157)
			///		&& who.professions.Contains(<fisher_id>) && who.CurrentTool) goto <choose_fish>

			var reroll = iLGenerator.DefineLabel();
			var resumeExecution = iLGenerator.DefineLabel();
			var hasRerolled = iLGenerator.DeclareLocal(typeof(bool));
			try
			{
				Helper
					.Insert( // set hasRerolled to false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled)
					)
					.FindLast( // find index of caught = new Object(whichFish, 1)
						new CodeInstruction(OpCodes.Newobj,
							AccessTools.Constructor(typeof(SObject),
								new[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int) }))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.AddLabels(resumeExecution) // branch here if shouldn't reroll
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4), // arg 4 = Farmer who
						new CodeInstruction(OpCodes.Ldloc_1), // local 1 = whichFish
						new CodeInstruction(OpCodes.Ldloc_S, operand: hasRerolled),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationGetFishPatch), nameof(_ShouldRerollFish))),
						new CodeInstruction(OpCodes.Brtrue_S, operand: resumeExecution),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled), // set hasRerolled to true
						new CodeInstruction(OpCodes.Br, operand: reroll)
					)
					.RetreatUntil( // start of choose fish
						new CodeInstruction(OpCodes.Call,
							AccessTools.Method(typeof(SUtility), nameof(SUtility.Shuffle)))
					)
					.Retreat(2)
					.AddLabels(reroll); // add goto label
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		private static bool _ShouldRerollFish(Farmer who, int currentFish, bool hasRerolled)
		{
			return !hasRerolled && (166 < currentFish && currentFish < 173 || currentFish == 152 || currentFish == 153 || currentFish == 157)
				&& who.CurrentTool is FishingRod rod
				&& Utility.BaitById.TryGetValue(rod.getBaitAttachmentIndex(), out var baitName)
				&& baitName.AnyOf("Bait", "Wild Bait", "Magic Bait")
				&& Utility.SpecificPlayerHasProfession("Fisher", who);
		}
	}
}