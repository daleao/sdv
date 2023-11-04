namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System;
using System.Linq;
using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal class ModuleSelectionOption : MultiCheckboxOption<OverhaulModule>
{
    private readonly Action _reloadMenu;

    /// <summary>Initializes a new instance of the <see cref="ModuleSelectionOption"/> class.</summary>
    /// <param name="reloadMenu">A delegate to the parent menu's Reload method, to be invoked if any modules are toggled.</param>
    internal ModuleSelectionOption(Action reloadMenu)
    : base(
        getOptionName: I18n.Gmcm_Core_Available,
        checkboxes: EnumerateModules().Skip(1).ToArray(),
        getCheckboxValue: module => module._ShouldEnable,
        setCheckboxValue: (module, value) => module._ShouldEnable = value,
        getCheckboxLabel: module => module.DisplayName,
        getCheckboxTooltip: module => module.Description,
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
        this._reloadMenu = reloadMenu;
    }

    /// <inheritdoc />
    protected override void AfterSave()
    {
        if (this.updatedValues.Any() || Config.LaunchInitialSetup)
        {
            Config.LaunchInitialSetup = false;
            ModHelper.WriteConfig(Config);
            this._reloadMenu();
        }

        this.updatedValues.Clear();
    }
}
