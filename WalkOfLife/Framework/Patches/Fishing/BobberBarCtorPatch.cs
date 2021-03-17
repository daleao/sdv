using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BobberBarCtorPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BobberBar), new Type[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
				postfix: new HarmonyMethod(GetType(), nameof(BobberBarCtorPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		protected static void BobberBarCtorPostfix(ref BobberBar __instance, ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			int bonusBobberHeight = Utility.GetAquaristBonusBobberBarHeight();
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}
		#endregion harmony patches
	}
}
