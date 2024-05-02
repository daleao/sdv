#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

/// <summary>Allows dynamically adding / removing items from a list in GMCM.</summary>
public class DynamicListOption
{
    protected const string ClickSound = "drumkit6";
    protected const int RowHeight = 60;
    protected const float ColumnSpacing = 16f;
    protected const float Margin = 16f;
    protected const float ButtonScale = 4f;

    protected readonly Func<string> GetOptionName;
    protected readonly Func<string>? GetOptionTooltip;
    protected readonly Func<IList<string>> GetValues;
    protected readonly Action<IList<string>> SetValues;
    protected readonly Func<float, int> GetColumnsFromWidth;

    protected readonly Lazy<Texture2D> TextBoxTexture =
        new(() => Game1.content.Load<Texture2D>("LooseSprites\\textBox"));

    protected readonly Lazy<Texture2D> PlusMinusButtonTexture = new(() => Game1.mouseCursors);
    protected readonly Lazy<Rectangle> MinusButtonSourceRect = new(() => new Rectangle(177, 345, 7, 8));
    protected readonly Lazy<Rectangle> PlusButtonSourceRect = new(() => new Rectangle(184, 345, 7, 8));
    protected readonly string? Id;

    protected IList<TextBox> textBoxes = new List<TextBox>();
    protected IList<string> cachedValues;
    protected bool? wasMouseLeftPressed;
    protected bool added = false;
    protected int removed = -1;
    protected int columns;

    private bool _isOpen;

    /// <summary>Initializes a new instance of the <see cref="DynamicListOption"/> class.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getValues">A delegate for getting the target list as a list of string items.</param>
    /// <param name="setValues">A delegate for setting the target list given a list of string items.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    public DynamicListOption(
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<string>> getValues,
        Action<IList<string>> setValues,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
    {
        this.GetOptionName = getOptionName;
        this.GetOptionTooltip = getOptionTooltip;
        this.GetValues = getValues;
        this.SetValues = setValues;
        this.GetColumnsFromWidth = getColumnsFromWidth ?? (width => (int)(width / 300));
        this.Id = id;
        this.cachedValues = getValues();
    }

