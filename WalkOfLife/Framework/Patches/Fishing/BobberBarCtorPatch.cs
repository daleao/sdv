using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BobberBarCtorPatch()
		{
			Original = typeof(BobberBar).Constructor(new[] { typeof(int), typeof(float), typeof(bool), typeof(int) });
			Postfix = new(GetType(), nameof(BobberBarCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		[HarmonyPostfix]
		private static void BobberBarCtorPostfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			var bonusBobberHeight = 0;
			try
			{
				if (Game1.player.HasProfession("Aquarist"))
					bonusBobberHeight = Util.Professions.GetAquaristBonusBobberBarHeight();
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return;
			}

			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}