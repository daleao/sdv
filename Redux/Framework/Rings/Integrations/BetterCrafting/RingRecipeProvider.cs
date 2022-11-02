namespace DaLion.Redux.Framework.Rings.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Integrations.BetterCrafting;
using StardewValley.Objects;

#endregion using directives

/// <summary>Provides <see cref="IRecipe"/> wrappers for Ring recipes with consume other Rings.</summary>
internal sealed class RingRecipeProvider : IRecipeProvider
{
    private readonly IBetterCraftingApi _api;

    /// <summary>Initializes a new instance of the <see cref="RingRecipeProvider"/> class.</summary>
    /// <param name="api">The Better Crafting API.</param>
    public RingRecipeProvider(IBetterCraftingApi api)
    {
        this._api = api;
    }

    /// <summary>
    ///     Gets the priority of this recipe provider, for sorting purposes.
    ///     When handling CraftingRecipe instances, the first provider
    ///     to return a result is used.
    /// </summary>
    public int RecipePriority => int.MaxValue;

    /// <summary>
    ///     Gets a value indicating whether whether or not additional recipes from this provider should be
    ///     cached. If the list should be updated every time the player
    ///     opens the menu, this should return false.
    /// </summary>
    public bool CacheAdditionalRecipes => false;

    /// <summary>Get an <see cref="IRecipe"/> wrapper for a <see cref="CraftingRecipe"/>.</summary>
    /// <param name="recipe">The vanilla <c>CraftingRecipe</c> to wrap.</param>
    /// <returns>A <see cref="IRecipe"/> wrapper, or null if this provider does not handle this recipe.</returns>
    public IRecipe? GetRecipe(CraftingRecipe recipe)
    {
        // make a Ring instance to get its DisplayName.
        switch (recipe.name)
        {
            case "Glow Ring":
                var smallGlowRing = new Ring(Constants.SmallGlowRingIndex);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return this._api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    this._api.CreateBaseIngredient(Constants.SunEssenceIndex, 10),

                    // Ring ingredient
                    this._api.CreateMatcherIngredient(
                        item => item is Ring { ParentSheetIndex: Constants.SmallGlowRingIndex },
                        2,
                        smallGlowRing.DisplayName,
                        Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            Constants.SmallGlowRingIndex,
                            16,
                            16)),
                });

            case "Magnet Ring":
                var smallMagnetRing = new Ring(Constants.SmallMagnetRingIndex);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return this._api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    this._api.CreateBaseIngredient(Constants.VoidEssenceIndex, 10),

                    // Ring ingredient
                    this._api.CreateMatcherIngredient(
                        item => item is Ring { ParentSheetIndex: Constants.SmallMagnetRingIndex },
                        2,
                        smallMagnetRing.DisplayName,
                        Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            Constants.SmallMagnetRingIndex,
                            16,
                            16)),
                });

            case "Glowstone Ring":
                var glowRing = new Ring(Constants.GlowRingIndex);
                var magnetRIng = new Ring(Constants.MagnetRingIndex);

                // return a recipe that uses data from the vanilla crafting recipe, but with custom ingredient handling
                return this._api.CreateRecipeWithIngredients(recipe, new[]
                {
                    // normal ingredients
                    this._api.CreateBaseIngredient(Constants.SunEssenceIndex, 20),
                    this._api.CreateBaseIngredient(Constants.VoidEssenceIndex, 20),

                    // Ring ingredient
                    this._api.CreateMatcherIngredient(
                        item => item is Ring { ParentSheetIndex: Constants.GlowRingIndex },
                        1,
                        glowRing.DisplayName,
                        Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            Constants.GlowRingIndex,
                            16,
                            16)),
                    this._api.CreateMatcherIngredient(
                        item => item is Ring { ParentSheetIndex: Constants.MagnetRingIndex },
                        1,
                        magnetRIng.DisplayName,
                        Game1.objectSpriteSheet,
                        Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            Constants.MagnetRingIndex,
                            16,
                            16)),
                });

            default:
                return null;
        }
    }

    /// <summary>
    ///     Get any additional recipes in <see cref="IRecipe"/> form. Additional recipes are
    ///     those recipes not included in either <see cref="CraftingRecipe.cookingRecipes"/>
    ///     or <see cref="CraftingRecipe.craftingRecipes"/>.
    /// </summary>
    /// <param name="cooking">Whether we want cooking recipes or crafting recipes.</param>
    /// <returns>An enumeration of this provider's additional recipes, or null.</returns>
    public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking)
    {
        return null;
    }
}
