#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
namespace DaLion.Shared.Integrations.GMCM;

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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

#endregion using directives

/// <summary>Allows multi-column page link options in GMCM.</summary>
/// <typeparam name="TCheckbox">The type of the object from which the bool checkbox field or property can be accessed.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Shockah/Stardew-Valley-Mods/blob/master/Kokoro/GMCM/MultiPageLinkOption.cs">Shockah</see>.</remarks>
public class MultiCheckboxOption<TCheckbox>
    where TCheckbox : notnull
{
    protected const string ClickSound = "drumkit6";
    protected const float ColumnSpacing = 16f;
    protected const float Margin = 16f;
    protected const float CheckboxScale = 4f;
    protected const int RowHeight = 60;
    protected const int CheckboxTextureWidth = 9;

    protected readonly Func<string> GetOptionName;
    protected readonly Func<TCheckbox, bool> GetCheckboxValue;
    protected readonly Action<TCheckbox, bool> SetCheckboxValue;
    protected readonly Func<TCheckbox, string>? GetCheckboxLabel;
    protected readonly Func<TCheckbox, string>? GetCheckboxTooltip;
    protected readonly Action<TCheckbox, bool>? OnValueUpdated;
    protected readonly Func<float, int> GetColumnsFromWidth;
    protected readonly TCheckbox[] Checkboxes;

    protected readonly Lazy<Texture2D> CheckedTexture = new(() => Game1.mouseCursors);
    protected readonly Lazy<Rectangle> CheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectChecked);
    protected readonly Lazy<Texture2D> UncheckedTexture = new(() => Game1.mouseCursors);
    protected readonly Lazy<Rectangle> UncheckedTextureSourceRect = new(() => OptionsCheckbox.sourceRectUnchecked);

    protected readonly IDictionary<TCheckbox, bool> updatedValues = new Dictionary<TCheckbox, bool>();
    protected readonly string? Id;

    protected bool? wasMouseLeftPressed;
    protected int columns;

    /// <summary>Initializes a new instance of the <see cref="MultiCheckboxOption{TCheckbox}"/> class.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="checkboxes">The checkbox values.</param>
    /// <param name="getCheckboxValue">Gets the current state of the checkbox value.</param>
    /// <param name="setCheckboxValue">Sets the current state of the checkbox value.</param>
    /// <param name="getCheckboxLabel">Gets the checkbox label.</param>
    /// <param name="getCheckboxTooltip">Gets the checkbox tooltip.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    public MultiCheckboxOption(
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Func<TCheckbox, string>? getCheckboxTooltip = null,
        Action<TCheckbox, bool>? onValueUpdated = null,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
    {
        this.GetOptionName = getOptionName;
        this.Checkboxes = checkboxes;
        this.GetCheckboxValue = getCheckboxValue;
        this.SetCheckboxValue = setCheckboxValue;
        this.GetCheckboxLabel = getCheckboxLabel;
        this.GetCheckboxTooltip = getCheckboxTooltip;
        this.OnValueUpdated = onValueUpdated;
        this.GetColumnsFromWidth = getColumnsFromWidth ?? (_ => 2);
        this.Id = id;
    }

    internal static string? Tooltip { get; private protected set; }

    internal void AddToMenu(IGenericModConfigMenuApi api, IManifest mod)
    {
        if (Game1.options.gamepadControls)
        {
            api.AddSectionTitle(mod, this.GetOptionName);
            for (var i = 0; i < this.Checkboxes.Length; i++)
            {
                var checkbox = this.Checkboxes[i];
                api.AddBoolOption(
                    mod: mod,
                    getValue: () => this.GetCheckboxValue(checkbox),
                    setValue: value => this.SetCheckboxValue(checkbox, value),
                    name: () => this.GetCheckboxLabel is null ? $"{checkbox}" : this.GetCheckboxLabel(checkbox),
                    tooltip: this.GetCheckboxTooltip is null ? null : () => this.GetCheckboxTooltip(checkbox),
                    fieldId: this.Id);
            }

            return;
        }

        api.AddComplexOption(
            mod: mod,
            name: this.GetOptionName,
            draw: this.Draw,
            beforeMenuOpened: this.BeforeMenuOpened,
            beforeSave: this.BeforeSave,
            afterSave: this.AfterSave,
            beforeReset: this.BeforeReset,
            afterReset: this.AfterReset,
            beforeMenuClosed: this.BeforeMenuClosed,
            height: this.GetHeight,
            fieldId: this.Id);
    }

    protected int GetHeight()
    {
        var rows = (int)Math.Ceiling(1f * this.Checkboxes.Length / this.columns) + 1; // extra row, we're not rendering inline
        return rows * RowHeight;
    }

    protected Vector2 GetMenuSize()
    {
        return new Vector2(Math.Min(1200, Game1.uiViewport.Width - 200), Game1.uiViewport.Height - 244);
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
            this.updatedValues.ForEach(pair => this.OnValueUpdated(pair.Key, pair.Value));
        }
    }

    protected virtual void AfterSave()
    {
        this.updatedValues.Clear();
    }

    protected virtual void BeforeReset()
    {
    }

    protected virtual void AfterReset()
    {
        this.updatedValues.Clear();
    }

    protected virtual void BeforeMenuOpened()
    {
        this.wasMouseLeftPressed = null;
        this.updatedValues.Clear();
    }

    protected virtual void BeforeMenuClosed()
    {
        Tooltip = null;
    }

    protected virtual void Draw(SpriteBatch b, Vector2 basePosition)
    {
        var isMouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        var didClick = isMouseLeftPressed && this.wasMouseLeftPressed == false;
        this.wasMouseLeftPressed = isMouseLeftPressed;
        var mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
        var mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

        var menuSize = this.GetMenuSize();
        var menuPosition = this.GetMenuPosition(menuSize);

        this.columns = this.GetColumnsFromWidth(menuSize.X);
        var hoveringMenu = mouseX >= menuPosition.X && mouseY >= menuPosition.Y &&
                           mouseX < menuPosition.X + menuSize.X && mouseY < menuPosition.Y + menuSize.Y;
        var columnWidth = (menuSize.X - ((this.columns - 1) * ColumnSpacing) - Margin) / this.columns;
        var valueSize = new Vector2(columnWidth, RowHeight);

        var row = 1;
        var column = 0;
        for (var i = 0; i < this.Checkboxes.Length; i++)
        {
            var checkbox = this.Checkboxes[i];
            var label = this.GetCheckboxLabel is null ? $"{checkbox}" : this.GetCheckboxLabel(checkbox);
            var isChecked = this.GetCheckboxValue(checkbox);
            var texture = isChecked ? this.CheckedTexture.Value : this.UncheckedTexture.Value;
            var textureSourceRect =
                isChecked ? this.CheckedTextureSourceRect.Value : this.UncheckedTextureSourceRect.Value;
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

            if (hoveringMenu && didClick)
            {
                var hoveringCheckbox = mouseX >= boxPosition.X && mouseY >= boxPosition.Y &&
                                       mouseX < boxPosition.X + (textureSourceRect.Width * CheckboxScale) &&
                                       mouseY < boxPosition.Y + (textureSourceRect.Height * CheckboxScale);
                if (hoveringCheckbox)
                {
                    var newValue = !this.GetCheckboxValue(checkbox);
                    this.SetCheckboxValue(checkbox, newValue);
                    this.updatedValues[checkbox] = newValue;
                    Game1.playSound(ClickSound);
                }
            }

            if (++column != this.columns)
            {
                continue;
            }

            row++;
            column = 0;
        }

        if (this.GetCheckboxTooltip is null)
        {
            return;
        }

        // draw tooltips
        row = 1;
        column = 0;
        Tooltip = null;
        for (var i = 0; i < this.Checkboxes.Length; i++)
        {
            var checkbox = this.Checkboxes[i];
            var label = this.GetCheckboxLabel is null ? $"{checkbox}" : this.GetCheckboxLabel(checkbox);
            var tooltip = this.GetCheckboxTooltip(checkbox);
            var boxPosition = new Vector2(
                menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                basePosition.Y + (valueSize.Y * row));
            var labelPosition = boxPosition + new Vector2((CheckboxTextureWidth * CheckboxScale) + 8, 0);
            var labelSize = new Vector2(
                SpriteText.getWidthOfString(label),
                SpriteText.getHeightOfString(label));
            var hoveringLabel = mouseX >= labelPosition.X && mouseY >= labelPosition.Y &&
                                mouseX < labelPosition.X + labelSize.X &&
                                mouseY < labelPosition.Y + labelSize.Y;
            if (hoveringLabel)
            {
                Tooltip = Game1.parseText(tooltip, Game1.smallFont, Game1.dialogueWidth / 3);
                break;
            }

            if (++column != this.columns)
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
    /// <param name="getCheckboxLabel">Gets the checkbox label.</param>
    /// <param name="getCheckboxTooltip">Gets the checkbox tooltip.</param>
    /// <param name="onValueUpdated">A delegate to be called after values are changed.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    public static void AddMultiCheckboxOption<TCheckbox>(
        this IGenericModConfigMenuApi api,
        IManifest mod,
        Func<string> getOptionName,
        TCheckbox[] checkboxes,
        Func<TCheckbox, bool> getCheckboxValue,
        Action<TCheckbox, bool> setCheckboxValue,
        Func<TCheckbox, string>? getCheckboxLabel = null,
        Func<TCheckbox, string>? getCheckboxTooltip = null,
        Action<TCheckbox, bool>? onValueUpdated = null,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
        where TCheckbox : notnull
    {
        new MultiCheckboxOption<TCheckbox>(
                getOptionName,
                checkboxes,
                getCheckboxValue,
                setCheckboxValue,
                getCheckboxLabel,
                getCheckboxTooltip,
                onValueUpdated,
                getColumnsFromWidth,
                id)
            .AddToMenu(api, mod);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1401 // Fields should be private
