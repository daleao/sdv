namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Enchantments;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the Arsenal config menu.</summary>
    private void RegisterEnchantments()
    {
        this
            .AddPage(OverhaulModule.Enchantments.Namespace, () => "Enchantment Settings")

            .AddCheckbox(
                () => "Melee Enchantments",
                () => "Whether to use the new and objectively better Melee Weapon enchantments.",
                config => config.Enchantments.MeleeEnchantments,
                (config, value) => config.Enchantments.MeleeEnchantments = value)
            .AddCheckbox(
                () => "Melee Enchantments",
                () => "Whether to use the new Slingshot enchantments. These enchantments can only be applied if the Slingshot Module is enabled.",
                config => config.Enchantments.RangedEnchantments,
                (config, value) => config.Enchantments.RangedEnchantments = value)
            .AddCheckbox(
                () => "Rebalanced Forges",
                () => "Improves certain underwhelming forges (analogous to changes by Rings module).",
                config => config.Enchantments.RebalancedForges,
                (config, value) => config.Enchantments.RebalancedForges = value)
            .AddDropdown(
                () => "Forge Socket Style",
                () => "Determines the style of the sprite used to represent gemstone forges in tooltips, if enabled.",
                config => config.Enchantments.SocketStyle.ToString(),
                (config, value) =>
                {
                    config.Enchantments.SocketStyle = Enum.Parse<Config.ForgeSocketStyle>(value);
                    Textures.ForgeIconTx = ModHelper.ModContent.Load<Texture2D>("assets/menus/ForgeIcon" +
                        $"_{config.Enchantments.SocketStyle}" +
                        (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
                         ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2")
                            ? "_Vintage"
                            : string.Empty));
                },
                new[] { "Diamond", "Round", "Iridium" },
                null)
            .AddDropdown(
                () => "Forge Socket Position",
                () => "Determines the relative position where forge sockets should be drawn, if enabled.",
                config => config.Enchantments.SocketPosition.ToString(),
                (config, value) => config.Enchantments.SocketPosition = Enum.Parse<Config.ForgeSocketPosition>(value),
                new[] { "Standard", "AboveSeparator" },
                null);
    }
}
