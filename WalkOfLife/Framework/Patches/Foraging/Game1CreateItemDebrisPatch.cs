using Harmony;
using StardewModdingAPI;
using StardewValley;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class Game1CreateItemDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1CreateItemDebrisPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createItemDebris)),
				postfix: new HarmonyMethod(GetType(), nameof(Game1CreateItemDebrisPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to count foraged berries as Ecologist.</summary>
		protected static void Game1CreateItemDebrisPostfix(Item item)
		{
			if (_IsWildBerry(item) && Globals.LocalPlayerHasProfession("ecologist"))
				++AwesomeProfessions.Data.ItemsForaged;
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given item is salmonberry or blackberry.</summary>
		/// <param name="item'>The given item.</param>
		private static bool _IsWildBerry(Item item)
		{
			return item?.ParentSheetIndex == 296 || item?.ParentSheetIndex == 410;
		}
		#endregion private methods
	}
}
