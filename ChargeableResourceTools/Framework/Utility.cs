using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TheLion.AwesomeTools
{
	/// <summary>Useful methods that don't fit anywhere specific.</summary>
	public static class Utility
	{
		/// <summary>Whether an Axe or Pickxae instance should run patched logic or original logic.</summary>
		/// <param name="tool">The tool.</param>
		public static bool ShouldCharge(Tool tool)
		{
			if ((AwesomeTools.Config.RequireModkey && !AwesomeTools.Config.Modkey.IsDown())
				|| (tool is Axe && (!AwesomeTools.Config.AxeConfig.EnableAxeCharging || tool.UpgradeLevel < AwesomeTools.Config.AxeConfig.RequiredUpgradeForCharging))
				|| (tool is Pickaxe && (!AwesomeTools.Config.PickaxeConfig.EnablePickaxeCharging || tool.UpgradeLevel < AwesomeTools.Config.PickaxeConfig.RequiredUpgradeForCharging)))
			{
				return false;
			}

			return true;
		}

		/// <summary>Whether Prismatic or Radioactive Tools mod is installed.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		public static bool HasHigherLevelToolMod(IModRegistry modRegistry)
		{
			return modRegistry.IsLoaded("stokastic.PrismaticTools") || modRegistry.IsLoaded("kakashigr.RadioactiveTools");
		}

		/// <summary>Whether Prismatic or Radioactive Tools mod is installed.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		/// <param name="whichMod">Which of the two mods is installed.</param>
		/// <returns>Returns the name of the installed mod, if either.</returns>
		public static bool HasHigherLevelToolMod(IModRegistry modRegistry, out string whichMod)
		{
			if (modRegistry.IsLoaded("stokastic.PrismaticTools"))
			{
				whichMod = "Prismatic";
				return true;
			}

			if (modRegistry.IsLoaded("kakashigr.RadioactiveTools"))
			{
				whichMod = "Radioactive";
				return true;
			}

			whichMod = "None";
			return false;
		}
	}
}