namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Linq;
using DaLion.Professions.Framework.Integrations;

#endregion using directives

/// <summary>Extensions for the <see cref="CraftingRecipe"/> class.</summary>
internal static class CraftingRecipeExtensions
{
    internal static void AlterCraftingRecipeForTapper(this CraftingRecipe recipe)
    {
        var betterCraftingPage = BetterCraftingIntegration.Instance?.BetterCraftingPage;
        if (betterCraftingPage is null)
        {
            recipe.recipeList.Clear();
            for (var i = 0; i < State.OriginalRecipeList.Count; i++)
            {
                if (i == State.TapperCraftingIngredientSelected)
                {
                    var ingredientPair = State.OriginalRecipeList.ElementAt(i);
                    var ingredientTotalValue = ItemRegistry.Create<SObject>(ingredientPair.Key).Price * ingredientPair.Value;
                    var sapPrice = ItemRegistry.Create<SObject>(QIDs.Sap).Price;
                    var sapRequiredQuantity = (int)Math.Max((double)ingredientTotalValue / sapPrice, 0);
                    recipe.recipeList["92"] = sapRequiredQuantity;
                    continue;
                }

                recipe.recipeList[State.OriginalRecipeList[i].Key] = State.OriginalRecipeList[i].Value;
            }
        }
        else
        {
            var betterCraftingIntegration = BetterCraftingIntegration.Instance!;
            var ingredients = betterCraftingIntegration.GetIngredientsFromHoveredRecipe();
            if (ingredients.Length <= State.TapperCraftingIngredientSelected)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
                return;
            }

            var ingredient = ingredients.GetValue(State.TapperCraftingIngredientSelected);
            if (ingredient is null)
            {
                ThrowHelper.ThrowInvalidOperationException($"Ingredient at index {State.TapperCraftingIngredientSelected} was null!");
                return;
            }

            var ingredientId = betterCraftingIntegration.GetIngredientId(ingredient);
            var ingredientQuantity = betterCraftingIntegration.GetIngredientQuantity(ingredient);
            var ingredientTotalValue = ItemRegistry.Create<SObject>(ingredientId).Price * ingredientQuantity;
            var sapPrice = ItemRegistry.Create<SObject>(QIDs.Sap).Price;
            var sapRequiredQuantity = (int)Math.Max((double)ingredientTotalValue / sapPrice, 0);

            var sapIngredient = betterCraftingIntegration.ConstructDefaultIngredient("92", sapRequiredQuantity);
            ingredients.SetValue(sapIngredient, State.TapperCraftingIngredientSelected);
            betterCraftingIntegration.ClearCraftCacheForHoveredRecipe();
        }
    }

    internal static void ResetCraftingRecipe(this CraftingRecipe recipe)
    {
        var betterCraftingPage = BetterCraftingIntegration.Instance?.BetterCraftingPage;
        if (betterCraftingPage is null)
        {
            recipe.recipeList = State.OriginalRecipeList.ToDictionary(e => e.Key, e => e.Value);

        }
        else
        {
            var betterCraftingIntegration = BetterCraftingIntegration.Instance!;
            var ingredients = betterCraftingIntegration.GetIngredientsFromHoveredRecipe();
            ingredients.SetValue(betterCraftingIntegration.HoveredIngredientsCopy!.GetValue(State.TapperCraftingIngredientSelected), State.TapperCraftingIngredientSelected);
            betterCraftingIntegration.ClearCraftCacheForHoveredRecipe();
        }
    }
}
