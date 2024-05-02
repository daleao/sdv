#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
namespace DaLion.Shared.Integrations.GMCM;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Extensions.Functional;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

#endregion using directives

/// <summary>Allows dynamically adding / removing items from a list in GMCM.</summary>
public class DynamicKeyValuePairListOption : DynamicListOption
{
    protected readonly Func<int, string>? GetTextBoxLabel;
    protected readonly Func<int, string>? GetTextBoxTooltip;
    protected readonly Func<IList<KeyValuePair<string, string>>> GetPairs;
    protected readonly Action<IList<KeyValuePair<string, string>>> SetPairs;

    private readonly bool _enumerateLabels;

    /// <summary>Initializes a new instance of the <see cref="DynamicKeyValuePairListOption"/> class.</summary>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getPairs">A delegate for getting the target list of pairs as a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="setPairs">A delegate for setting the target list of pairs given a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="getTextBoxLabel">Gets the <see cref="TextBox"/> label.</param>
    /// <param name="getTextBoxTooltip">Gets the <see cref="TextBox"/> tooltip.</param>
    /// <param name="enumerateLabels">Whether to enumerate the labels.</param>
    /// <param name="id">An optional id for this field.</param>
    public DynamicKeyValuePairListOption(
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<KeyValuePair<string, string>>> getPairs,
        Action<IList<KeyValuePair<string, string>>> setPairs,
        Func<int, string>? getTextBoxLabel = null,
        Func<int, string>? getTextBoxTooltip = null,
        bool enumerateLabels = false,
        string? id = null)
        : base(
            getOptionName,
            getOptionTooltip,
            () => string.Join(' ', getPairs().Select(pair => $"{pair.Key} {pair.Value}")).Split().ToList(),
            values =>
            {
                var pairs = new List<KeyValuePair<string, string>>();
                for (var i = 0; i < values.Count; i += 2)
                {
                    pairs.Add(new KeyValuePair<string, string>(values[i], values[i + 1]));
                }

                setPairs(pairs);
            },
            _ => 2,
            id)
    {
        this.GetTextBoxLabel = getTextBoxLabel;
        this.GetTextBoxTooltip = getTextBoxTooltip;
        this.GetPairs = getPairs;
        this.SetPairs = setPairs;
        this._enumerateLabels = enumerateLabels;
    }

    public static string? Tooltip { get; private protected set; }

