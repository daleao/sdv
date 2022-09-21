namespace DaLion.Stardew.Taxes.Integrations;

#region using directives

using System;
using DaLion.Common.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Taxes.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveTaxes
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegrationForImmersiveTaxes"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveTaxes(
        IModRegistry modRegistry,
        IManifest manifest,
        Func<ModConfig> getConfig,
        Action reset,
        Action saveAndApply)
    {
        this._configMenu =
            new GenericModConfigMenuIntegration<ModConfig>(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!this._configMenu.IsLoaded)
        {
            return;
        }

        // register
        this._configMenu
            .Register()
            .AddNumberField(
                () => "Income Tax Ceiling",
                () => "The taxable percentage of shipped products at the highest tax bracket.",
                config => config.IncomeTaxCeiling,
                (config, value) => config.IncomeTaxCeiling = value,
                0f,
                2f)
            .AddNumberField(
                () => "Annual Interest",
                () =>
                    "The interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the annual rate.",
                config => config.AnnualInterest,
                (config, value) => config.AnnualInterest = value,
                0f,
                2f);
    }
}
