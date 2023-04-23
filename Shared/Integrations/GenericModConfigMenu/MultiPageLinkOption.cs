#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using System;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

#endregion using directives

/// <summary>Allows multi-column page link options in GMCM.</summary>
/// <typeparam name="TPage">The type of the object which represents the page.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Shockah/Stardew-Valley-Mods/blob/master/Kokoro/GMCM/MultiPageLinkOption.cs">Shockah</see>.</remarks>
public class MultiPageLinkOption<TPage>
{
    protected const string ModQualifiedName = "GenericModConfigMenu.Mod, GenericModConfigMenu";
    protected const string SpecificModConfigMenuQualifiedName = "GenericModConfigMenu.Framework.SpecificModConfigMenu, GenericModConfigMenu";
    protected const string HoverSound = "shiny4";
    protected const int RowHeight = 60;
    protected const float ColumnSpacing = 16f;
    protected const float Margin = 16f;

    protected static Func<object? /* SpecificModConfigMenu */>? getActiveSpecificModConfigMenuDelegate;
    protected static Action<object /* SpecificModConfigMenu */, string>? openPageDelegate;

    protected readonly Func<string> GetOptionName;
    protected readonly Func<TPage, string> GetPageId;
    protected readonly Func<TPage, string> GetPageName;
    protected readonly TPage[] Pages;

    protected readonly int Columns;
    protected bool? wasMouseLeftPressed;
    protected Point? lastHoverPosition;

