namespace DaLion.Stardew.Tweaks.Framework.Patches.Rings;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CraftingRecipeConsumeIngredientsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeConsumeIngredientsPatch()
    {
        Original = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.consumeIngredients));
    }

    #region harmony patches

    /// <summary>Overrides ingredient consumption to allow non-SObject types.</summary>
    [HarmonyPrefix]
    private static bool CraftingRecipeConsumeIngredientsPrefix(CraftingRecipe __instance, IList<Chest> additional_materials)
    {
        foreach (var (index, required) in __instance.recipeList)
        {
            var remaining = index.IsRingIndex()
                ? Game1.player.ConsumeRing(index, required)
                : Game1.player.ConsumeObject(index, required);
            if (remaining <= 0 || additional_materials is null) continue;

            foreach (var chest in additional_materials)
            {
                if (chest is null) continue;

                remaining = chest.ConsumeRing(index, remaining);
                if (remaining > 0) continue;
                
                chest.clearNulls();
                break;
            }
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}