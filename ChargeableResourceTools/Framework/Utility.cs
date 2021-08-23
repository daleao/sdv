using StardewModdingAPI;

namespace TheLion.Stardew.Tools.Framework
{
	/// <summary>Useful methods that don't fit anywhere specific.</summary>
	public static class Utility
	{
		/// <summary>Whether the player is requesting a charge.</summary>
		public static bool ShouldCharge()
		{
			return !ModEntry.Config.RequireModkey || ModEntry.Config.Modkey.IsDown();
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