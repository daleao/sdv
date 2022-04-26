namespace DaLion.Common.Integrations.BetterCrafting;

public interface IBetterCraftingAPI
{
    /// <summary>
    /// Register a recipe provider with Better Crafting. Calling this
    /// will also invalidate the recipe cache.
    ///
    /// If the recipe provider was already registered, this does nothing.
    /// </summary>
    /// <param name="provider">The recipe provider to add</param>
    void AddRecipeProvider(object provider);

    /// <summary>
    /// Unregister a recipe provider. Calling this will also invalidate
    /// the recipe cache.
    ///
    /// If the recipe provider was not registered, this does nothing.
    /// </summary>
    /// <param name="provider">The recipe provider to remove</param>
    void RemoveRecipeProvider(object provider);
}