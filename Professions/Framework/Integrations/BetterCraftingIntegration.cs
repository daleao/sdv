namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("leclair.bettercrafting", "Better Crafting", "2.13.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    internal BetterCraftingIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    internal object? BetterCraftingPage { get; set; }

    internal Func<object, object?> GetRecipeBeingHovered { get; } = Reflector.GetUnboundFieldGetter<object, object?>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "hoverRecipe");

    internal Func<object, CraftingRecipe?> GetVanillaCraftingRecipe { get; } = Reflector.GetUnboundPropertyGetter<object, CraftingRecipe?>("Leclair.Stardew.Common.Crafting.IRecipe", "CraftingRecipe");

    internal Func<object, Array> GetIngredientsFromRecipe { get; } = Reflector.GetUnboundPropertyGetter<object, Array>("Leclair.Stardew.Common.Crafting.IRecipe", "Ingredients");

    internal Func<object, string> GetIngredientId { get; } = Reflector.GetUnboundFieldGetter<object, string>("Leclair.Stardew.BetterCrafting.Models.BaseIngredient", "ItemId");

    internal Func<object, int> GetIngredientQuantity { get; } = Reflector.GetUnboundPropertyGetter<object, int>("Leclair.Stardew.BetterCrafting.Models.BaseIngredient", "Quantity");

    internal Func<string, int, float, string?, object> ConstructIngredient { get; } = Reflector.GetConstructorDelegate<Func<string, int, float, string?, object>>("Leclair.Stardew.BetterCrafting.Models.BaseIngredient", 4);

    internal Action<object, object> ClearCraftCache { get; } = Reflector.GetUnboundMethodDelegate<Action<object, object>>("Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage", "ClearCraftCache");

    internal Array? HoveredIngredientsCopy { get; set; }

    internal object ConstructDefaultIngredient(string id, int quantity)
    {
        return this.ConstructIngredient(id, quantity, -1f, null);
    }

    internal Array GetIngredientsFromHoveredRecipe()
    {
        if (this.BetterCraftingPage is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<Array>("Crafting Page is null!");
        }

        var hoveredRecipe = this.GetRecipeBeingHovered(this.BetterCraftingPage);
        if (hoveredRecipe is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<Array>("Hovered Recipe is null!");
        }

        return this.GetIngredientsFromRecipe(hoveredRecipe);
    }

    internal void ClearCraftCacheForHoveredRecipe()
    {
        if (this.BetterCraftingPage is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Crafting Page is null!");
            return;
        }

        var hoveredRecipe = this.GetRecipeBeingHovered(this.BetterCraftingPage);
        if (hoveredRecipe is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Hovered Recipe is null!");
            return;
        }

        this.ClearCraftCache(this.BetterCraftingPage, hoveredRecipe);
    }
}
