using StardewValley;

namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;
using Common.Integrations.BetterCrafting;

#endregion using directives

internal class BetterCraftingIntegration : BaseIntegration
{
    private readonly IBetterCraftingAPI _betterCraftingApi;

    public BetterCraftingIntegration(
        IModRegistry modRegistry,
        Action<string, LogLevel> log
    ) : base("Custom Ore Nodes", "aedenthorn.CustomOreNodes", "2.1.1",
        modRegistry,
        log)
    {
        _betterCraftingApi = GetValidatedApi<IBetterCraftingAPI>();
    }

    public void Register()
    {
        AssertLoaded();

        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Amethyst Ring"));
        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Topaz Ring"));
        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Aquamarine Ring"));
        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Jade Ring"));
        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Emerald Ring"));
        _betterCraftingApi.AddRecipeProvider(new CraftingRecipe("Ruby Ring"));

    }
}