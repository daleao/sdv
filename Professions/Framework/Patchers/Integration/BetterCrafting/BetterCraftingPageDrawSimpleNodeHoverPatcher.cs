namespace DaLion.Professions.Framework.Patchers.Integration.BetterCrafting;

using DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ModRequirement("leclair.bettercrafting", minimumVersion: "2.18.0")]
internal sealed class BetterCraftingPageDrawSimpleNodeHoverPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BetterCraftingPageDrawSimpleNodeHoverPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BetterCraftingPageDrawSimpleNodeHoverPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "Leclair.Stardew.BetterCrafting.Menus.BetterCraftingPage"
            .ToType()
            .RequireMethod("DrawSimpleNodeHover");
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void DrawSimpleNodeHoverPrefix(SpriteBatch b, ref int offsetX, ref int offsetY)
    {
        if (State.TapperCraftingRecipeBeingHovered is null && State.LuremasterCraftingRecipeBeingHovered is null)
        {
            return;
        }

        offsetX = State.CraftingMenuCursorLockPosition.X - Game1.getOldMouseX();
        offsetY = State.CraftingMenuCursorLockPosition.Y - Game1.getOldMouseY();
        if (State.LuremasterCraftingRecipeBeingHovered is null)
        {
            return;
        }

        var inventory = BetterCraftingIntegration.Instance!
                .GetInventoryMenu()
                .inventory;
        inventory[State.LuremasterCraftingIngredientSelected]?.bounds.BorderHighlight(Color.Pink, b);
    }

    #endregion harmony patches
}
