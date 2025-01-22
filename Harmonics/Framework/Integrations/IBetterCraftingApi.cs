namespace DaLion.Harmonics.Framework.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface IBetterCraftingApi
{
    /// <summary>
    /// Add recipes to a default category. If a player has modified their
    /// category, this will not affect them.
    /// </summary>
    /// <param name="cooking">If true, we alter a cooking category. Otherwise, crafting.</param>
    /// <param name="categoryId">The ID of the category to alter.</param>
    /// <param name="recipeNames">An enumeration of recipe names for recipes to add to the category.</param>
    void AddRecipesToDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames);
}
