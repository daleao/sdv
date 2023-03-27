namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
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
                (config, value) => config.Enchantments.RebalancedForges = value);
    }
}
