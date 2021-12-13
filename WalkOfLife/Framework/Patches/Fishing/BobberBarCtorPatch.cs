using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BobberBarCtorPatch()
		{
			Original = RequireConstructor<BobberBar>(typeof(int), typeof(float), typeof(bool), typeof(int));
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		[HarmonyPostfix]
		private static void BobberBarCtorPostfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			if (!Game1.player.HasProfession("Aquarist")) return;

			var bonusBobberHeight = Utility.Professions.GetAquaristBonusBobberBarHeight(Game1.player);
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}