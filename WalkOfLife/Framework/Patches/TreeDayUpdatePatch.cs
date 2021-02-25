using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TreeDayUpdatePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TreeDayUpdatePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
				transpiler: new HarmonyMethod(GetType(), nameof(TreeDayUpdateTranspiler))
			);
		}

		/// <summary>Patch to increase Abrorist tree growth speed.</summary>
		protected static IEnumerable<CodeInstruction> TreeDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Tree)}::{nameof(Tree.dayUpdate)}.");

			/// From: if (Game1.random.NextDouble() < 0.15 || (fertilized.Value && Game1.random.NextDouble() < 0.6))
			/// To: if (Game1.random.NextDouble() < Game1.player.professions.Contains(<arborist_id>) ? 0.1875 : 0.15 || (fertilized.Value && Game1.random.NextDouble() < Game1.player.professions.Contains(<arborist_id>) ? 0.9 : 0.6 

			return _helper.Flush();
		}
	}
}
