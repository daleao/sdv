#nullable enable
namespace DaLion.Common.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

public interface IBetterCraftingAPI
{
    #region gui

    /// <summary>
    /// Try to open the Better Crafting menu. This may fail if there is another
    /// menu open that cannot be replaced.
    ///
    /// If opening the menu from an object in the world, such as a workbench,
    /// its location and tile position can be provided for automatic detection
    /// of nearby chests.
    ///
    /// Better Crafting has its own handling of mutexes, so please do not worry
    /// about locking Chests before handing them off to the menu.
    ///
    /// When discovering additional containers, Better Crafting scans all tiles
    /// around each of its existing known containers. If a location and position
    /// for the menu source is provided, the tiles around that position will
    /// be scanned as well.
    ///
    /// Discovery depends on the user's settings, though at a minimum a 3x3 area
    /// will be scanned to mimic the scanning radius of the vanilla workbench.
    /// </summary>
    /// <param name="cooking">If true, open the cooking menu. If false, open the crafting menu.</param>
    /// <param name="silent_open">If true, do not make a sound upon opening the menu.</param>
    /// <param name="location">The map the associated object is in, or null if there is no object</param>
    /// <param name="position">The tile position the associated object is at, or null if there is no object</param>
    /// <param name="area">The tile area the associated object covers, or null if there is no object or if the object only covers a single tile</param>
    /// <param name="discover_containers">If true, attempt to discover additional material containers.</param>
    /// <param name="containers">An optional list of containers to draw extra crafting materials from.</param>
    /// <param name="listed_recipes">An optional list of recipes by name. If provided, only these recipes will be listed in the crafting menu.</param>
    /// <returns>Whether or not the menu was opened successfully</returns>
    public bool OpenCraftingMenu(
		bool cooking,
		bool silent_open = false,
		GameLocation? location = null,
		Vector2? position = null,
		Rectangle? area = null,
		bool discover_containers = true,
		IList<Tuple<object, GameLocation>>? containers = null,
		IList<string>? listed_recipes = null
	);

	public Type GetMenuType();

    #endregion gui

    #region recipes

    /// <summary>
    /// Register a recipe provider with Better Crafting. Calling this
    /// will also invalidate the recipe cache.
    ///
    /// If the recipe provider was already registered, this does nothing.
    /// </summary>
    /// <param name="provider">The recipe provider to add</param>
    public void AddRecipeProvider(IRecipeProvider provider);

    /// <summary>
    /// Unregister a recipe provider. Calling this will also invalidate
    /// the recipe cache.
    ///
    /// If the recipe provider was not registered, this does nothing.
    /// </summary>
    /// <param name="provider">The recipe provider to remove</param>
    public void RemoveRecipeProvider(IRecipeProvider provider);

    /// <summary>
    /// Invalidate the recipe cache. You should call this if your recipe
    /// provider ever adds new recipes after registering it.
    /// </summary>
    public void InvalidateRecipeCache();

    /// <summary>
    /// Get all known recipes from all providers.
    /// </summary>
    /// <param name="cooking">If true, return cooking recipes. If false, return
    /// crafting recipes.</param>
    /// <returns>A collection of the recipes.</returns>
    public IReadOnlyCollection<IRecipe> GetRecipes(bool cooking);

    /// <summary>
    /// Create a simple <see cref="IRecipe"/> that gets its properties from an
    /// existing <see cref="CraftingRecipe"/> but that uses different
    /// <see cref="IIngredient"/>s.
    /// </summary>
    /// <param name="recipe">The <see cref="CraftingRecipe"/> to use
    /// as a base.</param>
    /// <param name="ingredients">An enumeration of <see cref="IIngredient"/>s
    /// the recipe should consume.</param>
    /// <param name="onPerformCraft">An optional event handler to perform
    /// additional logic when the item is crafted.</param>
    public IRecipe CreateRecipeWithIngredients(CraftingRecipe recipe, IEnumerable<IIngredient> ingredients,
        Action<IPerformCraftEvent>? onPerformCraft = null);

    #endregion recipes

    #region ingredients

    /// <summary>
    /// Create a simple <see cref="IIngredient"/> that matches an item by ID
    /// and that consumes an exact quantity.
    /// </summary>
    /// <param name="item">The item ID to match.</param>
    /// <param name="quantity">The quantity to consume.</param>
    public IIngredient CreateBaseIngredient(int item, int quantity);

