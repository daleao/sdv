using Harmony;
using StardewValley;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class Game1CreateItemDebrisPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createItemDebris)),
				postfix: new HarmonyMethod(GetType(), nameof(Game1CreateItemDebrisPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increment Ecologist forage counter for wild berries.</summary>
		private static void Game1CreateItemDebrisPostfix(Item item)
		{
			if (Utility.IsWildBerry(item as SObject) && Utility.LocalPlayerHasProfession("Ecologist"))
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: 1);
		}

		#endregion harmony patches
	}
}