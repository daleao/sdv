using System.Collections.Generic;
using DaLion.Common.Extensions;

namespace DaLion.Stardew.Rings.BetterCrafting;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class BetterCraftingIntegration : BaseIntegration<IBetterCraftingAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("Better Crafting", "leclair.bettercrafting", "1.0.0", modRegistry, log)
    {
        ModEntry.Api = ModApi;
    }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();

        ModApi!.AddRecipeProvider(new RingRecipeProvider());

        var newRingRecipes = new List<string>
        {
            "Glow Ring", "Magnet Ring", "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring",
            "Ruby Ring"
        };
        ModApi!.AddRecipesToDefaultCategory(false, "combat_rings", newRingRecipes);
    }
}