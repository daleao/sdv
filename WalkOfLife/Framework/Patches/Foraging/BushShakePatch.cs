using Harmony;
using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class BushShakePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BushShakePatch(ProfessionsConfig config, IMonitor monitor)
			: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Bush), name: "shake"),
				transpiler: new HarmonyMethod(GetType(), nameof(BushShakeTranspiler))
			);
		}

		/// <summary>Patch to nerf Ecologist berry quality.</summary>
		private static IEnumerable<CodeInstruction> BushShakeTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Bush)}::shake.");

			/// From: Game1.player.professions.Contains(16) ? 4 : 0
			/// To: Game1.player.professions.Contains(16) ? _GetForageQualityForEcologist() : 0

			try
			{
				_helper
					.FindProfessionCheck(Farmer.botanist)		// find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)
					)
					.GetLabels(out List<Label> labels)
					.ReplaceWith(								// replace with custom quality
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BushShakePatch), nameof(_GetForageQualityForEcologist)))
					)
					.SetLabels(labels);
			}
			catch(Exception ex)
			{
				_helper.Error($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		private static int _GetForageQualityForEcologist()
		{
			return AwesomeProfessions.Data.ForageablesCollectedAsEcologist < _config.Ecologist.ForagesNeededForBestQuality ? (AwesomeProfessions.Data.ForageablesCollectedAsEcologist < _config.Ecologist.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}
	}
}
