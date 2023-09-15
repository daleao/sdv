namespace DaLion.Overhaul.Modules.Tweex.Integrations.BetterCrafting;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.BetterCrafting;

#endregion using directives

[ModRequirement("leclair.bettercrafting", "Better Crafting", "1.0.0")]
internal sealed class BetterCraftingIntegration : ModIntegration<BetterCraftingIntegration, IBetterCraftingApi>
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingIntegration"/> class.</summary>
    internal BetterCraftingIntegration()
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
        if (TweexModule.Config.ImmersiveGlowstoneProgression)
        {
            recipes.AddRange(new[] { "Small Glow Ring", "Small Magnet Ring", "Glow Ring", "Magnet Ring", });
        }

        this.ModApi.AddRecipesToDefaultCategory(false, "combat_rings", recipes);

        Log.D("[TWX]: Registered the Better Crafting integration.");
        return true;
    }
}
