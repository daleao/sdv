using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CrabPotPerformObjectDropInActionPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CrabPotPerformObjectDropInActionPatch(ModConfig config, IMonitor monitor)
			: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(CrabPot), nameof(CrabPot.performObjectDropInAction)),
				transpiler: new HarmonyMethod(GetType(), nameof(CrabPotPerformObjectDropInActionTranspiler))
			);
		}

		/// <summary></summary>
		private static IEnumerable<CodeInstruction> CrabPotPerformObjectDropInActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(CrabPot)}::{nameof(CrabPot.performObjectDropInAction)}.");

			/// Skipped: if (who.professions.Contains(<luremaster_id>)...

			try
			{
				_helper											// find index of luremaster check
					.FindProfessionCheck(Farmer.mariner)
					.Retreat()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brtrue)		// end of profession check
					)
					.GetOperand(out object resumeExecution)		// copy false case destination
					.Return()
					.Insert(									// insert uncoditional branch to skip this check and resume execution
						new CodeInstruction(OpCodes.Br_S, (Label)resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Luremaster remove bait requirement.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
