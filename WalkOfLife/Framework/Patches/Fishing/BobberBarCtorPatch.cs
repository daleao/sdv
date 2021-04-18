using Harmony;
using StardewValley.Menus;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(BobberBar), new[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
				postfix: new HarmonyMethod(GetType(), nameof(BobberBarCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		private static void BobberBarCtorPostfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			int bonusBobberHeight;
			try
			{
				bonusBobberHeight = Utility.GetAquaristBonusBobberBarHeight();
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(BobberBarCtorPostfix)}:\n{ex}");
				return;
			}
			
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}