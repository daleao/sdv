namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Rings.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveRings
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
    public GenericModConfigMenuIntegrationForImmersiveRings(IModRegistry modRegistry, IManifest manifest,
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
            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.RebalancedRings,
                (config, value) => config.RebalancedRings = value
            );
    }
}