namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using Common.Extensions.Reflection;
using StardewValley.Menus;
using System;

#endregion using directives

internal static class SpaceCoreUtils
{
    internal static Lazy<Func<IClickableMenu, ClickableTextureComponent>> GetNewForgeMenuLeftIngredientSpot = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireField("leftIngredientSpot")
            .CompileUnboundFieldGetterDelegate<IClickableMenu, ClickableTextureComponent>());

    internal static Lazy<Func<IClickableMenu, int, int>> GetNewForgeMenuForgeCostAtLevel = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("GetForgeCostAtLevel")
            .CompileUnboundDelegate<Func<IClickableMenu, int, int>>());

    internal static Lazy<Func<IClickableMenu, Item, Item, int>> GetNewForgeMenuForgeCost = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("GetForgeCost")
            .CompileUnboundDelegate<Func<IClickableMenu, Item, Item, int>>());

    internal static Lazy<Action<IClickableMenu, Item>> SetNewForgeMenuHeldItem = new(() =>
        "SpaceCore.Interface.NewForgeMenu".ToType().RequireField("heldItem")
            .CompileUnboundFieldSetterDelegate<IClickableMenu, Item>());
}