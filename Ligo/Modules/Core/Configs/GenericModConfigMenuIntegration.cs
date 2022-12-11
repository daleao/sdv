namespace DaLion.Ligo.Modules.Core.Configs;

#region using directives

using DaLion.Shared.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegration"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    internal GenericModConfigMenuIntegration(
        IModRegistry modRegistry,
        IManifest manifest,
        Func<ModConfig> getConfig,
        Action reset,
        Action saveAndApply)
    {
        this._configMenu =
            new GenericModConfigMenuIntegration<ModConfig>(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the core config menu, if it is available.</summary>
    internal void Register()
    {
        // get config menu
        if (!this._configMenu.IsLoaded)
        {
            return;
        }

        // register
        this._configMenu
            .Register(titleScreenOnly: true)

            .AddParagraph(
                () => "Choose the modules to enable. " +
                      "When a module is enabled, it's settings will be added to the Generic Mod Config Menu in the page links below. You must save and exit the menu for the changes to take effect. " +
                      "Note that disabling modules at runtime is not supported; you must re-launch the game completely for a disable to take effect. " +
                      "Certain modules can be dangerous to the save file if enabled or disabled mid-playthrough.")
            .AddCheckbox(
                () => LigoModule.Arsenal.DisplayName,
                () => "Whether to enable the Arsenal module. This will overhaul weapon enchantments and introduce new weapon mechanics like combo hits. This will also make Slingshots more on par with other weapons by allowing critical hits, enchantments and other features. " +
                      "Before disabling this module make sure to disable the `BringBangStabbySwords` setting to avoid being stuck with unusable weapons. ",
                config => config.EnableArsenal,
                (config, value) => config.EnableArsenal = value,
                LigoModule.Arsenal.Namespace)
            .AddCheckbox(
                () => LigoModule.Ponds.DisplayName,
                () => "Whether to enable the Ponds module. This will make Fish Ponds useful and immersive by preserving fish quality, scaling roe production with population, and spontaneously growing algae if left empty. " +
                      "Before disabling this module you should reset all pond data using provided console commands.",
                config => config.EnablePonds,
                (config, value) => config.EnablePonds = value,
                LigoModule.Ponds.Namespace)
            .AddCheckbox(
                () => LigoModule.Professions.DisplayName,
                () => "Whether to enable the main Professions module. This will overhaul the game's professions, introducing new gameplay mechanics and optionally extending the skill progression for very late-game saves. " +
                      "This module should be safe to enable or disable at any time. Keep in mind that simply disabling the module will not remove or otherwise make changes to character skill levels or unlocked professions. You may use the provided console commands to restore vanilla settings. ",
                config => config.EnableProfessions,
                (config, value) => config.EnableProfessions = value,
                LigoModule.Professions.Namespace)
            .AddCheckbox(
                () => LigoModule.Rings.DisplayName,
                () => "Whether to enable the Rings module. This will rebalance certain underwhelming rings, make ring crafting more immersive, and overhaul the Iridium Band as a powerful late-game combat asset. " +
                      "Please note that this module introduces new items via Json Assets, and therefore enabling or disabling it mid-playthrough will cause a Json Shuffle. ",
                config => config.EnableRings,
                (config, value) => config.EnableRings = value,
                LigoModule.Rings.Namespace)
            .AddCheckbox(
                () => LigoModule.Taxes.DisplayName,
                () => "Whether to enable the Taxes module. This will introduce a simple yet realistic taxation system to the game. Because surely a nation at war would be on top of that juicy end-game income. " +
                      "This module should be safe to enable or disable at any time.",
                config => config.EnableTaxes,
                (config, value) => config.EnableTaxes = value,
                LigoModule.Taxes.Namespace)
            .AddCheckbox(
                () => LigoModule.Tools.DisplayName,
                () => "Whether to enable the Tools module. This will allow Axe and Pick to charge up like the Hoe and Watering Can, and optionally allow customizing the affected area of all these tools. " +
                      "This module should be safe to enable or disable at any time.",
                config => config.EnableTools,
                (config, value) => config.EnableTools = value,
                LigoModule.Tools.Namespace)
            .AddCheckbox(
                () => LigoModule.Tweex.DisplayName,
                () => "Whether to enable the Tweaks module. This will fix misc. vanilla inconsistencies and balancing issues. " +
                      "This module should be safe to enable or disable at any time. ",
                config => config.EnableTweex,
                (config, value) => config.EnableTweex = value,
                LigoModule.Tweex.Namespace);

        this._configMenu.SetTitleScreenOnlyForNextOptions(false);

        // add page links
        if (Config.EnableArsenal)
        {
            this._configMenu.AddPageLink(LigoModule.Arsenal.Namespace, () => "Go to Arsenal settings");
        }

        if (Config.EnablePonds)
        {
            this._configMenu.AddPageLink(LigoModule.Ponds.Namespace, () => "Go to Pond settings");
        }

        if (Config.EnableProfessions)
        {
            this._configMenu.AddPageLink(LigoModule.Professions.Namespace, () => "Go to Profession settings");
        }

        if (Config.EnableRings)
        {
            this._configMenu.AddPageLink(LigoModule.Rings.Namespace, () => "Go to Ring settings");
        }

        if (Config.EnableTools)
        {
            this._configMenu.AddPageLink(LigoModule.Tools.Namespace, () => "Go to Tool settings");
        }

        if (Config.EnableTaxes)
        {
            this._configMenu.AddPageLink(LigoModule.Taxes.Namespace, () => "Go to Tax settings");
        }

        if (Config.EnableTweex)
        {
            this._configMenu.AddPageLink(LigoModule.Tweex.Namespace, () => "Go to Tweak settings");
        }

        // add page contents
        if (Config.EnableArsenal)
        {
            this.RegisterArsenal();
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
    }

    /// <summary>Reload the core config menu.</summary>
    internal void Reload()
    {
        this._configMenu.Unregister();
        this.Register();
        Log.D("The mod config menu was reset.");
    }
}
