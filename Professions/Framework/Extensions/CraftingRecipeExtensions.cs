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
        var bcMenu = BetterCraftingIntegration.Instance?.Menu;
        if (bcMenu is null)
        {
            recipe.recipeList.Clear();
            for (var i = 0; i < State.OriginalRecipeList.Count; i++)
            {
                if (i == State.TapperCraftingIngredientSelected)
                {
                    var originalPair = State.OriginalRecipeList.ElementAt(i);
                    var originalTotalValue = ItemRegistry.Create<SObject>(originalPair.Key).Price * originalPair.Value;
                    var sapPrice = ItemRegistry.Create<SObject>(QIDs.Sap).Price;
                    var sapRequiredQuantity = (int)Math.Max((double)originalTotalValue / sapPrice, 1);
                    recipe.recipeList["92"] = sapRequiredQuantity;

                    continue;
                }

                recipe.recipeList[State.OriginalRecipeList[i].Key] = State.OriginalRecipeList[i].Value;
            }
        }
        else
        {
            var bcIntegration = BetterCraftingIntegration.Instance!;
            bcIntegration.AssertLoaded();

            var ingredients = bcMenu.ActiveRecipe?.Ingredients;
            var reflectedIngredients = bcIntegration.GetIngredientsFromHoveredRecipe();
            if (ingredients is null || ingredients.Length <= State.TapperCraftingIngredientSelected ||
                reflectedIngredients is null || ingredients.Length != reflectedIngredients.Length)
            {
                ThrowHelper.ThrowInvalidOperationException();
                return;
            }

            var ingredient = ingredients[State.TapperCraftingIngredientSelected];
            var reflectedIngredient = reflectedIngredients.GetValue(State.TapperCraftingIngredientSelected);
            if (ingredient is null || reflectedIngredient is null)
            {
                ThrowHelper.ThrowInvalidOperationException($"Ingredient at index {State.TapperCraftingIngredientSelected} was null!");
                return;
            }

            var ingredientId = bcIntegration.GetIngredientId(reflectedIngredient);
            var ingredientQuantity = ingredient.Quantity;
            var ingredientTotalValue = ItemRegistry.Create<SObject>(ingredientId).Price * ingredientQuantity;
            var sapPrice = ItemRegistry.Create<SObject>(QIDs.Sap).Price;
            var sapRequiredQuantity = (int)Math.Max((double)ingredientTotalValue / sapPrice, 1);

            //var sapIngredient = bcIntegration.ModApi.CreateBaseIngredient("92", sapRequiredQuantity);
            var sapIngredient = bcIntegration.GetNewIngredient("92", sapRequiredQuantity);
            if (sapIngredient is null)
            {
                return;
            }

            //ingredients[State.TapperCraftingIngredientSelected] = sapIngredient;
            //var builder = bcIntegration.ModApi.RecipeBuilder(recipe);
            //builder.ClearIngredients();
            //builder.AddIngredients(ingredients);
            reflectedIngredients.SetValue(sapIngredient, State.TapperCraftingIngredientSelected);
            bcIntegration.ClearCraftCacheForHoveredRecipe();
        }
    }

    internal static void AlterCraftingRecipeForLuremaster(this CraftingRecipe recipe, SObject substitute)
    {
        var bcMenu = BetterCraftingIntegration.Instance?.Menu;
        if (bcMenu is null)
        {
            recipe.recipeList.Clear();
            var originalPair = State.OriginalRecipeList.First();
            var baitTotalValue = ItemRegistry.Create<SObject>(originalPair.Key).Price * originalPair.Value;
            var substituteRequiredQuantity = (int)Math.Max((double)baitTotalValue / substitute.Price, 1);
            recipe.recipeList[substitute.ItemId] = substituteRequiredQuantity;

            var numberProducedPerCraft = (int)Math.Max((double)substitute.Price / baitTotalValue, 1);
            recipe.numberProducedPerCraft = numberProducedPerCraft;
        }
        else
        {
            var bcIntegration = BetterCraftingIntegration.Instance!;
            bcIntegration.AssertLoaded();

            var ingredients = bcMenu.ActiveRecipe?.Ingredients;
            var reflectedIngredients = bcIntegration.GetIngredientsFromHoveredRecipe();
            if (ingredients is null || ingredients.Length != 1 ||
                reflectedIngredients is null || ingredients.Length != reflectedIngredients.Length)
            {
                ThrowHelper.ThrowInvalidOperationException();
                return;
            }

            var ingredient = ingredients[0];
            var reflectedIngredient = reflectedIngredients.GetValue(0);
            if (ingredient is null || reflectedIngredient is null)
            {
                ThrowHelper.ThrowInvalidOperationException($"Ingredient at index zero was null!");
                return;
            }

            var originalId = bcIntegration.GetIngredientId(reflectedIngredient);
            var originalQuantity = ingredient.Quantity;
            var baitTotalValue = ItemRegistry.Create<SObject>(originalId).Price * originalQuantity;
            var substituteRequiredQuantity = (int)Math.Max((double)baitTotalValue / substitute.Price, 1);

            var numberProducedPerCraft = (int)Math.Max((double)substitute.Price / baitTotalValue, 1);
            recipe.numberProducedPerCraft = numberProducedPerCraft;

            //var substituteIngredient = bcIntegration.ModApi.CreateBaseIngredient(substitute.ItemId, substituteRequiredQuantity);
            var substituteIngredient = bcIntegration.GetNewIngredient(substitute.ItemId, substituteRequiredQuantity);
            if (substituteIngredient is null)
            {
                return;
            }

            //var builder = bcIntegration.ModApi.RecipeBuilder(recipe);
            //builder.ClearIngredients();
            //builder.AddIngredient(substituteIngredient);
            reflectedIngredients.SetValue(substituteIngredient, 0);
            bcIntegration.ClearCraftCacheForHoveredRecipe();
        }
    }

    internal static void ResetTapperCraftingRecipe(this CraftingRecipe recipe)
    {
        var bcMenu = BetterCraftingIntegration.Instance?.Menu;
        if (bcMenu is null)
        {
            recipe.recipeList = State.OriginalRecipeList.ToDictionary(e => e.Key, e => e.Value);
        }
        else
        {
            var bcIntegration = BetterCraftingIntegration.Instance!;
            bcIntegration.AssertLoaded();

            var ingredients = bcMenu.ActiveRecipe?.Ingredients;
            if (ingredients is null || bcIntegration.HoveredIngredientsCopy is null)
            {
                ThrowHelper.ThrowInvalidOperationException();
                return;
            }

            ingredients[State.TapperCraftingIngredientSelected] = bcIntegration.HoveredIngredientsCopy[State.TapperCraftingIngredientSelected];
            bcIntegration.ClearCraftCacheForHoveredRecipe();
        }

        recipe.numberProducedPerCraft = State.OriginalQuantityPerCraft;
    }

    internal static void ResetLuremasterCraftingRecipe(this CraftingRecipe recipe)
    {
        var bcMenu = BetterCraftingIntegration.Instance?.Menu;
        if (bcMenu is null)
        {
            recipe.recipeList = State.OriginalRecipeList.ToDictionary(e => e.Key, e => e.Value);
        }
        else
        {
            var bcIntegration = BetterCraftingIntegration.Instance!;
            bcIntegration.AssertLoaded();

            var ingredients = bcMenu.ActiveRecipe?.Ingredients;
            if (ingredients is null || bcIntegration.HoveredIngredientsCopy is null)
            {
                ThrowHelper.ThrowInvalidOperationException();
                return;
            }

            ingredients[0] = bcIntegration.HoveredIngredientsCopy[0];
            bcIntegration.ClearCraftCacheForHoveredRecipe();
        }

        recipe.numberProducedPerCraft = State.OriginalQuantityPerCraft;
    }
}
