using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;

namespace TheLion.AwesomeTools.Framework
{
	/// <summary>Useful methods that don't fit anywhere specific.</summary>
	public static class Utils
	{
		/// <summary>Whether an Axe or Pickxae instance should run patched logic or original logic.</summary>
		/// <param name="tool">The tool.</param>
		public static bool ShouldCharge(Tool tool)
		{
			if (!ModEntry.Config.PickaxeConfig.EnablePickaxeCharging || (ModEntry.Config.RequireModkey && !ModEntry.Config.Modkey.IsDown()))
			{
				return false;
			}

			switch (tool)
			{
				case Axe:
					if (tool.UpgradeLevel < ModEntry.Config.AxeConfig.RequiredUpgradeForCharging)
					{
						return false;
					}
					break;
				case Pickaxe:
					if (tool.UpgradeLevel < ModEntry.Config.PickaxeConfig.RequiredUpgradeForCharging)
					{
						return false;
					}
					break;
			}

			return true;
		}

		/// <summary>Whether Prismatic or Radioactive Tools mod is installed.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		public static bool HasHigherLevelToolMod(IModRegistry modRegistry)
		{
			return modRegistry.IsLoaded("stokastic.PrismaticTools") || modRegistry.IsLoaded("kakashigr.RadioactiveTools");
		}
	}
}