    internal virtual void AddToMenu(IGenericModConfigMenuApi api, IManifest mod)
    {
        if (Game1.options.gamepadControls)
        {
            api.AddTextOption(
                mod: mod,
                getValue: () => string.Join(';', this.GetValues()),
                setValue: value => this.SetValues(value.Split(';').ToList()),
                name: this.GetOptionName,
                tooltip: this.GetOptionTooltip,
                fieldId: this.Id);

            return;
        }

        api.AddComplexOption(
            mod: mod,
            name: this.GetOptionName,
            draw: this.Draw,
            tooltip: this.GetOptionTooltip,
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
        var rows = (int)Math.Ceiling(1f * (this.cachedValues.Count + 1) / this.columns) + 1; // extra row, we're not rendering inline
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
        if (this._isOpen)
        {
            this.SetValues(this.cachedValues.Where(value => !string.IsNullOrEmpty(value)).ToList());
        }
    }

    protected virtual void AfterSave()
    {
    }

    protected virtual void BeforeReset()
    {
    }

    protected virtual void AfterReset()
    {
        this.cachedValues = this.GetValues();
        this.textBoxes = new List<TextBox>();
        for (var i = 0; i < this.cachedValues.Count; i++)
        {
            var textBox = new TextBox(
                Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
                null,
                Game1.smallFont,
                Game1.textColor) { limitWidth = false };
            this.textBoxes.Add(textBox);
        }
    }

    protected virtual void BeforeMenuOpened()
    {
        this.wasMouseLeftPressed = null;
        this.cachedValues = this.GetValues();
        this.textBoxes = new List<TextBox>();
        for (var i = 0; i < this.cachedValues.Count; i++)
        {
            var textBox = new TextBox(
                Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
                null,
                Game1.smallFont,
                Game1.textColor) { limitWidth = false, Text = this.cachedValues[i] };
            this.textBoxes.Add(textBox);
        }

        this._isOpen = true;
    }

    protected virtual void BeforeMenuClosed()
    {
        this.cachedValues.Clear();
        this.textBoxes.Clear();
        this._isOpen = false;
    }

    protected virtual void Draw(SpriteBatch b, Vector2 basePosition)
    {
        if (this.added)
        {
            this.cachedValues.Add(string.Empty);
            var textBox = new TextBox(
                Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
                null,
                Game1.smallFont,
                Game1.textColor) { limitWidth = false };
            this.textBoxes.Add(textBox);
            this.added = false;
        }

        if (this.removed >= 0)
        {
            this.cachedValues.RemoveAt(this.removed);
            this.textBoxes.RemoveAt(this.removed);
            this.removed = -1;
        }

        var isMouseLeftPressed = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
        var didClick = isMouseLeftPressed && this.wasMouseLeftPressed == false;
        this.wasMouseLeftPressed = isMouseLeftPressed;
        var mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
        var mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

        var menuSize = this.GetMenuSize();
        var menuPosition = this.GetMenuPosition(menuSize);

        this.columns = this.GetColumnsFromWidth(menuSize.X);
        var columnWidth = (menuSize.X - ((this.columns - 1) * ColumnSpacing) - Margin) / this.columns;
        var valueSize = new Vector2(columnWidth, RowHeight);

        var row = 1;
        var column = 0;
        for (var i = 0; i <= this.cachedValues.Count; i++)
        {
            var boxPosition = this.columns == 1
                ? new Vector2(
                    menuPosition.X + (menuSize.X / 2f) - 7,
                    basePosition.Y - valueSize.Y)
                : new Vector2(
                    menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                    basePosition.Y + (valueSize.Y * row));

            if (i < this.cachedValues.Count)
            {
                var textBox = this.textBoxes[i];
                textBox.Update();
                this.cachedValues[i] = textBox.Text;

                textBox.X = (int)boxPosition.X;
                textBox.Y = (int)boxPosition.Y;
                textBox.Draw(b);
            }

            var buttonPosition = boxPosition + new Vector2(this.TextBoxTexture.Value.Width + 8, 6f);
            b.Draw(
                this.PlusMinusButtonTexture.Value,
                buttonPosition,
                i == this.cachedValues.Count ? this.PlusButtonSourceRect.Value : this.MinusButtonSourceRect.Value,
                Color.White,
                0,
                Vector2.Zero,
                ButtonScale,
                SpriteEffects.None,
                0);

            var hoveringButton = mouseX >= buttonPosition.X && mouseY >= buttonPosition.Y &&
                               mouseX < buttonPosition.X + 28 && mouseY < buttonPosition.Y + 32;
            if (hoveringButton && didClick)
            {
                Game1.playSound(ClickSound);
                if (i == this.cachedValues.Count)
                {
                    this.added = true;
                }
                else
                {
                    this.removed = i;
                }

                Game1.playSound(ClickSound);
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

/// <summary>Extends the <see cref="IGenericModConfigMenuApi"/> with <see cref="DynamicListOption"/>.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Exception for complex GMCM option.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Exception for complex GMCM option.")]
public static class DynamicListOptionExtensions
{
    /// <summary>Adds a new instance of <see cref="DynamicListOption"/> to the specified <paramref name="mod"/>'s config menu.</summary>
    /// <param name="api">The <see cref="IGenericModConfigMenuApi"/>.</param>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getValues">A delegate for getting the target list as a list of string items.</param>
    /// <param name="setValues">A delegate for setting the target list given a list of string items.</param>
    /// <param name="getColumnsFromWidth">Gets the number of columns based on the width of the menu.</param>
    /// <param name="id">An optional id for this field.</param>
    public static void AddDynamicListOption(
        this IGenericModConfigMenuApi api,
        IManifest mod,
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<string>> getValues,
        Action<IList<string>> setValues,
        Func<float, int>? getColumnsFromWidth = null,
        string? id = null)
    {
        new DynamicListOption(
                getOptionName,
                getOptionTooltip,
                getValues,
                setValues,
                getColumnsFromWidth,
                id)
            .AddToMenu(api, mod);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1401 // Fields should be private