    public override void AddToMenu(IGenericModConfigMenuApi api, IManifest mod)
    {
        if (Game1.options.gamepadControls)
        {
            api.AddTextOption(
                mod: mod,
                getValue: () => string.Join(';', this.GetPairs().Select(pair => pair.Key)),
                setValue: keys =>
                {
                    var split = keys.Split(';');
                    var currentPairs = this.GetPairs();
                    var newPairs = split.Select((t, i) => i < currentPairs.Count
                            ? new KeyValuePair<string, string>(t, currentPairs[i].Value)
                            : new KeyValuePair<string, string>(t, currentPairs[^1].Value))
                        .ToList();

                    this.SetPairs(newPairs);
                },
                name: this.GetTextBoxLabel?.Partial(0) ?? this.GetOptionName,
                tooltip: this.GetTextBoxTooltip?.Partial(0),
                fieldId: this.Id);

            api.AddTextOption(
                mod: mod,
                getValue: () => string.Join(';', this.GetPairs().Select(pair => pair.Value)),
                setValue: keys =>
                {
                    var split = keys.Split(';');
                    var currentPairs = this.GetPairs();
                    var newPairs = new List<KeyValuePair<string, string>>();
                    for (var i = 0; i < currentPairs.Count; i++)
                    {
                        if (i < split.Length)
                        {
                            newPairs.Add(new KeyValuePair<string, string>(currentPairs[i].Key, split[i]));
                        }
                    }

                    this.SetPairs(newPairs);
                },
                name: this.GetTextBoxLabel?.Partial(1) ?? this.GetOptionName,
                tooltip: this.GetTextBoxTooltip?.Partial(1),
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

    protected override void Draw(SpriteBatch b, Vector2 basePosition)
    {
        if (this.added)
        {
            for (var i = 0; i < 2; i++)
            {
                this.cachedValues.Add(string.Empty);
                var textBox = new TextBox(
                    Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
                    null,
                    Game1.smallFont,
                    Game1.textColor);
                this.textBoxes.Add(textBox);
            }

            this.added = false;
        }

        if (this.removed >= 0)
        {
            for (var i = 0; i < 2; i++)
            {
                this.cachedValues.RemoveAt(this.removed - 1);
                this.textBoxes.RemoveAt(this.removed - 1);
            }

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

        for (var i = 0; i <= this.cachedValues.Count; i++)
        {
            var row = (i / 2) + 1;
            var column = i == this.cachedValues.Count ? 1 : i % 2;
            var label = this.GetTextBoxLabel is null ? string.Empty :
                i == this.cachedValues.Count
                    ? this.GetTextBoxLabel(i - 1)
                    : this.GetTextBoxLabel(i);
            if (column == 0 && this._enumerateLabels)
            {
                label += $" #{row}";
            }

            var labelSize = new Vector2(
                SpriteText.getWidthOfString(label),
                SpriteText.getHeightOfString(label));
            var labelPosition = new Vector2(
                menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                basePosition.Y + (valueSize.Y * row));

            if (!string.IsNullOrEmpty(label) && i != this.cachedValues.Count)
            {
                Utility.drawTextWithShadow(
                    b,
                    label,
                    Game1.dialogueFont,
                    labelPosition,
                    Game1.textColor);
            }

            var boxPosition = this.columns == 1 && string.IsNullOrEmpty(label)
                ? new Vector2(
                    menuPosition.X + (menuSize.X / 2f) - 7,
                    basePosition.Y - valueSize.Y)
                : new Vector2(
                    labelPosition.X + labelSize.X + 8,
                    labelPosition.Y);

            if (i < this.cachedValues.Count)
            {
                var textBox = this.textBoxes[i];
                textBox.Update();
                this.cachedValues[i] = textBox.Text;

                textBox.X = (int)boxPosition.X;
                textBox.Y = (int)boxPosition.Y;
                textBox.Draw(b);
            }

            if (column != 1)
            {
                continue;
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
            if (!hoveringButton || !didClick)
            {
                continue;
            }

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

        if (this.GetTextBoxLabel is null || this.GetTextBoxTooltip is null)
        {
            return;
        }

        Tooltip = null;
        for (var i = 0; i < this.cachedValues.Count; i++)
        {
            var row = (i / 2) + 1;
            var column = i % 2;
            var label = this.GetTextBoxLabel(i);
            var tooltip = this.GetTextBoxTooltip(i);
            var labelPosition = new Vector2(
                menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                basePosition.Y + (valueSize.Y * row));
            var labelSize = new Vector2(
                SpriteText.getWidthOfString(label),
                SpriteText.getHeightOfString(label));
            var hoveringLabel = mouseX >= labelPosition.X && mouseY >= labelPosition.Y &&
                                mouseX < labelPosition.X + labelSize.X &&
                                mouseY < labelPosition.Y + labelSize.Y;
            if (!hoveringLabel)
            {
                continue;
            }

            Tooltip = Game1.parseText(tooltip, Game1.smallFont, Game1.dialogueWidth / 3);
            break;
        }
    }
}

/// <summary>Extends the <see cref="IGenericModConfigMenuApi"/> with <see cref="DynamicListOption"/>.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Exception for complex GMCM option.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Exception for complex GMCM option.")]
public static class DynamicKeyValuePairListOptionExtensions
{
    /// <summary>Adds a new instance of <see cref="DynamicListOption"/> to the specified <paramref name="mod"/>'s config menu.</summary>
    /// <param name="api">The <see cref="IGenericModConfigMenuApi"/>.</param>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getOptionName">Gets the option name.</param>
    /// <param name="getOptionTooltip">Gets the option tooltip.</param>
    /// <param name="getPairs">A delegate for getting the target list of pairs as a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="setPairs">A delegate for setting the target list of pairs given a list of <see cref="string"/>-<see cref="string"/> pairs.</param>
    /// <param name="getTextBoxLabel">Gets the <see cref="TextBox"/> label.</param>
    /// <param name="getTextBoxTooltip">Gets the <see cref="TextBox"/> tooltip.</param>
    /// <param name="enumerateLabels">Whether to enumerate the labels.</param>
    /// <param name="id">An optional id for this field.</param>
    public static void AddDynamicKeyValuePairListOption(
        this IGenericModConfigMenuApi api,
        IManifest mod,
        Func<string> getOptionName,
        Func<string>? getOptionTooltip,
        Func<IList<KeyValuePair<string, string>>> getPairs,
        Action<IList<KeyValuePair<string, string>>> setPairs,
        Func<int, string>? getTextBoxLabel = null,
        Func<int, string>? getTextBoxTooltip = null,
        bool enumerateLabels = false,
        string? id = null)
    {
        new DynamicKeyValuePairListOption(
                getOptionName,
                getOptionTooltip,
                getPairs,
                setPairs,
                getTextBoxLabel,
                getTextBoxTooltip,
                enumerateLabels,
                id)
            .AddToMenu(api, mod);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1401 // Fields should be private
