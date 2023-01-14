namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.BetterCrafting;

#endregion using directives

[RequiresMod("leclair.bettercrafting", "Better Crafting", "1.0.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration, IBetterCraftingApi>
{
    private BetterCraftingIntegration()
        : base("leclair.bettercrafting", "Better Crafting", "1.0.0", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        this.ModApi.AddRecipeProvider(new RingRecipeProvider(this.ModApi));

        var recipes = new List<string>();

        if (RingsModule.Config.CraftableGlowAndMagnetRings)
        {
            recipes.AddRange(new[] { "Glow Ring", "Magnet Ring", });
        }

        if (RingsModule.Config.CraftableGemRings)
        {
            recipes.AddRange(new[]
            {
                "Amethyst Ring", "Topaz Ring", "Aquamarine Ring", "Jade Ring", "Emerald Ring", "Ruby Ring",
                "Garnet Ring",
            });
        }

        this.ModApi.AddRecipesToDefaultCategory(false, "combat_rings", recipes);
        return true;
    }
}