    /// <summary>Initializes a new instance of the <see cref="MultiPageLinkOption{TOption}"/> class.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="pages">The page values.</param>
    /// <param name="getPageId">Gets the destination page ID.</param>
    /// <param name="getPageName">Gets the destination page name.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    public MultiPageLinkOption(
        Func<string> getOptionName,
        TPage[] pages,
        Func<TPage, string> getPageId,
        Func<TPage, string> getPageName,
        Func<float, int> getColumnsFromWidth)
    {
        this.GetOptionName = getOptionName;
        this.Pages = pages;
        this.GetPageId = getPageId;
        this.GetPageName = getPageName;
        this.Columns = getColumnsFromWidth(this.GetMenuSize().X);
    }

    internal void AddToMenu(IGenericModConfigMenuApi api, IManifest mod)
    {
        api.AddComplexOption(
            mod: mod,
            name: this.GetOptionName,
            draw: this.Draw,
            height: this.GetHeight,
            beforeMenuOpened: this.BeforeMenuOpened,
            beforeSave: this.BeforeSave,
            afterSave: this.AfterSave,
            beforeReset: this.BeforeReset,
            afterReset: this.AfterReset,
            beforeMenuClosed: this.BeforeMenuClosed);
    }

    protected int GetHeight()
    {
        var rows = (int)Math.Ceiling(1f * this.Pages.Length / this.Columns) + 1; // extra row, we're not rendering inline
        return rows * RowHeight;
    }

    protected Vector2 GetMenuSize()
    {
        return new Vector2(Math.Min(1200, Game1.uiViewport.Width - 200), Game1.uiViewport.Height - 128 - 116);
    }

    protected Vector2 GetMenuPosition(Vector2? size = null)
    {
        size ??= this.GetMenuSize();
        return new Vector2((Game1.uiViewport.Width - size.Value.X) / 2, (Game1.uiViewport.Height - size.Value.Y) / 2);
    }

    protected virtual void BeforeSave()
    {
    }

    protected virtual void AfterSave()
    {
    }

    protected virtual void BeforeReset()
    {
    }

    protected virtual void AfterReset()
    {
    }

    protected virtual void BeforeMenuOpened()
    {
        this.wasMouseLeftPressed = null;
        this.lastHoverPosition = null;
    }

    protected virtual void BeforeMenuClosed()
    {
    }

    protected virtual void Draw(SpriteBatch b, Vector2 basePosition)
    {
        Point? newHoverPosition = null;

        var isMouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        var didClick = isMouseLeftPressed && this.wasMouseLeftPressed == false;
        this.wasMouseLeftPressed = isMouseLeftPressed;
        var mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
        var mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

        var menuSize = this.GetMenuSize();
        var menuPosition = this.GetMenuPosition(menuSize);
        var hoveringMenu = mouseX >= menuPosition.X && mouseY >= menuPosition.Y && mouseX < menuPosition.X + menuSize.X &&
                       mouseY < menuPosition.Y + menuSize.Y;
        var columnWidth = (menuSize.X - ((this.Columns - 1) * ColumnSpacing) - Margin) / this.Columns;
        var optionSize = new Vector2(columnWidth, RowHeight);

        var row = 1;
        var column = 0;
        foreach (var page in this.Pages)
        {
            var name = this.GetPageName(page);
            var nameSize = new Vector2(SpriteText.getWidthOfString(name), SpriteText.getHeightOfString(name));
            var valuePosition = new Vector2(
                menuPosition.X + Margin + ((optionSize.X + ColumnSpacing) * column),
                basePosition.Y + (optionSize.Y * row));
            var hoveringValue = mouseX >= valuePosition.X && mouseY >= valuePosition.Y &&
                            mouseX < valuePosition.X + nameSize.X && mouseY < valuePosition.Y + nameSize.Y;
            SpriteText.drawString(
                b,
                name,
                (int)valuePosition.X,
                (int)valuePosition.Y,
                layerDepth: 1,
                color: hoveringMenu && hoveringValue ? SpriteText.color_Gray : -1);

            if (hoveringMenu && hoveringValue)
            {
                newHoverPosition = new Point(column, row);
                if (didClick)
                {
                    this.OpenPage(this.GetPageId(page));
                }
            }

            if (++column != this.Columns)
            {
                continue;
            }

            row++;
            column = 0;
        }

        if (newHoverPosition is not null && newHoverPosition != this.lastHoverPosition)
        {
            Game1.playSound(HoverSound);
        }

        this.lastHoverPosition = newHoverPosition;
    }

    [MemberNotNull(nameof(getActiveSpecificModConfigMenuDelegate), nameof(openPageDelegate))]
    private static void SetupReflectionIfNecessary()
    {
        getActiveSpecificModConfigMenuDelegate = () =>
        {
            var result = ModQualifiedName
                .ToType()
                .RequirePropertyGetter("ActiveConfigMenu")
                .Invoke(null, null);
            return result?.GetType().Name == "SpecificModConfigMenu" ? result : null;
        };

        openPageDelegate = (menu, pageId) =>
            ((Action<string>)SpecificModConfigMenuQualifiedName
                .ToType()
                .RequireField("OpenPage").GetValue(menu)!)
            .Invoke(pageId);
    }

    private void OpenPage(string pageId)
    {
        SetupReflectionIfNecessary();

        var menu = getActiveSpecificModConfigMenuDelegate();
        if (menu is null)
        {
            return;
        }

        openPageDelegate(menu, pageId);
    }
}

/// <summary>Extends the <see cref="IGenericModConfigMenuApi"/> with <see cref="AddMultiPageLinkOption{TPage}"/>.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Exception for complex GMCM option.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Exception for complex GMCM option.")]
public static class MultiPageLinkOptionExtensions
{
    /// <summary>Adds a new instance of <see cref="MultiPageLinkOption{TOption}"/> to the specified <paramref name="mod"/>'s config menu.</summary>
    /// <typeparam name="TPage">The type of the object which represents the page.</typeparam>
    /// <param name="api">The <see cref="IGenericModConfigMenuApi"/>.</param>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="pages">The page values.</param>
    /// <param name="getPageId">Gets the destination page ID.</param>
    /// <param name="getPageName">Gets the destination page name.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    public static void AddMultiPageLinkOption<TPage>(
        this IGenericModConfigMenuApi api,
        IManifest mod,
        Func<string> getOptionName,
        TPage[] pages,
        Func<TPage, string> getPageId,
        Func<TPage, string> getPageName,
        Func<float, int> getColumnsFromWidth)
    {
        new MultiPageLinkOption<TPage>(
                getOptionName,
                pages,
                getPageId,
                getPageName,
                getColumnsFromWidth)
            .AddToMenu(api, mod);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1401 // Fields should be private
