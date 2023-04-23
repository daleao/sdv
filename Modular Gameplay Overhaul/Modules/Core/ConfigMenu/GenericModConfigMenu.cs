namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Linq;
using DaLion.Shared.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu : GenericModConfigMenuIntegration<GenericModConfigMenu, ModConfig>
{
    private static bool _reload;

    private GenericModConfigMenu()
        : base(ModHelper.ModRegistry, Manifest)
    {
    }

    /// <summary>Registers the integration and performs initial setup.</summary>
    internal new void Register()
    {
        // register
        this
            .Register(titleScreenOnly: true);

        if (!Data.InitialSetupComplete)
        {
            this.AddParagraph(
                () => "Hi there! Looks like this is your first time starting MARGO.\n\nLet's begin by choosing the modules you want to enable. " +
                      "Only \"Professions\" and \"Tweex\" are enabled by default. Please make sure to read the description pages for each module to learn more about them. " +
                      "When you are done, click on Save & Close.\n\nNote that certain modules may cause a JSON shuffle or other side-effects if enabled or disabled mid-playthrough.");
        }
        else
        {
            this.AddParagraph(
                () => "Choose the modules to enable. " +
                      "You must save and exit this menu after enabling or disabling a module for those changes to take effect. " +
                      "Links to specific module settings pages will appear below for enabled modules. " +
                      "\n\nNote that certain modules may cause a JSON shuffle or other side-effects if enabled or disabled mid-playthrough.");
        }

        this
            .AddModuleSelectionOption();

        if (!Data.InitialSetupComplete)
        {
            return;
        }

        this
            .SetTitleScreenOnlyForNextOptions(false)
            .AddMultiPageLinkOption(
                getOptionName: () => "Module settings:",
                pages: EnumerateModules().Skip(1).Where(m => m._ShouldEnable).ToArray(),
                getPageId: module => module.Namespace,
                getPageName: module => module.Name,
                getColumnsFromWidth: _ => 2);

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

        this.OnFieldChanged((_, _) => { _reload = true; });
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
        Config.Log();
        if (!_reload)
        {
            return;
        }

        this.Reload();
        _reload = false;
    }

    /// <summary>Adds a new instance of <see cref="ModuleSelectionOption"/> to this mod menu.</summary>
    private GenericModConfigMenu AddModuleSelectionOption()
    {
        this.AssertRegistered();
        new ModuleSelectionOption(this.Reload).AddToMenu(this.ModApi, this.ConsumerManifest);
        return this;
    }
}
