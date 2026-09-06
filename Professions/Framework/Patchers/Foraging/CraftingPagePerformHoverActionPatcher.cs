namespace DaLion.Professions.Framework.Patchers.Foraging;

using DaLion.Shared.Extensions.SMAPI;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CraftingPagePerformHoverActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CraftingPage>(nameof(CraftingPage.performHoverAction));
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
    private static void CraftingPagePerformHoverActionPostfix(CraftingPage __instance, int x, int y)
    {
        if (!Game1.player.HasProfession(Profession.Tapper, true))
        {
            return; // run original logic
        }

        if (Config.ModKey.HasKeybind(new(SButton.LeftShift)))
        {
            var ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl) && __instance.GetChildMenu() is null;
            if (ctrling)
            {
                return;
            }
        }
        else if (Config.ModKey.HasKeybind(new(SButton.LeftControl)))
        {
            var shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift) && __instance.GetChildMenu() is null;
            if (shifting)
            {
                return;
            }
        }

        var recipe = __instance.hoverRecipe;
        if (recipe is null)
        {
            return;
        }

        if (Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered is null)
        {
            State.TapperCraftingRecipeBeingHovered = recipe;
            State.OriginalRecipeList = [.. recipe.recipeList];
            State.TapperCraftingMenuCursorLockPosition = new(x, y);
            recipe.AlterCraftingRecipeForTapper();
            return;
        }

        if (!Config.ModKey.IsDown() && State.TapperCraftingRecipeBeingHovered == recipe)
        {
            State.TapperCraftingIngredientSelected = 0;
            State.TapperCraftingRecipeBeingHovered = null;
            recipe.ResetCraftingRecipe();
        }
    }

    #endregion harmony patches
}