    /// <summary>
    /// Create a simple <see cref="IIngredient"/> that matches a specific
    /// currency and consumes an exact quantity.
    /// </summary>
    /// <param name="type">The currency to match.</param>
    /// <param name="quantity">The quantity to consume.</param>
    public IIngredient CreateCurrencyIngredient(CurrencyType type, int quantity);

    /// <summary>
    /// Create a simple <see cref="IIngredient"/> that matches items using a
    /// function and that consumes an exact quantity.
    /// </summary>
    /// <param name="matcher">The function to check items</param>
    /// <param name="quantity">The quantity to consume.</param>
    /// <param name="displayName">The name to display for the ingredient.</param>
    /// <param name="texture">The texture to display the ingredient with.</param>
    /// <param name="source">The source rectangle of the texture to display.</param>
    public IIngredient CreateMatcherIngredient(Func<Item, bool> matcher, int quantity, string displayName,
        Texture2D texture, Rectangle? source = null);

    /// <summary>
    /// Create a simple <see cref="IIngredient"/> that does not match anything
    /// but requires a quantity of one, thus always preventing a recipe
    /// from being crafted. It displays as an error item in the
    /// ingredients list.
    /// </summary>
    public IIngredient CreateErrorIngredient();

    /// <summary>
    /// Consume matching items from a player, and also from a set of
    /// <see cref="IInventory"/> instances. This is a helper method for
    /// building custom <see cref="IIngredient"/>s.
    /// </summary>
    /// <param name="items">An enumeration of tuples where the function
    /// matches items, and the integer is the quantity to consume.</param>
    /// <param name="who">The player to consume items from, if any. Items
    /// are consumed from the player's inventory first.</param>
    /// <param name="inventories">An enumeration of <see cref="IInventory"/>
    /// instances to consume items from, such as the one passed to
    /// <see cref="IIngredient.Consume(Farmer, IList{IInventory}?, int, bool)"/>.</param>
    /// <param name="maxQuality">The maximum quality to consume.</param>
    /// <param name="lowQualityFirst">Whether or not to consume low quality
    /// items first.</param>
    public void ConsumeItems(IEnumerable<(Func<Item, bool>, int)> items, Farmer? who,
        IEnumerable<IInventory>? inventories, int maxQuality = int.MaxValue, bool lowQualityFirst = false);

    #endregion ingredients

    #region categories

    /// <summary>
    /// Create a new default category for recipes. Every player will receive
    /// this category, but they may delete it or alter it as they see fit.
    /// </summary>
    /// <param name="cooking">If true, this category is added to cooking.
    /// Otherwise, crafting.</param>
    /// <param name="categoryId">An internal ID for the category. Make sure
    /// this is unique.</param>
    /// <param name="Name">A human-readable name displayed in the menu.</param>
    /// <param name="recipeNames">An enumeration of recipe names for recipes to
    /// display in the category.</param>
    /// <param name="iconRecipe">The name of a recipe to use as the category's
    /// default icon.</param>
    public void CreateDefaultCategory(bool cooking, string categoryId, string Name,
        IEnumerable<string>? recipeNames = null, string? iconRecipe = null);

    /// <summary>
    /// Add recipes to a default category. If a player has modified their
    /// category, this will not affect them.
    /// </summary>
    /// <param name="cooking">If true, we alter a cooking category.
    /// Otherwise, crafting.</param>
    /// <param name="categoryId">The ID of the category to alter.</param>
    /// <param name="recipeNames">An enumeration of recipe names for recipes to
    /// add to the category.</param>
    public void AddRecipesToDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames);

    /// <summary>
    /// Remove recipes from a default category. If a player has modified their
    /// category, this will not affect them.
    /// </summary>
    /// <param name="cooking">If true, we alter a cooking category.
    /// Otherwise, crafting.</param>
    /// <param name="categoryId">The ID of the category to alter.</param>
    /// <param name="recipeNames">An enumeration of recipe names for recipes to
    /// remove from the category.</param>
    public void RemoveRecipesFromDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames);

    #endregion categories

    #region inventories

    /// <summary>
    /// Register an inventory provider with Better Crafting. Inventory
    /// providers are used for interfacing with chests and other objects
    /// in the world that contain items.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    public void RegisterInventoryProvider(Type type, IInventoryProvider provider);

    /// <summary>
    /// Unregister an inventory provider.
    /// </summary>
    /// <param name="type"></param>
    public void UnregisterInventoryProvider(Type type);

    #endregion inventories
}