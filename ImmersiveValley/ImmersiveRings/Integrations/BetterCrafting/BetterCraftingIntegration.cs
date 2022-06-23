namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class BetterCraftingIntegration : BaseIntegration<IBetterCraftingAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry)
        : base("Better Crafting", "leclair.bettercrafting", "1.0.0", modRegistry)
    {
    }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();
        ModApi!.AddRecipeProvider(new RingRecipeProvider(ModApi));
        
        var newRingRecipes = new List<string>
        {
            "Glow Ring", "Magnet Ring", "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring",
            "Ruby Ring"
        };
        ModApi!.AddRecipesToDefaultCategory(false, "combat_rings", newRingRecipes);
    }
}