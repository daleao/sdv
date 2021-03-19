using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	internal class CraftingRecipeCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CraftingRecipeCtorPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(CraftingRecipe), new Type[] { typeof(string), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(CraftingRecipeCtorPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
		protected static void CraftingRecipeCtorPostfix(ref CraftingRecipe __instance)
		{
			if (__instance.name.Equals("Tapper") && Utility.LocalPlayerHasProfession("tapper"))
			{
				__instance.recipeList = new Dictionary<int, int>
				{
					{ 388, 25 },
					{ 334, 1 }
				};
			}
			else if (__instance.name.Contains("Bomb") && Utility.LocalPlayerHasProfession("blaster"))
			{
				__instance.recipeList = __instance.name switch
				{
					"Cherry Bomb" => new Dictionary<int, int>
					{
						{ 378, 2 },
						{ 382, 1 }
					},
					"Bomb" => new Dictionary<int, int>
					{
						{ 380, 2 },
						{ 382, 1 }
					},
					"Mega Bomb" => new Dictionary<int, int>
					{
						{ 384, 2 },
						{ 382, 1 }
					}
				};
			}
		}
		#endregion harmony patches
	}
}
