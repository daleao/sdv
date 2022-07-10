using StardewValley;
using StardewValley.Menus;
using System;

namespace DaLion.Stardew.Arsenal.Framework;

internal static class SpaceCoreUtils
{
    internal static Func<IClickableMenu, ClickableTextureComponent>? GetNewForgeMenuLeftIngredientSpot { get; set; }
    internal static Func<IClickableMenu, int, int>? GetNewForgeMenuForgeCostAtLevel { get; set; }
    internal static Func<IClickableMenu, Item, Item, int>? GetNewForgeMenuForgeCost { get; set; }
    internal static Action<IClickableMenu, Item>? SetNewForgeMenuHeldItem { get; set; }
}