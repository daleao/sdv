namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Integrations.GenericModConfigMenu;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Slingshots config menu.</summary>
    private void AddSlingshotOptions()
    {
        this
            .AddPage(OverhaulModule.Slingshots.Namespace, () => "Slingshot Settings")

            .AddSectionTitle(() => "General Settings")
            .AddCheckbox(
                () => "Enable Rebalance",
                () => "Whether to re-balance the damage and knockback modifiers of Slingshots to achieve a better balance when the features below are enabled.",
                config => config.Slingshots.EnableRebalance,
                (config, value) => config.Slingshots.EnableRebalance = value)
            .AddCheckbox(
                () => "Enable Critical Hits",
                () => "Allows slingshots to deal critical damage and be affected by critical modifiers.",
                config => config.Slingshots.EnableCriticalHits,
                (config, value) => config.Slingshots.EnableCriticalHits = value)
            .AddCheckbox(
                () => "Enable Enchantments",
                () => "Allows slingshots to be enchanted at the Forge. Includes both gemstone enchantments and Prismatic Shard enchantments, provided the Enchantments Module is enabled to provide them.",
                config => config.Slingshots.EnableEnchantments,
                (config, value) => config.Slingshots.EnableEnchantments = value)
            .AddCheckbox(
                () => "Enable Special Move",
                () => "Enables a custom stunning smack special move for slingshots.",
                config => config.Slingshots.EnableSpecialMove,
                (config, value) => config.Slingshots.EnableSpecialMove = value)
            .AddCheckbox(
                () => "Disable Grace Period",
                () => "Allows slingshot projectiles to hit targets before the 100-ms grace period.",
                config => config.Slingshots.DisableGracePeriod,
                (config, value) => config.Slingshots.DisableGracePeriod = value)
            .AddCheckbox(
                () => "Bullseye Replaces Cursor",
                () =>
                    "Draws a bulls-eye instead of the mouse cursor while firing a slingshot. This option does not support pull-back firing for obvious reasons.",
                config => config.Slingshots.BullseyeReplacesCursor,
                (config, value) => config.Slingshots.BullseyeReplacesCursor = value)
            .AddCheckbox(
                () => "Enable Infinity Slingshot",
                () => "Allows creating the Infinity Slingshot at the Forge.",
                config => config.Slingshots.EnableInfinitySlingshot,
                (config, value) => config.Slingshots.EnableInfinitySlingshot = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Movement & Control Settings")
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging a slingshot (for special moves).",
                config => config.Weapons.FaceMouseCursor,
                (config, value) => config.Weapons.FaceMouseCursor = value)
            .AddCheckbox(
                () => "Slick Moves",
                () => "Drift in the current running direction when firing a slingshot.",
                config => config.Slingshots.SlickMoves,
                (config, value) => config.Slingshots.SlickMoves = value)
            .AddCheckbox(
                () => "Enable Auto-Selection",
                () => "The chosen slingshot will be automatically equipped near enemies.",
                config => config.Slingshots.EnableAutoSelection,
                (config, value) =>
                {
                    config.Slingshots.EnableAutoSelection = value;
                    if (!value)
                    {
                        SlingshotsModule.State.AutoSelectableSlingshot = null;
                    }
                })
            .AddKeyBinding(
                () => "Selection Key",
                () => "The key used for choosing a slingshot for auto-selection, if enabled.",
                config => config.Slingshots.SelectionKey,
                (config, value) => config.Slingshots.SelectionKey = value)
            .AddColorPicker(
                () => "Selection Border Color",
                () => "The color used to indicate the chosen slingshot for auto-selected.",
                config => config.Slingshots.SelectionBorderColor,
                (config, value) => config.Slingshots.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddNumberField(
                () => "Auto-Selection Range",
                () => "The minimum distance away from a monster to auto-select your chosen slingshot.",
                config => (int)config.Slingshots.AutoSelectionRange,
                (config, value) => config.Slingshots.AutoSelectionRange = (uint)value,
                1,
                9);
    }
}
