using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationCheckActionPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationCheckActionPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				_TargetMethod(),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationCheckActionTranspiler))
			);
		}

		/// <summary>Patch to nerf Ecologist forage quality.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationCheckActionTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.checkAction)}.");

			/// From: objects[vector].Quality = 4
			/// To: objects[vector].Quality = _GetForageQualityForEcologist()

			try
			{
				_helper
					.FindProfessionCheck(Farmer.botanist)		// find index of botanist check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_4)	// start of objects[vector].Quality = 4
					)
					.ReplaceWith(								// replace with custom quality
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationCheckActionPatch), nameof(_GetForageQualityForEcologist)))
					);
			}
			catch(Exception ex)
			{
				_helper.Error($"Failed while patching modded Ecologist forage quality.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		private static int _GetForageQualityForEcologist()
		{
			return AwesomeProfessions.Data.ForageablesCollectedAsEcologist < _config.Ecologist.ForagesNeededForBestQuality ? (AwesomeProfessions.Data.ForageablesCollectedAsEcologist < _config.Ecologist.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the inner method to patch.</summary>
		private static MethodBase _TargetMethod()
		{
			var targetMethod = typeof(GameLocation).InnerMethodsStartingWith("<checkAction>b__0").First();
			if (targetMethod == null)
				throw new MissingMethodException("Target method '<checkAction>b__0' was not found.");

			return targetMethod;
		}
	}
}
