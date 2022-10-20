namespace DaLion.Stardew.Rings.Integrations;

#region using directives

using DaLion.Common.Extensions.SMAPI;
using DaLion.Common.Integrations.GenericModConfigMenu;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Rings.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveRings
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuIntegrationForImmersiveRings"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveRings(
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
            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.RebalancedRings,
                (config, value) =>
                {
                    config.RebalancedRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                })
            .AddCheckbox(
                () => "Craftable Gemstone Rings",
                () => "Adds new combat recipes for crafting gemstone rings.",
                config => config.CraftableGemRings,
                (config, value) =>
                {
                    config.CraftableGemRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                })
            .AddCheckbox(
                () => "Craftable Glow and Magnet Rings",
                () => "Adds new mining recipes for crafting glow and magnet rings.",
                config => config.CraftableGlowAndMagnetRings,
                (config, value) =>
                {
                    config.CraftableGlowAndMagnetRings = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "Immersive Glowstone Recipe",
                () => "Replaces the glowstone ring recipe with one that makes sense.",
                config => config.ImmersiveGlowstoneRecipe,
                (config, value) =>
                {
                    config.ImmersiveGlowstoneRecipe = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .AddCheckbox(
                () => "The One Iridium Band",
                () => "Replaces the Iridium Band recipe and effect. Adds new forge mechanics.",
                config => config.TheOneIridiumBand,
                (config, value) =>
                {
                    config.TheOneIridiumBand = value;
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                })
            .AddCheckbox(
                () => "The One Infinity Band",
                () => "If The One Iridium Band is enabled, adds additional requirements to obtain the ultimate ring.",
                config => config.TheOneInfinityBand,
                (config, value) =>
                {
                    config.TheOneInfinityBand = value;
                });
    }
}
