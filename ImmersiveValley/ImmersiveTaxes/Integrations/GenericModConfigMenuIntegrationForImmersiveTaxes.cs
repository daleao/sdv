namespace DaLion.Stardew.Taxes.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Taxes.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveTaxes
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public GenericModConfigMenuIntegrationForImmersiveTaxes(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
    {
        _configMenu = new(modRegistry, manifest, log, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()
            .AddNumberField(
                () => "Income Tax Ceiling",
                () => "The taxable percentage of shipped products at the highest tax bracket.",
                config => config.IncomeTaxCeiling,
                (config, value) => config.IncomeTaxCeiling = value,
                0f,
                2f
            )
            .AddNumberField(
                () => "Annual Interest",
                () => "The interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the annual rate.",
                config => config.AnnualInterest,
                (config, value) => config.AnnualInterest = value,
                0f,
                2f
            );
    }
}