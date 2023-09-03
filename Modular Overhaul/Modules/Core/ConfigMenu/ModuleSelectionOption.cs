namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System;
using System.Linq;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

#endregion using directives

internal class ModuleSelectionOption : MultiCheckboxOption<OverhaulModule>
{
    private readonly Func<OverhaulModule, string?> _getCheckboxTooltip;
    private readonly Action _reloadMenu;

    /// <summary>Initializes a new instance of the <see cref="ModuleSelectionOption"/> class.</summary>
    /// <param name="reloadMenu">A delegate to the parent menu's Reload method, to be invoked if any modules are toggled.</param>
    internal ModuleSelectionOption(Action reloadMenu)
    : base(
        getOptionName: I18n.Gmcm_Core_Available,
        checkboxes: EnumerateModules().Skip(1).ToArray(),
        getCheckboxValue: module => module._ShouldEnable,
        setCheckboxValue: (module, value) => module._ShouldEnable = value,
        getColumnsFromWidth: _ => 2,
        getCheckboxLabel: module => module.DisplayName,
        onValueUpdated: (module, newValue) =>
        {
            if (newValue)
            {
                module.Activate(ModHelper);
                module.RegisterIntegrations();
            }
            else
            {
                module.Deactivate();
            }
        })
    {
        this._getCheckboxTooltip = module => module is CoreModule ? null : module.Description;
        this._reloadMenu = reloadMenu;
    }

    /// <summary>Gets the tooltip for the currently hovered checkbox label, if any.</summary>
    internal static string? Tooltip { get; private set; }

    /// <inheritdoc />
    protected override void AfterSave()
    {
        if (this.updatedValues.Any())
        {
            this._reloadMenu();
        }

        this.updatedValues.Clear();
    }

    /// <inheritdoc />
    protected override void BeforeMenuClosed()
    {
        Tooltip = null;
    }

    /// <inheritdoc />
    protected override void Draw(SpriteBatch b, Vector2 basePosition)
    {
        base.Draw(b, basePosition);

        var mouseX = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseX() : Game1.getOldMouseX();
        var mouseY = Constants.TargetPlatform == GamePlatform.Android ? Game1.getMouseY() : Game1.getOldMouseY();

        var menuSize = this.GetMenuSize();
        var menuPosition = this.GetMenuPosition(menuSize);
        var columnWidth = (menuSize.X - ((this.Columns - 1) * ColumnSpacing) - Margin) / this.Columns;
        var valueSize = new Vector2(columnWidth, RowHeight);

        var row = 1;
        var column = 0;
        Tooltip = null;
        foreach (var checkbox in this.Checkboxes)
        {
            var label = this.GetCheckboxLabel is null ? $"{checkbox}" : this.GetCheckboxLabel(checkbox);
            var tooltip = this._getCheckboxTooltip(checkbox);
            var isChecked = this.GetCheckboxValue(checkbox);
            var textureSourceRect = isChecked ? this.CheckedTextureSourceRect.Value : this.UncheckedTextureSourceRect.Value;
            var boxPosition = new Vector2(
                menuPosition.X + Margin + ((valueSize.X + ColumnSpacing) * column),
                basePosition.Y + (valueSize.Y * row));
            var labelPosition = boxPosition + new Vector2((textureSourceRect.Width * CheckboxScale) + 8, 0);
            var labelSize = new Vector2(
                SpriteText.getWidthOfString(label),
                SpriteText.getHeightOfString(label));
            var hoveringLabel = mouseX >= labelPosition.X && mouseY >= labelPosition.Y &&
                                mouseX < labelPosition.X + labelSize.X &&
                                mouseY < labelPosition.Y + labelSize.Y;
            if (hoveringLabel)
            {
                Tooltip = Game1.parseText(tooltip, Game1.smallFont, Game1.dialogueWidth / 3);
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
