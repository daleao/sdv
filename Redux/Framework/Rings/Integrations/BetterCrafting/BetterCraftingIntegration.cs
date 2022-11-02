namespace DaLion.Redux.Framework.Rings.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.BetterCrafting;

#endregion using directives

internal sealed class BetterCraftingIntegration : BaseIntegration<IBetterCraftingApi>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry)
        : base("Better Crafting", "leclair.bettercrafting", "1.0.0", modRegistry)
    {
    }

    /// <summary>Registers the ring recipe provider.</summary>
    public void Register()
    {
        this.AssertLoaded();
        this.ModApi!.AddRecipeProvider(new RingRecipeProvider(this.ModApi));

        var newRingRecipes = new List<string>
        {
            "Glow Ring",
            "Magnet Ring",
            "Amethyst Ring",
            "Topaz Ring",
            "Aquamarine Ring",
            "Jade Ring",
            "Emerald Ring",
            "Ruby Ring",
        };
        this.ModApi!.AddRecipesToDefaultCategory(false, "combat_rings", newRingRecipes);
    }
}
