using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CraftingRecipeCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CraftingRecipeCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(CraftingRecipe), new Type[] { typeof(string), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(CraftingRecipeCtorPostfix))
			);
		}

		/// <summary>Patch for cheaper crafting recipes for Blaster and Tapper.</summary>
		protected static void CraftingRecipeCtorPostfix(ref CraftingRecipe __instance)
		{
			if (__instance.name.Equals("Tapper") && Utils.LocalPlayerHasProfession("tapper"))
			{
				__instance.recipeList = new Dictionary<int, int>
				{
					{ 388, 25 },
					{ 334, 1 }
				};
			}
			else if (__instance.name.Contains("Bomb") && Utils.LocalPlayerHasProfession("blaster"))
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
	}
}
