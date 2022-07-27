namespace DaLion.Common.Integrations.Automate;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>An item type that can be searched and added to the player through the console.</summary>
/// <remarks>This is copied from the SMAPI source code and should be kept in sync with it.</remarks>
public enum ItemType
{
    /// <summary>The item isn't covered by one of the known types.</summary>
    Unknown,

    /// <summary>A big craftable object in <see cref="StardewValley.Game1.bigCraftablesInformation"/></summary>
    BigCraftable,

    /// <summary>A <see cref="StardewValley.Objects.Boots"/> item.</summary>
    Boots,

    /// <summary>A <see cref="StardewValley.Objects.Clothing"/> item.</summary>
    Clothing,

    /// <summary>A <see cref="StardewValley.Objects.Wallpaper"/> flooring item.</summary>
    Flooring,

    /// <summary>A <see cref="StardewValley.Objects.Furniture"/> item.</summary>
    Furniture,

    /// <summary>A <see cref="StardewValley.Objects.Hat"/> item.</summary>
    Hat,

    /// <summary>Any object in <see cref="StardewValley.Game1.objectInformation"/> (except rings).</summary>
    Object,

    /// <summary>A <see cref="StardewValley.Objects.Ring"/> item.</summary>
    Ring,

    /// <summary>A <see cref="StardewValley.Tool"/> tool.</summary>
    Tool,

    /// <summary>A <see cref="StardewValley.Objects.Wallpaper"/> wall item.</summary>
    Wallpaper,

    /// <summary>A <see cref="StardewValley.Tools.MeleeWeapon"/> or <see cref="StardewValley.Tools.Slingshot"/> item.</summary>
    Weapon
}

/// <summary>A machine processing state.</summary>
public enum MachineState
{
    /// <summary>The machine is not currently enabled (e.g. out of season or needs to be started manually).</summary>
    Disabled,

    /// <summary>The machine has no input.</summary>
    Empty,

    /// <summary>The machine is processing an input.</summary>
    Processing,

    /// <summary>The machine finished processing an input and has an output item ready.</summary>
    Done
}

/// <summary>An automatable entity, which can implement a more specific type like <see cref="IMachine"/> or <see cref="IContainer"/>. If it doesn't implement a more specific type, it's treated as a connector with no additional logic.</summary>
public interface IAutomatable
{
    /*********
    ** Accessors
    *********/
    /// <summary>The location which contains the machine.</summary>
    GameLocation Location { get; }

    /// <summary>The tile area covered by the machine.</summary>
    Rectangle TileArea { get; }
}

/// <summary>Constructs machines, containers, or connectors which can be added to a machine group.</summary>
public interface IAutomationFactory
{
    /*********
    ** Accessors
    *********/
    /// <summary>Get a machine, container, or connector instance for a given object.</summary>
    /// <param name="obj">The in-game object.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(SObject obj, GameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given terrain feature.</summary>
    /// <param name="feature">The terrain feature.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given building.</summary>
    /// <param name="building">The building.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetFor(Building building, BuildableGameLocation location, in Vector2 tile);

    /// <summary>Get a machine, container, or connector instance for a given tile position.</summary>
    /// <param name="location">The location to check.</param>
    /// <param name="tile">The tile position to check.</param>
    /// <returns>Returns an instance or <c>null</c>.</returns>
    IAutomatable? GetForTile(GameLocation location, in Vector2 tile);
}

/// <summary>An ingredient stack (or stacks) which can be consumed by a machine.</summary>
public interface IConsumable
{
    /*********
    ** Accessors
    *********/
    /// <summary>The items available to consumable.</summary>
    ITrackedStack Consumables { get; }

    /// <summary>A sample item for comparison.</summary>
    /// <remarks>This should not be a reference to the original stack.</remarks>
    Item Sample { get; }

    /// <summary>The number of items needed for the recipe.</summary>
    int CountNeeded { get; }

    /// <summary>Whether the consumables needed for this requirement are ready.</summary>
    bool IsMet { get; }


    /*********
    ** Public methods
    *********/
    /// <summary>Remove the needed number of this item from the stack.</summary>
    void Reduce();

    /// <summary>Remove the needed number of this item from the stack and return a new stack matching the count.</summary>
    Item? Take();
}

/// <summary>Provides and stores items for machines.</summary>
public interface IContainer : IAutomatable, IEnumerable<ITrackedStack>
{
    /*********
    ** Accessors
    *********/
    /// <summary>The container name (if any).</summary>
    string Name { get; }

    /// <summary>The raw mod data for the container.</summary>
    ModDataDictionary ModData { get; }

    /// <summary>Whether this is a Junimo chest, which shares a global inventory with all other Junimo chests.</summary>
    bool IsJunimoChest { get; }


    /*********
    ** Public methods
    *********/
    /// <summary>Find items in the pipe matching a predicate.</summary>
    /// <param name="predicate">Matches items that should be returned.</param>
    /// <param name="count">The number of items to find.</param>
    /// <returns>If the pipe has no matching item, returns <c>null</c>. Otherwise returns a tracked item stack, which may have less items than requested if no more were found.</returns>
    ITrackedStack? Get(Func<Item, bool> predicate, int count);

    /// <summary>Store an item stack.</summary>
    /// <param name="stack">The item stack to store.</param>
    /// <remarks>If the storage can't hold the entire stack, it should reduce the tracked stack accordingly.</remarks>
    void Store(ITrackedStack stack);

    /// <summary>Get the number of item stacks currently stored in the container.</summary>
    int GetFilled();

