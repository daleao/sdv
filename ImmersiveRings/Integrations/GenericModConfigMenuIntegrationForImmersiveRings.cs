namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Stardew.Integrations;

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
            .Register(true)
            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.RebalancedRings,
                (config, value) => config.RebalancedRings = value
            )
            .AddCheckbox(
                () => "Craftable Gem Rings",
                () => "Adds new combat recipes for crafting gemstone rings.",
                config => config.CraftableGemRings,
                (config, value) => config.CraftableGemRings = value
            )
            .AddCheckbox(
                () => "Craftable Glow And Magnet Rings",
                () => "Adds new mining recipes for crafting glow and magnet rings.",
                config => config.CraftableGlowAndMagnetRings,
                (config, value) => config.CraftableGlowAndMagnetRings = value
            )
            .AddCheckbox(
                () => "Immersive Glowstone Recipe",
                () => "Replaces the glowstone ring recipe.",
                config => config.ImmersiveGlowstoneRecipe,
                (config, value) => config.ImmersiveGlowstoneRecipe = value
            )
            .AddCheckbox(
                () => "Forgeable Iridium Band",
                () => "Replaces the iridium band recipe and effect. Adds new forge mechanics.",
                config => config.ForgeableIridiumBand,
                (config, value) => config.ForgeableIridiumBand = value
            );
    }
}