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

		/// <summary>Patch to count foraged berries as Ecologist.</summary>
		private static void Game1CreateItemDebrisPostfix(Item item)
		{
			if (Utility.IsWildBerry(item as SObject) && Utility.LocalFarmerHasProfession("ecologist"))
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged");
		}

		#endregion harmony patches
	}
}