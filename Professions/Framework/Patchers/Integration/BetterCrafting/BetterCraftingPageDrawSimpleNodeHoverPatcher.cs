namespace DaLion.Professions.Framework.Patchers.Integration.BetterCrafting;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

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
    private static void DrawSimpleNodeHoverPrefix(ref int offsetX, ref int offsetY)
    {
        if (State.TapperCraftingRecipeBeingHovered is not null)
        {
            offsetX = State.TapperCraftingMenuCursorLockPosition.X - Game1.getOldMouseX();
            offsetY = State.TapperCraftingMenuCursorLockPosition.Y - Game1.getOldMouseY();
        }
    }

    #endregion harmony patches
}
