using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class FishingRodStartMinigameEndFunctionPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FishingRodStartMinigameEndFunctionPatch(ModConfig config, IMonitor monitor)
			: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
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

		/// <summary></summary>
		private static IEnumerable<CodeInstruction> FishingRodStartMinigameEndFunctionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(FishingRod)}::{nameof(FishingRod.startMinigameEndFunction)}.");

			/// Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...

			try
			{
				_helper											// find index of pirate check
					.FindProfessionCheck(Farmer.pirate)
					.Retreat(2)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Brtrue)		// end of profession check
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
