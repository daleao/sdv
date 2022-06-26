namespace DaLion.Common.Integrations;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

#endregion using directives

/// <summary>
/// The various currency types supported by <see cref="IBetterCrafting.CreateCurrencyIngredient(string, int)"/>
/// </summary>
public enum CurrencyType
{
	/// <summary>The player's gold.</summary>
	Money,
	/// <summary>
	    /// The player's earned points at the current festival. This should likely
	    /// never actually be used, since players can't craft while they're at a
	    /// festival in the first place.
	/// </summary>
	FestivalPoints,
	/// <summary>The player's casino points.</summary>
	ClubCoins,
	/// <summary>The player's Qi Gems.</summary>
	QiGems
};


/// <summary>
/// Used by Better Crafting to discover and
/// interact with various item storages in the game.
/// </summary>
public interface IInventoryProvider
{
	/// <summary>
	/// Check to see if this object is valid for inventory operations.
	///
	/// If location is null, it should not be considered when determining
	/// the validity of the object.
	/// 
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns>whether or not the object is valid</returns>
	bool IsValid(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if items can be inserted into this object.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	bool CanInsertItems(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if items can be extracted from this object.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	bool CanExtractItems(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// For objects larger than a single tile on the map, return the rectangle representing
	/// the object. For single tile objects, return null.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <returns></returns>
	Rectangle? GetMultiTileRegion(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Return the real position of the object. If the object has no position, returns null.
	/// For multi-tile objects, this should return the "main" object if there is one. 
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	Vector2? GetTilePosition(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Get the NetMutex that locks the object for multiplayer synchronization. This method must
	/// return a mutex. If null is returned, the object will be skipped.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	NetMutex? GetMutex(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Whether or not a mutex is required for interacting with this object's inventory.
	/// You should always use a mutex to ensure items are handled safely with multiplayer,
	/// but in case you're doing something exceptional and Better Crafting should not
	/// worry about locking, you can explicitly disable mutex handling.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	bool IsMutexRequired(object @object, GameLocation? location, Farmer? who) => true;

	/// <summary>
	/// Get a list of items in the object's inventory, for modification or viewing. Assume that
	/// anything using this list will use GetMutex() to lock the inventory before modifying.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	IList<Item?>? GetItems(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Check to see if a specific item is allowed to be stored in the object's inventory.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	/// <param name="item">the item we're checking</param>
	bool IsItemValid(object @object, GameLocation? location, Farmer? who, Item item) => true;

	/// <summary>
	/// Clean the inventory of the object. This is for removing null entries, organizing, etc.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	void CleanInventory(object @object, GameLocation? location, Farmer? who);

	/// <summary>
	/// Get the actual inventory capacity of the object's inventory. New items may be added to the
	/// GetItems() list up until this count.
	/// </summary>
	/// <param name="object">the object</param>
	/// <param name="location">the map where the object is</param>
	/// <param name="who">the player accessing the inventory, or null if no player is involved</param>
	int GetActualCapacity(object @object, GameLocation? location, Farmer? who);
}


/// <summary>
/// Represents an item storage that
/// Better Crafting is interacting with, whether by extracting
/// items or inserting them.
/// </summary>
public interface IInventory
{
	/// <summary>
	/// The object that has inventory.
	/// </summary>
	object Object { get; }

	/// <summary>
	/// Where this object is located, if a location is relevant.
	/// </summary>
	GameLocation? Location { get; }

	/// <summary>
	/// The player accessing the inventory, if a player is involved.
	/// </summary>
	Farmer? Player { get; }

	/// <summary>
	/// The NetMutex for this object, which should be locked before
	/// using it. If there is no mutex, then we apparently don't
	/// need to worry about that.
	/// </summary>
	NetMutex? Mutex { get; }

	/// <summary>
	/// Whether or not the object is locked and ready for read/write usage.
	/// </summary>
	bool IsLocked();

	/// <summary>
	/// Whether or not the object is a valid inventory.
	/// </summary>
	bool IsValid();

	/// <summary>
	/// Whether or not we can insert items into this inventory.
	/// </summary>
	bool CanInsertItems();

	/// <summary>
	/// Whether or not we can extract items from this inventory.
	/// </summary>
	bool CanExtractItems();

	/// <summary>
	/// For multi-tile inventories, the region that this inventory takes
	/// up in the world. Only rectangular multi-tile inventories are
	/// supported, and this is used primarily for discovering connections.
	/// </summary>
	Rectangle? GetMultiTileRegion();

	/// <summary>
	/// Get the tile position of this object in the world, if it has one.
	/// For multi-tile inventories, this should be the primary tile if
	/// one exists.
	/// </summary>
	Vector2? GetTilePosition();

	/// <summary>
	/// Get this object's inventory as a list of items. May be null if
	/// there is an issue accessing the object's inventory.
	/// </summary>
	IList<Item?>? GetItems();

    /// <summary>
    /// Check to see if a specific item is allowed to be stored in the
    /// object's inventory.
    /// </summary>
    /// <param name="item">The item to check.</param>
    bool IsItemValid(Item item);

	/// <summary>
	/// Attempt to clean the object's inventory. This should remove null
	/// entries, and run any other necessary logic.
	/// </summary>
	void CleanInventory();

	/// <summary>
	/// Get the number of item slots in the object's inventory. When adding
	/// items to the inventory, we will never extend the list beyond this
	/// number of entries.
	/// </summary>
	int GetActualCapacity();
}


/// <summary>
/// Represents a single ingredient used when crafting a
/// recipe. An ingredient can be an item, a currency, or anything else.
///
/// The API provides methods for getting basic item and currency ingredients,
/// so you need not use this unless you're doing something fancy.
/// </summary>
public interface IIngredient
{
	/// <summary>
	/// Whether or not this <c>IIngredient</c> supports quality control
	/// options, including using low quality first and limiting the maximum
	/// quality to use.
	/// </summary>
	bool SupportsQuality { get; }

	// Display
	/// <summary>
	/// The name of this ingredient to be displayed in the menu.
	/// </summary>
	string DisplayName { get; }

	/// <summary>
	/// The texture to use when drawing this ingredient in the menu.
	/// </summary>
	Texture2D Texture { get; }

	/// <summary>
	/// The source rectangle to use when drawing this ingredient in the menu.
	/// </summary>
	Rectangle SourceRectangle { get; }

	#region quantity

	/// <summary>
	/// The amount of this ingredient required to perform a craft.
	/// </summary>
	int Quantity { get; }

	/// <summary>
	/// Determine how much of this ingredient is available for crafting both
	/// in the player's inventory and in the other inventories.
	/// </summary>
	/// <param name="who">The farmer performing the craft</param>
	/// <param name="items">A list of all available <see cref="Item"/>s across
	/// all available <see cref="IInventory"/> instances. If you only support
	/// consuming ingredients from certain <c>IInventory</c> types, you should
	/// not use this value and instead iterate over the inventories. Please
	/// note that this does <b>not</b> include the player's inventory.</param>
	/// <param name="inventories">All the available inventories.</param>
	/// <param name="maxQuality">The maximum item quality we are allowed to
	/// count. This cannot be ignored unless <see cref="SupportsQuality"/>
	/// returns <c>false</c>.</param>
	int GetAvailableQuantity(Farmer who, IList<Item?>? items, IList<IInventory>? inventories, int maxQuality);

	#endregion quantity

	#region consumption

	/// <summary>
	/// Consume this ingredient out of the player's inventory and the other
	/// available inventories.
	/// </summary>
	/// <param name="who">The farmer performing the craft</param>
	/// <param name="inventories">All the available inventories.</param>
	/// <param name="maxQuality">The maximum item quality we are allowed to
	/// count. This cannot be ignored unless <see cref="SupportsQuality"/>
	/// returns <c>false</c>.</param>
	/// <param name="lowQualityFirst">Whether or not we should make an effort
	/// to consume lower quality ingredients before ocnsuming higher quality
	/// ingredients.</param>
	void Consume(Farmer who, IList<IInventory>? inventories, int maxQuality, bool lowQualityFirst);

	#endregion consumption
}


/// <summary>
/// Event fired by Better Crafting whenever a player performs a
/// craft, and may be fired multiple times in quick succession if a player is
/// performing bulk crafting.
/// </summary>
public interface IPerformCraftEvent
{
    /// <summary>
	/// The player performing the craft.
	/// </summary>
	Farmer Player { get; }

	/// <summary>
	/// The item being crafted, may be null depending on the recipe.
	/// </summary>
	Item? Item { get; set; }

	/// <summary>
	/// The <c>BetterCraftingPage</c> menu instance that the player is
	/// crafting from.
	/// </summary>
	IClickableMenu Menu { get; }

	/// <summary>
	/// Cancel the craft, marking it as a failure. The ingredients will not
	/// be consumed and the player will not receive the item.
	/// </summary>
	void Cancel();

	/// <summary>
	/// Complete the craft, marking it as a success. The ingredients will be
	/// consumed and the player will receive the item, if there is one.
	/// </summary>
	void Complete();
}


/// <summary>
/// Represents a single crafting recipe, though it need not
/// be associated with a vanilla <see cref="StardewValley.CraftingRecipe"/>.
/// Recipes usually produce <see cref="Item"/>s, but they are not required
/// to do so.
/// </summary>
public interface IRecipe
{

	#region identity

	/// <summary>
	/// An additional sorting value to apply to recipes in the Better Crafting
	/// menu. Applied before other forms of sorting.
	/// </summary>
	int SortValue { get; }

	/// <summary>
	/// The internal name of the recipe. For standard recipes, this matches the
	/// name of the recipe used in the player's cookingRecipes / craftingRecipes
	/// dictionaries. For non-standard recipes, this can be anything as long as
	/// it's unique, and it's recommended to prefix the names with your mod's
	/// unique ID to ensure uniqueness.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// A name displayed to the user.
	/// </summary>
	string DisplayName { get; }

	/// <summary>
	/// An optional description of the recipe displayed on its tooltip.
	/// </summary>
	string? Description { get; }

	/// <summary>
	/// Whether or not the player knows this recipe.
	/// </summary>
	/// <param name="who">The player we're asking about</param>
	bool HasRecipe(Farmer who);

	/// <summary>
	/// How many times the player has crafted this recipe. If advanced crafting
	/// information is enabled, and this value is non-zero, it will be
	/// displayed on recipe tooltips.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	int GetTimesCrafted(Farmer who);

	/// <summary>
	/// The vanilla <c>CraftingRecipe</c> instance for this recipe, if one
	/// exists. This may be used for interoperability with some other
	/// mods, but is not required.
	/// </summary>
	CraftingRecipe? CraftingRecipe { get; }

	#endregion identity

	#region display

	/// <summary>
	/// The texture to use when drawing this recipe in the menu.
	/// </summary>
	Texture2D Texture { get; }

	/// <summary>
	/// The source rectangle to use when drawing this recipe in the menu.
	/// </summary>
	Rectangle SourceRectangle { get; }

	/// <summary>
	/// How tall this recipe should appear in the menu, in grid squares.
	/// </summary>
	int GridHeight { get; }

	/// <summary>
	/// How wide this recipe should appear in the menu, in grid squares.
	/// </summary>
	int GridWidth { get; }

	#endregion display

	#region cost and quantity

	/// <summary>
	/// The quantity of item produced every time this recipe is crafted.
	/// </summary>
	int QuantityPerCraft { get; }

	/// <summary>
	/// The ingredients used by this recipe.
	/// </summary>
	IIngredient[]? Ingredients { get; }

	#endregion cost and quantity

	#region creation

	/// <summary>
	/// Whether or not the item created by this recipe is stackable, and thus
	/// eligible for bulk crafting.
	/// </summary>
	bool Stackable { get; }

	/// <summary>
	/// Check to see if the given player can currently craft this recipe. This
	/// method is suitable for checking external conditions. For example, the
	/// add-on for crafting buildings from the crafting menu uses this to check
	/// that the current <see cref="GameLocation"/> allows building.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	bool CanCraft(Farmer who);

	/// <summary>
	/// An optional, extra string to appear on item tooltips. This can be used
	/// for displaying error messages to the user, or anything else that would
	/// be relevant. For example, the add-on for crafting buildings uses this
	/// to display error messages telling users why they are unable to craft
	/// a building, if they cannot.
	/// </summary>
	/// <param name="who">The player we're asking about.</param>
	string? GetTooltipExtra(Farmer who);

	/// <summary>
	/// Create an instance of the Item this recipe crafts, if this recipe
	/// crafts an item. Returning null is perfectly acceptable.
	/// </summary>
	Item? CreateItem();

	/// <summary>
	/// This method is called when performing a craft, and can be used to
	/// perform asynchronous actions or other additional logic as required.
	/// While crafting is taking place, Better Crafting will hold locks on
	/// every inventory involved. You should ideally do as little work
	/// here as possible.
	/// </summary>
	/// <param name="evt">Details about the event, and methods for telling
	/// Better Crafting when the craft has succeeded or failed.</param>
	void PerformCraft(IPerformCraftEvent evt)
	{
		evt.Complete();
	}

	#endregion creation
}


/// <summary>
/// Used to discover crafting recipes
/// for display in the menu.
/// </summary>
public interface IRecipeProvider
{
	/// <summary>
	/// The priority of this recipe provider, sort sorting purposes.
	/// When handling CraftingRecipe instances, the first provider
	/// to return a result is used.
	/// </summary>
	int RecipePriority { get; }

	/// <summary>
	/// Get an <see cref="IRecipe"/> wrapper for a <see cref="CraftingRecipe"/>.
	/// </summary>
	/// <param name="recipe">The vanilla <c>CraftingRecipe</c> to wrap</param>
	/// <returns>An IRecipe wrapper, or null if this provider does
	/// not handle this recipe.</returns>
	IRecipe? GetRecipe(CraftingRecipe recipe);

	/// <summary>
	/// Whether or not additional recipes from this provider should be
	/// cached. If the list should be updated every time the player
	/// opens the menu, this should return false.
	/// </summary>
	bool CacheAdditionalRecipes { get; }

	/// <summary>
	/// Get any additional recipes in IRecipe form. Additional recipes
	/// are those recipes not included in the `CraftingRecipe.cookingRecipes`
	/// and `CraftingRecipe.craftingRecipes` objects.
	/// </summary>
	/// <param name="cooking">Whether we want cooking recipes or crafting recipes.</param>
	/// <returns>An enumeration of this provider's additional recipes, or null.</returns>
	IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking);
}