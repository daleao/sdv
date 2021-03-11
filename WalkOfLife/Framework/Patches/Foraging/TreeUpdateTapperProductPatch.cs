using Harmony;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TreeUpdateTapperProductPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TreeUpdateTapperProductPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeUpdateTapperProductPostfix))
			);
		}

		/// <summary>Patch to decrease syrup production time for Tapper.</summary>
		protected static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
		{
			if (tapper_instance.heldObject.Value != null && Utils.AnyPlayerHasProfession("tapper", out int n))
			{
				if (tapper_instance.MinutesUntilReady > 0)
					tapper_instance.MinutesUntilReady = (int)(tapper_instance.MinutesUntilReady * Math.Pow(0.75, n));
			}
		}
	}
}
