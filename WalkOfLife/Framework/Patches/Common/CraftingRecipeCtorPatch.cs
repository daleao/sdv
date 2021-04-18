using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	internal class CraftingRecipeCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(CraftingRecipe), new[] { typeof(string), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(CraftingRecipeCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
		private static void CraftingRecipeCtorPostfix(ref CraftingRecipe __instance)
		{
			try
			{
				if (__instance.name.Equals("Tapper") && Utility.LocalPlayerHasProfession("Tapper"))
				{
					__instance.recipeList = new Dictionary<int, int>
					{
						{ 388, 25 },	// wood
						{ 334, 1 }		// copper bar
					};
				}
				else if (__instance.name.Contains("Bomb") && Utility.LocalPlayerHasProfession("Blaster"))
				{
					__instance.recipeList = __instance.name switch
					{
						"Cherry Bomb" => new Dictionary<int, int>
						{
							{ 378, 2 },	// copper ore
							{ 382, 1 }	// coal
						},
						"Bomb" => new Dictionary<int, int>
						{
							{ 380, 2 },	// iron ore
							{ 382, 1 }	// coal
						},
						"Mega Bomb" => new Dictionary<int, int>
						{
							{ 384, 2 },	// gold ore
							{ 382, 1 }	// coal
						},
						_ => __instance.recipeList
					};
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(CraftingRecipeCtorPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}