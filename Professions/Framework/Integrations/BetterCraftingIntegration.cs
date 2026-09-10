namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using StardewValley.Menus;
using IBetterCraftingMenu = IBetterCraftingApi.IBetterCraftingMenu;
using IIngredient = IBetterCraftingApi.IIngredient;

#endregion using directives

[ModRequirement("leclair.bettercrafting", "Better Crafting", "2.13.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration, IBetterCraftingApi>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    internal BetterCraftingIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    internal IBetterCraftingMenu? Menu => this.ModApi?.GetActiveMenu();

    internal Func<IClickableMenu, InventoryMenu> GetInventory { get; } = Reflector.GetUnboundFieldGetter<IClickableMenu, InventoryMenu>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "inventory");

    internal Func<IClickableMenu, Item?> GetItemBeingHovered { get; } = Reflector.GetUnboundFieldGetter<IClickableMenu, Item?>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "hoverItem");

    internal Func<object, object?> GetRecipeBeingHovered { get; } = Reflector.GetUnboundFieldGetter<object, object?>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "hoverRecipe");

    internal Func<object, CraftingRecipe?> GetVanillaCraftingRecipe { get; } = Reflector.GetUnboundPropertyGetter<object, CraftingRecipe?>("Leclair.Stardew.Common.Crafting.IRecipe", "CraftingRecipe");

    internal Func<object, Array> GetIngredientsFromRecipe { get; } = Reflector.GetUnboundPropertyGetter<object, Array>("Leclair.Stardew.Common.Crafting.IRecipe", "Ingredients");

    // cannot cast from API IIngredient to actual BaseIngredient
    internal Func<object, string> GetIngredientId { get; } = Reflector.GetUnboundFieldGetter<object, string>("Leclair.Stardew.BetterCrafting.Models.BaseIngredient", "ItemId");

    internal Func<string, int, float, string?, object> ConstructBaseIngredient { get; } = Reflector.GetConstructorDelegate<Func<string, int, float, string?, object>>("Leclair.Stardew.BetterCrafting.Models.BaseIngredient", 4);

    internal Action<object, object> ClearCraftCache { get; } = Reflector.GetUnboundMethodDelegate<Action<object, object>>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "ClearCraftCache");

    internal IIngredient[]? HoveredIngredientsCopy { get; set; }

    internal InventoryMenu GetInventoryMenu()
    {
        return this.Menu is null
            ? ThrowHelper.ThrowInvalidOperationException<InventoryMenu>("Better Crafting page was null!")
            : this.GetInventory(this.Menu.Menu);
    }

    internal Array GetIngredientsFromHoveredRecipe()
    {
        if (this.Menu is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<Array>("Crafting Page was null!");
        }

        var hoveredRecipe = this.GetRecipeBeingHovered(this.Menu.Menu);
        if (hoveredRecipe is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<Array>("Hovered Recipe was null!");
        }

        return this.GetIngredientsFromRecipe(hoveredRecipe);
    }

    internal object GetNewIngredient(string id, int quantity)
    {
        return this.ConstructBaseIngredient(id, quantity, -1f, null);
    }

    internal void ClearCraftCacheForHoveredRecipe()
    {
        if (this.Menu is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Better Crafting page was null!");
            return;
        }

        var hoveredRecipe = this.GetRecipeBeingHovered(this.Menu.Menu);
        if (hoveredRecipe is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Hovered Recipe was null!");
            return;
        }

        this.ClearCraftCache(this.Menu.Menu, hoveredRecipe);
    }
}
