namespace DaLion.Stardew.Ponds.Integrations;

#region using directives

using Common.Integrations.GenericModConfigMenu;
using System;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Ponds.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersivePonds
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersivePonds(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu = new(modRegistry, manifest, getConfig, reset, saveAndApply);
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
                () => "Roe Production Chance Multiplier",
                () => "Multiplies a fish's base chance to produce roe each day.",
                config => config.RoeProductionChanceMultiplier,
                (config, value) => config.RoeProductionChanceMultiplier = value,
                0.1f,
                2f
            );
    }
}