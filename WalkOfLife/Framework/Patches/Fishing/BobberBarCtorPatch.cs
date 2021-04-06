using Harmony;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BobberBar), new[] { typeof(int), typeof(float), typeof(bool), typeof(int) }),
				postfix: new HarmonyMethod(GetType(), nameof(BobberBarCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		private static void BobberBarCtorPostfix(ref BobberBar __instance, ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			int bonusBobberHeight = Utility.GetAquaristBonusBobberBarHeight();
			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}