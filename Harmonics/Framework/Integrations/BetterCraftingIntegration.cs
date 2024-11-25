namespace DaLion.Harmonics.Framework.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("leclair.bettercrafting", "Better Crafting", "2.13.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration, IBetterCraftingApi>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    internal BetterCraftingIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        return true;
        if (!this.IsLoaded)
        {
            return false;
        }

        List<string> recipes = [];
        if (Config.CraftableGemstoneRings)
        {
            recipes.AddRange([
                "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring", "Ruby Ring",
                "Garnet Ring",
            ]);
        }

        this.ModApi.AddRecipesToDefaultCategory(false, "combat_rings", recipes);
        Log.D("Registered the Better Crafting integration.");
        return true;
    }
}
