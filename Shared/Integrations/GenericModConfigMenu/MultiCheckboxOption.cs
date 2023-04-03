namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

/// <summary>Allows multi-column page link options in GMCM.</summary>
/// <typeparam name="TCheckbox">The type of the object from which the bool checkbox field or property can be accessed.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Shockah/Stardew-Valley-Mods/blob/master/Kokoro/GMCM/MultiPageLinkOption.cs">Shockah</see>.</remarks>
public class MultiCheckboxOption<TCheckbox>
    where TCheckbox : notnull
{
    protected const string ClickSound = "drumkit6";
    protected const int RowHeight = 60;
    protected const float ColumnSpacing = 16f;
    protected const float Margin = 16f;
    protected const float CheckboxScale = 4f;

    protected readonly Func<string> GetOptionName;
    protected readonly Func<TCheckbox, bool> GetCheckboxValue;
    protected readonly Action<TCheckbox, bool> SetCheckboxValue;
    protected readonly Func<TCheckbox, string>? GetCheckboxLabel;
    protected readonly TCheckbox[] Checkboxes;

    protected readonly Lazy<Texture2D> CheckedTexture = new(() => Game1.mouseCursors);
    protected readonly Lazy<Rectangle> CheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectChecked);
    protected readonly Lazy<Texture2D> UncheckedTexture = new(() => Game1.mouseCursors);
    protected readonly Lazy<Rectangle> UncheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectUnchecked);
    protected readonly Action<TCheckbox, bool>? OnValueUpdated;

    protected readonly int Columns;
    protected bool? WasMouseLeftPressed;
    protected IDictionary<TCheckbox, bool> UpdatedValues = new Dictionary<TCheckbox, bool>();

    /// <summary>Initializes a new instance of the <see cref="MultiCheckboxOption{TCheckbox}"/> class.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="checkboxes">The checkbox values.</param>
    /// <param name="getCheckboxValue">Gets the current state of the checkbox value.</param>
    /// <param name="setCheckboxValue">Sets the current state of the checkbox value.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="getCheckboxLabel">Gets the checkbox label.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    public MultiCheckboxOption(
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<float, int> getColumnsFromWidth,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Action<TCheckbox, bool>? onValueUpdated = null)
    {
        this.GetOptionName = getOptionName;
        this.Checkboxes = checkboxes;
        this.GetCheckboxValue = getCheckboxValue;
        this.SetCheckboxValue = setCheckboxValue;
        this.GetCheckboxLabel = getCheckboxLabel;
        this.OnValueUpdated = onValueUpdated;
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
        var rows = (int)Math.Ceiling(1f * this.Checkboxes.Length / this.Columns) + 1; // extra row, we're not rendering inline
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
        if (this.OnValueUpdated is not null)
        {
            this.UpdatedValues.ForEach(pair => this.OnValueUpdated(pair.Key, pair.Value));
        }
    }

    protected virtual void AfterSave()
    {
        this.UpdatedValues.Clear();
    }

    protected virtual void BeforeReset()
    {
    }

    protected virtual void AfterReset()
    {
    }

    protected virtual void BeforeMenuOpened()
    {
        this.WasMouseLeftPressed = null;
        this.UpdatedValues.Clear();
    }

    protected virtual void BeforeMenuClosed()
    {
    }

    protected virtual void Draw(SpriteBatch b, Vector2 basePosition)
    {
        var isMouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        var didClick = isMouseLeftPressed && this.WasMouseLeftPressed == false;
        this.WasMouseLeftPressed = isMouseLeftPressed;
        var mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
        var mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

        var menuSize = this.GetMenuSize();
        var menuPosition = this.GetMenuPosition(menuSize);
        var hoveringMenu = mouseX >= menuPosition.X && mouseY >= menuPosition.Y &&
                           mouseX < menuPosition.X + menuSize.X && mouseY < menuPosition.Y + menuSize.Y;
        var columnWidth = (menuSize.X - ((this.Columns - 1) * ColumnSpacing) - Margin) / this.Columns;
        var valueSize = new Vector2(columnWidth, RowHeight);

        var row = 1;
        var column = 0;
        foreach (var checkbox in this.Checkboxes)
        {
            var label = this.GetCheckboxLabel is null ? $"{checkbox}" : this.GetCheckboxLabel(checkbox);
            var isChecked = this.GetCheckboxValue(checkbox);
            var texture = isChecked ? this.CheckedTexture.Value : this.UncheckedTexture.Value;
            var textureSourceRect = isChecked ? this.CheckedTextureSourceRect.Value : this.UncheckedTextureSourceRect.Value;
            var boxPosition = new Vector2(
                menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                basePosition.Y + (valueSize.Y * row));

            b.Draw(
                texture,
                boxPosition + new Vector2(0, 3),
                textureSourceRect,
                Color.White,
                0,
                Vector2.Zero,
                CheckboxScale,
                SpriteEffects.None,
                0);
            Utility.drawTextWithShadow(
                b,
                label,
                Game1.dialogueFont,
                boxPosition + new Vector2((textureSourceRect.Width * CheckboxScale) + 8, 0),
                Game1.textColor);

            if (hoveringMenu & didClick)
            {
                var hoveringCheckbox = mouseX >= boxPosition.X && mouseY >= boxPosition.Y &&
                                       mouseX < boxPosition.X + (textureSourceRect.Width * CheckboxScale) &&
                                       mouseY < boxPosition.Y + (textureSourceRect.Height * CheckboxScale);
                if (hoveringCheckbox)
                {
                    var newValue = !this.GetCheckboxValue(checkbox);
                    this.SetCheckboxValue(checkbox, newValue);
                    this.UpdatedValues[checkbox] = newValue;
                    Game1.playSound(ClickSound);
                }
            }

            if (++column != this.Columns)
            {
                continue;
            }

            row++;
            column = 0;
        }
    }
}

/// <summary>Extends the <see cref="IGenericModConfigMenuApi"/> with <see cref="MultiCheckboxOption{TCheckbox}"/>.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Exception for complex GMCM option.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Exception for complex GMCM option.")]
public static class MultiCheckboxOptionExtensions
{
    /// <summary>Adds a new instance of <see cref="MultiCheckboxOption{TCheckbox}"/> to the specified <paramref name="mod"/>'s config menu.</summary>
    /// <typeparam name="TCheckbox">The type of the object which represents the page.</typeparam>
    /// <param name="api">The <see cref="IGenericModConfigMenuApi"/>.</param>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="checkboxes">The checkbox values.</param>
    /// <param name="getCheckboxValue">Gets the current state of the checkbox value.</param>
    /// <param name="setCheckboxValue">Sets the current state of the checkbox value.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="getCheckboxLabel">Gets the checkbox label.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    public static void AddMultiCheckboxOption<TCheckbox>(
        this IGenericModConfigMenuApi api,
        IManifest mod,
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<float, int> getColumnsFromWidth,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Action<TCheckbox, bool>? onValueUpdated = null)
        where TCheckbox : notnull
    {
        new MultiCheckboxOption<TCheckbox>(
                getOptionName,
                checkboxes,
                getCheckboxValue,
                setCheckboxValue,
                getColumnsFromWidth,
                getCheckboxLabel,
                onValueUpdated)
            .AddToMenu(api, mod);
    }
}