    /// <summary>Get the total number of item stacks that can be stored in the container.</summary>
    int GetCapacity();
}

/// <summary>A machine that accepts input and provides output.</summary>
public interface IMachine : IAutomatable
{
    /*********
    ** Accessors
    *********/
    /// <summary>A unique ID for the machine type.</summary>
    /// <remarks>This value should be identical for two machines if they have the exact same behavior and input logic. For example, if one machine in a group can't process input due to missing items, Automate will skip any other empty machines of that type in the same group since it assumes they need the same inputs.</remarks>
    string MachineTypeID { get; }


    /*********
    ** Public methods
    *********/
    /// <summary>Get the machine's processing state.</summary>
    MachineState GetState();

    /// <summary>Get the output item.</summary>
    ITrackedStack? GetOutput();

    /// <summary>Provide input to the machine.</summary>
    /// <param name="input">The available items.</param>
    /// <returns>Returns whether the machine started processing an item.</returns>
    bool SetInput(IStorage input);
}

/// <summary>Describes a generic recipe based on item input and output.</summary>
public interface IRecipe
{
    /*********
    ** Accessors
    *********/
    /// <summary>Matches items that can be used as input.</summary>
    Func<Item, bool> Input { get; }

    /// <summary>The number of inputs needed.</summary>
    int InputCount { get; }

    /// <summary>The output to generate (given an input).</summary>
    Func<Item, Item> Output { get; }

    /// <summary>The time needed to prepare an output (given an input).</summary>
    Func<Item, int> Minutes { get; }


    /*********
    ** Methods
    *********/
    /// <summary>Get whether the recipe can accept a given item as input (regardless of stack size).</summary>
    /// <param name="stack">The item to check.</param>
    bool AcceptsInput(ITrackedStack stack);
}

/// <summary>Manages access to items in the underlying containers.</summary>
public interface IStorage
{
    /*********
    ** Public methods
    *********/
    /// <summary>Get all items from the given pipes.</summary>
    IEnumerable<ITrackedStack> GetItems();

    /****
    ** TryGetIngredient
    ****/
    /// <summary>Get an ingredient needed for a recipe.</summary>
    /// <param name="predicate">Returns whether an item should be matched.</param>
    /// <param name="count">The number of items to find.</param>
    /// <param name="consumable">The matching consumables.</param>
    /// <returns>Returns whether the requirement is met.</returns>
    bool TryGetIngredient(Func<ITrackedStack, bool> predicate, int count, [NotNullWhen(true)] out IConsumable? consumable);

    /// <summary>Get an ingredient needed for a recipe.</summary>
    /// <param name="id">The item or category ID.</param>
    /// <param name="count">The number of items to find.</param>
    /// <param name="consumable">The matching consumables.</param>
    /// <param name="type">The item type to find, or <c>null</c> to match any.</param>
    /// <returns>Returns whether the requirement is met.</returns>
    bool TryGetIngredient(int id, int count, [NotNullWhen(true)] out IConsumable? consumable, ItemType? type = ItemType.Object);

    /// <summary>Get an ingredient needed for a recipe.</summary>
    /// <param name="recipes">The items to match.</param>
    /// <param name="consumable">The matching consumables.</param>
    /// <param name="recipe">The matched requisition.</param>
    /// <returns>Returns whether the requirement is met.</returns>
    bool TryGetIngredient(IRecipe[] recipes, [NotNullWhen(true)] out IConsumable? consumable, [NotNullWhen(true)] out IRecipe? recipe);

    /****
    ** TryConsume
    ****/
    /// <summary>Consume an ingredient needed for a recipe.</summary>
    /// <param name="predicate">Returns whether an item should be matched.</param>
    /// <param name="count">The number of items to find.</param>
    /// <returns>Returns whether the item was consumed.</returns>
    bool TryConsume(Func<ITrackedStack, bool> predicate, int count);

    /// <summary>Consume an ingredient needed for a recipe.</summary>
    /// <param name="itemID">The item ID.</param>
    /// <param name="count">The number of items to find.</param>
    /// <param name="type">The item type to find, or <c>null</c> to match any.</param>
    /// <returns>Returns whether the item was consumed.</returns>
    bool TryConsume(int itemID, int count, ItemType? type = ItemType.Object);

    /****
    ** TryPush
    ****/
    /// <summary>Add the given item stack to the pipes if there's space.</summary>
    /// <param name="item">The item stack to push.</param>
    bool TryPush(ITrackedStack? item);
}

/// <summary>An item stack in an input pipe which can be reduced or taken.</summary>
public interface ITrackedStack
{
    /*********
    ** Accessors
    *********/
    /// <summary>A sample item for comparison.</summary>
    /// <remarks>This should be equivalent to the underlying item (except in stack size), but *not* a reference to it.</remarks>
    Item Sample { get; }

    /// <summary>The underlying item type.</summary>
    ItemType Type { get; }

    /// <summary>The number of items in the stack.</summary>
    int Count { get; }


    /*********
    ** Public methods
    *********/
    /// <summary>Remove the specified number of this item from the stack.</summary>
    /// <param name="count">The number to consume.</param>
    void Reduce(int count);

    /// <summary>Remove the specified number of this item from the stack and return a new stack matching the count.</summary>
    /// <param name="count">The number to get.</param>
    Item? Take(int count);

    /// <summary>Ignore one item in each stack, to ensure that no stack can be fully consumed.</summary>
    void PreventEmptyStacks();
}