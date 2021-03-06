using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationCheckActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal GameLocationCheckActionPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
				prefix: new HarmonyMethod(GetType(), nameof(GameLocationCheckActionPrefix))
			);
		}

		protected static bool GameLocationCheckActionPrefix()
		{
			return true; // run original logic;
		}
	}
}
