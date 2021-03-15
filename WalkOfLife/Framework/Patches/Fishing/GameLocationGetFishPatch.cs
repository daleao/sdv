using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationGetFishPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationGetFishPatch(IMonitor monitor)
			: base(monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationGetFishTranspiler))
			);
		}

		#region harmony patches
		/// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationGetFishTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.getFish)}");

			/// Injected: if (_CanReroll(whichFish, who) && !hasRerolled) goto <choose_fish>

			Label reroll = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();
			var hasRerolled = iLGenerator.DeclareLocal(typeof(bool));

			try
			{
				_helper
					.Insert(									// set hasRerolled to false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled)
					)
					.FindLast(									// find index of caught = new Object(whichFish, 1)
						new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(SObject), new Type[] { typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int) }))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldloc_1)
					)
					.AddLabel(resumeExecution)					// branch here if has rerolled
					.Insert(									// insert check if should reroll
						new CodeInstruction(OpCodes.Ldloc_1),					// local 1 = whichFish
						new CodeInstruction(OpCodes.Ldarg_S, operand: (byte)4),	// arg 4 = Farmer who
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationGetFishPatch), nameof(_CanReroll))),
						new CodeInstruction(OpCodes.Brfalse_S, operand: resumeExecution),
						new CodeInstruction(OpCodes.Ldloc_S, operand: hasRerolled),
						new CodeInstruction(OpCodes.Brtrue_S, operand: resumeExecution),
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Stloc_S, operand: hasRerolled),
						new CodeInstruction(OpCodes.Br, operand: reroll)
					)
					.RetreatUntil(								// start of choose fish
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.Shuffle)))
					)
					.Retreat(2)
					.AddLabel(reroll);							// add goto label
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given player is eligible for a fish reroll.</summary>
		/// <param name="index">An item index.</param>
		/// <param name="who">The player.</param>
		private static bool _CanReroll(int index, Farmer who)
		{
			return _IsTrash(index) && Globals.SpecificPlayerHasProfession("fisher", who);
		}

		/// <summary>Whether a given item index corresponds to trash.</summary>
		/// <param name="index">An item index.</param>
		private static bool _IsTrash(int index)
		{
			return index > 166 && index < 173;
		}
		#endregion private methods
	}
}
