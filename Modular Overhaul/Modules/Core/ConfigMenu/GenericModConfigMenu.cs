namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Linq;
using DaLion.Shared.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu : GenericModConfigMenuIntegration<GenericModConfigMenu, ModConfig>
{
    private static bool _reload;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenu"/> class.</summary>
    internal GenericModConfigMenu()
        : base(ModHelper.ModRegistry, Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.SetTitleScreenOnlyForNextOptions(true);
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

        this.AddModuleSelectionOption();
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
        if (Config.EnableProfessions)
        {
            this.AddProfessionOptions();
        }

        if (Config.EnableCombat)
        {
            this.AddCombatOptions();
        }

        if (Config.EnableWeapons)
        {
            this.AddWeaponOptions();
        }

        if (Config.EnableSlingshots)
        {
            this.AddSlingshotOptions();
        }

        if (Config.EnableTools)
        {
            this.AddToolOptions();
        }

        if (Config.EnableEnchantments)
        {
            this.AddEnchantmentOptions();
        }

        if (Config.EnableRings)
        {
            this.AddRingOptions();
        }

        if (Config.EnablePonds)
        {
            this.AddPondOptions();
        }

        if (Config.EnableTaxes)
        {
            this.AddTaxOptions();
        }

        if (Config.EnableTweex)
        {
            this.AddMiscOptions();
        }

        this.OnFieldChanged((_, _) => { _reload = true; });
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
