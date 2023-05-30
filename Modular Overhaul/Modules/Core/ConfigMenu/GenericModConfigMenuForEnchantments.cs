namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Enchantments;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for ENCH.</summary>
    private void AddEnchantmentOptions()
    {
        this
            .AddPage(OverhaulModule.Enchantments.Namespace, I18n.Gmcm_Ench_Heading)

            .AddCheckbox(
                I18n.Gmcm_Ench_Meleeenchantments_Title,
                I18n.Gmcm_Ench_Meleeenchantments_Title,
                config => config.Enchantments.MeleeEnchantments,
                (config, value) =>
                {
                    config.Enchantments.MeleeEnchantments = value;
                    Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                        .Invoke(null);
                })
            .AddCheckbox(
                I18n.Gmcm_Ench_Rangedenchantments_Title,
                I18n.Gmcm_Ench_Rangedenchantments_Desc,
                config => config.Enchantments.RangedEnchantments,
                (config, value) =>
                {
                    config.Enchantments.RangedEnchantments = value;
                    Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                        .Invoke(null);
                })
            .AddCheckbox(
                I18n.Gmcm_Ench_Rebalancedforges_Title,
                I18n.Gmcm_Ench_Rebalancedforges_Desc,
                config => config.Enchantments.RebalancedForges,
                (config, value) => config.Enchantments.RebalancedForges = value)
            .AddDropdown(
                I18n.Gmcm_Ench_Forgesocketstyle_Title,
                I18n.Gmcm_Ench_Forgesocketstyle_Desc,
                config => config.Enchantments.SocketStyle.ToString(),
                (config, value) =>
                {
                    config.Enchantments.SocketStyle = Enum.Parse<Config.ForgeSocketStyle>(value);
                    ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/GemstoneSockets");
                },
                new[] { "Diamond", "Round", "Iridium" },
                value => _I18n.Get("gmcm.ench.forgesocketstyle." + value.ToLowerInvariant()))
            .AddDropdown(
                I18n.Gmcm_Ench_Forgesocketpos_Title,
                I18n.Gmcm_Ench_Forgesocketpos_Desc,
                config => config.Enchantments.SocketPosition.ToString(),
                (config, value) => config.Enchantments.SocketPosition = Enum.Parse<Config.ForgeSocketPosition>(value),
                new[] { "Standard", "AboveSeparator" },
                value => _I18n.Get("gmcm.ench.forgesocketpos." + value.ToLowerInvariant()));
    }
}
