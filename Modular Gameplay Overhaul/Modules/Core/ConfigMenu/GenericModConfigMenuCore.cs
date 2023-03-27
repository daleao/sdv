namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore : GenericModConfigMenuIntegration<GenericModConfigMenuCore, ModConfig>
{
    private static bool _reload;

    private GenericModConfigMenuCore()
        : base(ModHelper.ModRegistry, Manifest)
    {
    }

    /// <summary>Registers the integration and performs initial setup.</summary>
    internal new void Register()
    {
        // register
        this
            .Register(titleScreenOnly: true)

            .AddParagraph(
                () => "Choose the modules to enable. " +
                      "When a module is enabled, it's settings will be added to the Generic Mod Config Menu in the page links below. You must save and exit the menu for the changes to take effect. " +
                      "Note that certain modules may cause a JSON shuffle or other harmful side-effects if enabled or disabled mid-playthrough.")
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Professions.Name,
                () => "Whether to enable the Professions module, which overhauls the game's professions with the goal of supporting more diverse and interesting playstyles.",
                config => config.EnableProfessions,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Professions.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Professions.Deactivate();
                    }

                    config.EnableProfessions = value;
                },
                OverhaulModule.Professions.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Combat.Name,
                () => "Whether to enable the Combat module, which re-balances offensive stats and allows customizing combat difficulty.",
                config => config.EnableCombat,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Combat.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Combat.Deactivate();
                    }

                    config.EnableCombat = value;
                },
                OverhaulModule.Combat.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Weapons.Name,
                () => "Whether to enable the Weapons module, which adds several new gameplay mechanics and re-balances for Melee Weapons so as to diversify combat and provide viable alternatives to the ubiquitous sword." +
                      "**Please be wary of Json Shuffle when toggling this module.**",
                config => config.EnableWeapons,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Weapons.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Weapons.Deactivate();
                    }

                    config.EnableWeapons = value;
                },
                OverhaulModule.Combat.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Slingshots.Name,
                () => "Whether to enable the Slingshots module, which bring Slingshots up to par with Melee Weapons by adding the ability to crit., perform special moves and receive enchantments.",
                config => config.EnableWeapons,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Slingshots.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Slingshots.Deactivate();
                    }

                    config.EnableSlingshots = value;
                },
                OverhaulModule.Combat.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Tools.Name,
                () => "Whether to enable the Tools module, which enables full customization of farming tools, allows Axe and Pick to be charged and several additional quality-of-life features.",
                config => config.EnableTools,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Tools.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Tools.Deactivate();
                    }

                    config.EnableTools = value;
                },
                OverhaulModule.Tools.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Enchantments.Name,
                () => "Whether to enable the Enchantments module, which re-balances certain underwhelming gemstone enchantments and completely overhauls the enchantment pool for both Melee Weapons and Slingshots.",
                config => config.EnableEnchantments,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Enchantments.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Enchantments.Deactivate();
                    }

                    config.EnableEnchantments = value;
                },
                OverhaulModule.Combat.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Rings.Name,
                () => "Whether to enable the Rings module, which re-balance certain underwhelming rings, make ring crafting more immersive, and overhaul the Iridium Band as a powerful late-game combat asset." +
                      "**Please be wary of Json Shuffle when toggling this module.**",
                config => config.EnableRings,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Rings.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Rings.Deactivate();
                    }

                    config.EnableRings = value;
                },
                OverhaulModule.Rings.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Ponds.Name,
                () => "Whether to enable the Ponds module, which will make Fish Ponds useful and immersive by preserving fish quality, scaling roe production with population, and spontaneously growing algae if left empty. " +
                      "**Please clear all Fish Ponds before disabling this module.**",
                config => config.EnablePonds,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Ponds.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Ponds.Deactivate();
                    }

                    config.EnablePonds = value;
                },
                OverhaulModule.Ponds.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Taxes.Name,
                () => "Whether to enable the Taxes module, which introduces a realistic taxation system for additional challenge and an end-game gold sink.",
                config => config.EnableTaxes,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Taxes.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Taxes.Deactivate();
                    }

                    config.EnableTaxes = value;
                },
                OverhaulModule.Taxes.Namespace)
            .AddCheckbox(
                () => "Enable " + OverhaulModule.Tweex.Name,
                () => "Whether to enable the Tweex module, which fixes misc. vanilla inconsistencies for greater immersion.",
                config => config.EnableTweex,
                (config, value) =>
                {
                    if (value)
                    {
                        OverhaulModule.Tweex.Activate(ModHelper);
                    }
                    else
                    {
                        OverhaulModule.Tweex.Deactivate();
                    }

                    config.EnableTweex = value;
                },
                OverhaulModule.Tweex.Namespace);

        this.SetTitleScreenOnlyForNextOptions(false);

        // add page links
        if (Config.EnableCombat)
        {
            this.AddPageLink(OverhaulModule.Combat.Namespace, () => "Go to Combat settings");
        }

        if (Config.EnableEnchantments)
        {
            this.AddPageLink(OverhaulModule.Enchantments.Namespace, () => "Go to Enchantment settings");
        }

        if (Config.EnablePonds)
        {
            this.AddPageLink(OverhaulModule.Ponds.Namespace, () => "Go to Pond settings");
        }

        if (Config.EnableProfessions)
        {
            this.AddPageLink(OverhaulModule.Professions.Namespace, () => "Go to Profession settings");
        }

        if (Config.EnableRings)
        {
            this.AddPageLink(OverhaulModule.Rings.Namespace, () => "Go to Ring settings");
        }

        if (Config.EnableSlingshots)
        {
            this.AddPageLink(OverhaulModule.Slingshots.Namespace, () => "Go to Slingshot settings");
        }

        if (Config.EnableTools)
        {
            this.AddPageLink(OverhaulModule.Tools.Namespace, () => "Go to Tool settings");
        }

        if (Config.EnableTaxes)
        {
            this.AddPageLink(OverhaulModule.Taxes.Namespace, () => "Go to Tax settings");
        }

        if (Config.EnableTweex)
        {
            this.AddPageLink(OverhaulModule.Tweex.Namespace, () => "Go to Tweak settings");
        }

        if (Config.EnableWeapons)
        {
            this.AddPageLink(OverhaulModule.Weapons.Namespace, () => "Go to Weapon settings");
        }

        // add page contents
        if (Config.EnableCombat)
        {
            this.RegisterCombat();
        }

        if (Config.EnableEnchantments)
        {
            this.RegisterEnchantments();
        }

        if (Config.EnablePonds)
        {
            this.RegisterPonds();
        }

        if (Config.EnableProfessions)
        {
            this.RegisterProfessions();
        }

        if (Config.EnableRings)
        {
            this.RegisterRings();
        }

        if (Config.EnableSlingshots)
        {
            this.RegisterSlingshots();
        }

        if (Config.EnableTools)
        {
            this.RegisterTools();
        }

        if (Config.EnableTaxes)
        {
            this.RegisterTaxes();
        }

        if (Config.EnableTweex)
        {
            this.RegisterTweex();
        }

        if (Config.EnableWeapons)
        {
            this.RegisterWeapons();
        }

        this.OnFieldChanged((id, _) =>
        {
            foreach (var module in EnumerateModules())
            {
                if (id != module.Namespace)
                {
                    continue;
                }

                _reload = true;
                break;
            }
        });
    }

    /// <summary>Resets the mod config menu.</summary>
    internal void Reload()
    {
        this.Unregister().Register();
        Log.D("[GMCM]: The Modular Overhaul config menu has been reloaded.");
    }

    /// <inheritdoc />
    protected override ModConfig GetConfig()
    {
        return Config;
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new ModConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);
        ModHelper.LogConfig(Config);
        if (!_reload)
        {
            return;
        }

        this.Reload();
        _reload = false;
    }
}
