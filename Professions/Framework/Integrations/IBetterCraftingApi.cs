namespace DaLion.Professions.Framework.Integrations;

#region using directives

using System.Collections.Generic;
using StardewValley.Menus;

#endregion using directives

/// <summary>The public interface for the Better Crafting mod's API.</summary>
public interface IBetterCraftingApi
{
    /// <summary>
    /// An <c>IIngredient</c> represents a single ingredient used when crafting a
    /// recipe. An ingredient can be an item, a currency, or anything else.
    ///
    /// The API provides methods for getting basic item and currency ingredients,
    /// so you need not use this unless you're doing something fancy.
    /// </summary>
    public interface IIngredient
    {
        /// <summary>
        /// The amount of this ingredient required to perform a craft.
        /// </summary>
        int Quantity { get; }
    }

    /// <summary>
    /// An <c>IRecipe</c> represents a single crafting recipe, though it need not
    /// be associated with a vanilla <see cref="StardewValley.CraftingRecipe"/>.
    /// Recipes usually produce <see cref="Item"/>s, but they are not required
    /// to do so.
    /// </summary>
    public interface IRecipe
    {
        /// <summary>
        /// The internal name of the recipe. For standard recipes, this matches the
        /// name of the recipe used in the player's cookingRecipes / craftingRecipes
        /// dictionaries. For non-standard recipes, this can be anything as long as
        /// it's unique, and it's recommended to prefix the names with your mod's
        /// unique ID to ensure uniqueness.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The vanilla <c>CraftingRecipe</c> instance for this recipe, if one
        /// exists. This may be used for interoperability with some other
        /// mods, but is not required.
        /// </summary>
        CraftingRecipe? CraftingRecipe { get; }

        /// <summary>
        /// The ingredients used by this recipe.
        /// </summary>
        IIngredient[]? Ingredients { get; }
    }

    ///// <summary>
    ///// This class allows you to easily modify any part of an <see cref="IRecipe"/>'s
    ///// behavior, including its appearance, cost, and the item(s) it produces.
    /////
    ///// This is primarily meant for customizing how an existing <see cref="CraftingRecipe"/>
    ///// functions, but can be used for creating new recipes.
    ///// </summary>
    //public interface IRecipeBuilder
    //{
    //    /// <summary>
    //    /// Clear the recipe's ingredients list. Optionally, a predicate can be
    //    /// provided to only clear ingredients from the list that match the predicate.
    //    /// </summary>
    //    /// <param name="predicate">An optional predicate for selecting which
    //    /// ingredients should be removed.</param>
    //    /// <returns>The same <see cref="IRecipeBuilder"/> instance</returns>
    //    IRecipeBuilder ClearIngredients(Func<IIngredient, bool>? predicate = null);

    //    /// <summary>
    //    /// Add a new ingredient to the recipe's ingredients list.
    //    /// </summary>
    //    /// <param name="ingredient">The ingredient to add</param>
    //    /// <returns>The same <see cref="IRecipeBuilder"/> instance</returns>
    //    IRecipeBuilder AddIngredient(IIngredient ingredient);

    //    /// <summary>
    //    /// Add multiple new ingredients to the recipe's ingredients list.
    //    /// </summary>
    //    /// <param name="ingredients">The ingredients to add</param>
    //    /// <returns>The same <see cref="IRecipeBuilder"/> instance</returns>
    //    IRecipeBuilder AddIngredients(IEnumerable<IIngredient> ingredients);
    //}

    /// <summary>
    /// This interface contains a few basic properties on the Better Crafting
    /// menu that may be useful for other mods.
    /// </summary>
    public interface IBetterCraftingMenu
    {
        /// <summary>
        /// The <see cref="IClickableMenu"/> instance for this menu. This is the
        /// same object, but included for convenience due to how API proxying works.
        /// </summary>
        IClickableMenu Menu { get; }

        /// <summary>
        /// Get the current recipe. This is normally the recipe that the
        /// player's cursor is hovering over, but when performing a craft
        /// or when the bulk crafting menu is open, it will return the
        /// relevant recipe.
        /// </summary>
        IRecipe? ActiveRecipe { get; }
    }

    /// <summary>
    /// Return the Better Crafting menu's type. In case you want to do
    /// spooky stuff to it, I guess.
    /// </summary>
    /// <returns>The BetterCraftingMenu type.</returns>
    Type GetMenuType();

    /// <summary>
    /// Get the currently open Better Crafting menu. This may be <c>null</c> if
    /// the menu is still opening.
    /// </summary>
    IBetterCraftingMenu? GetActiveMenu();

    /// <summary>
    /// Cast an <see cref="IClickableMenu"/> to a <see cref="IBetterCraftingMenu"/>
    /// if it's an instance of our menu, or return <c>null</c> otherwise.
    /// </summary>
    /// <param name="menu">The menu to cast.</param>
    IBetterCraftingMenu? GetMenu(IClickableMenu menu);

    ///// <summary>
    ///// Get a new <see cref="IRecipeBuilder"/> for customizing a recipe.
    ///// </summary>
    ///// <param name="recipe">The recipe to customize.</param>
    //IRecipeBuilder RecipeBuilder(CraftingRecipe recipe);

    /// <summary>
    /// Create a simple <see cref="IIngredient"/> that matches an item by ID
    /// and that consumes an exact quantity.
    /// </summary>
    /// <param name="item">The item ID to match.</param>
    /// <param name="quantity">The quantity to consume.</param>
    /// <param name="recycleRate">The percentage of items to return when recycling.</param>
    IIngredient CreateBaseIngredient(string item, int quantity, float recycleRate = 1f);
}
