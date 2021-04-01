using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
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
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationGetFishTranspiler))
			);
		}

		#region harmony patches

		/// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
		private static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.getFish)}.");

			/// Injected: if (!hasRerolled && whichFish > 166 && whichFish < 173 && who.professions.Contains(<fisher_id>)) goto <choose_fish>

			Label reroll = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();
			var hasRerolled = iLGenerator.DeclareLocal(typeof(bool));
			try
			{
				Helper
					.Insert(                                                                // set hasRerolled to false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled)
					)
					.FindLast(                                                              // find index of caught = new Object(whichFish, 1)
						new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(SObject), new Type[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int) }))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.AddLabels(resumeExecution)                                             // branch here if has rerolled or shouldn't reroll
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_S, operand: hasRerolled),
						new CodeInstruction(OpCodes.Brtrue_S, operand: resumeExecution),    // check if has rerolled already
						new CodeInstruction(OpCodes.Ldloc_1),                               // local 1 = whichFish
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 166),                // check if fish index > 166
						new CodeInstruction(OpCodes.Ble_S, operand: resumeExecution),
						new CodeInstruction(OpCodes.Ldloc_1),
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 173),                // check if fish index < 173
						new CodeInstruction(OpCodes.Bge_S, operand: resumeExecution),
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4)              // arg 4 = Farmer who
					)
					.InsertProfessionCheckForSpecificPlayer(Utility.ProfessionMap.Forward["fisher"], resumeExecution)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled),         // set hasRerolled to true
						new CodeInstruction(OpCodes.Br, operand: reroll)
					)
					.RetreatUntil(                                                          // start of choose fish
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SUtility), nameof(SUtility.Shuffle)))
					)
					.Retreat(2)
					.AddLabels(reroll);                                                     // add goto label
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}