namespace DaLion.Professions.Framework.Patchers.Integration.BetterCrafting;

#region using directives

using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("leclair.bettercrafting", minimumVersion: "2.18.0")]
internal sealed class BetterCraftingPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BetterCraftingPagePerformHoverActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
            .ToType()
            .RequireMethod("performHoverAction");
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CraftingPagePerformHoverActionPrefix()
    {
        return State.TapperCraftingRecipeBeingHovered is null;
    }

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CraftingPagePerformHoverActionPostfix(object __instance, int x, int y)
    {
        if (!Game1.player.HasProfession(Profession.Tapper, true))
        {
            return; // run original logic
        }

        var betterCraftingIntegration = BetterCraftingIntegration.Instance!;
        var recipe = betterCraftingIntegration.GetRecipeBeingHovered(__instance);
        if (recipe is null)
        {
            return;
        }

        var vanillaRecipe = betterCraftingIntegration.GetVanillaCraftingRecipe(recipe);
        var ingredients = betterCraftingIntegration.GetIngredientsFromRecipe(recipe);
        if (vanillaRecipe is null || ingredients is null || ingredients.Length == 0)
        {
            //ThrowHelper.ThrowInvalidOperationException("Better Crafting recipe data is invalid."); // apparently BCBuildings violates this without being invalid
            return;
        }

        if (Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered is null)
        {
            State.TapperCraftingRecipeBeingHovered = vanillaRecipe;
            betterCraftingIntegration.BetterCraftingPage = __instance;
            betterCraftingIntegration.HoveredIngredientsCopy = (Array)ingredients.Clone();
            State.TapperCraftingMenuCursorLockPosition = new(x, y);
            vanillaRecipe.AlterCraftingRecipeForTapper();
            return;
        }

        if (!Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered == vanillaRecipe)
        {
            State.TapperCraftingIngredientSelected = 0;
            State.TapperCraftingRecipeBeingHovered = null;
            vanillaRecipe.ResetCraftingRecipe();
            betterCraftingIntegration.BetterCraftingPage = null;
            betterCraftingIntegration.HoveredIngredientsCopy = null;
        }
    }

    #endregion harmony patches
}
