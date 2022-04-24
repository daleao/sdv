namespace DaLion.Stardew.Tweaks.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Stardew.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Tweaks.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveTweaks
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
    public GenericModConfigMenuIntegrationForImmersiveTweaks(IModRegistry modRegistry, IManifest manifest,
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
                () => "Age Tapper Trees",
                () => "Allows regular trees to age and improve their syrup quality every year.",
                config => config.AgeTapperTrees,
                (config, value) => config.AgeTapperTrees = value
            )
            .AddCheckbox(
                () => "Tappers Reward Exp",
                () => "Gain foraging experience when a tapper is harvested.",
                config => config.TappersRewardExp,
                (config, value) => config.TappersRewardExp = value
            )
            .AddCheckbox(
                () => "Age Bee Houses",
                () => "Allows bee houses to age and improve their honey quality every year.",
                config => config.AgeBeeHouses,
                (config, value) => config.AgeBeeHouses = value
            )
            .AddCheckbox(
                () => "Kegs Remember Honey Flower",
                () => "Allows the production of tasty flowery meads.",
                config => config.KegsRememberHoneyFlower,
                (config, value) => config.KegsRememberHoneyFlower = value
            )
            .AddDropdown(
                () => "Honey Mead Style",
                () => "The visual style for different honey mead icons, if using BetterArtisanGoodIcons.",
                config => config.HoneyMeadStyle.ToString(),
                (config, value) => config.HoneyMeadStyle = value,
                new[] {"ColoredBottles", "ColoredCaps", "Elesea"},
                value =>
                {
                    ModEntry.ModHelper.Content.InvalidateCache($"{ModEntry.Manifest.UniqueID}/BetterHoneyMeadIcons");
                    return value;
                })
            .AddCheckbox(
                () => "Berry Bushes Reward Exp",
                () => "Gain foraging experience when a berry bush is harvested.",
                config => config.BerryBushesRewardExp,
                (config, value) => config.BerryBushesRewardExp = value
            )
            .AddCheckbox(
                () => "Prevent Fruit Tree Growth in Winter",
                () => "Regular trees can't grow in winter. Why should fruit trees be any different?",
                config => config.PreventFruitTreeGrowthInWinter,
                (config, value) => config.PreventFruitTreeGrowthInWinter = value
            )
            .AddCheckbox(
                () => "Large Products Yield Quantity Over Quality",
                () =>
                    "Causes one large egg or milk to produce two mayonnaise / cheese but at regular quality, instead of one at gold quality.",
                config => config.LargeProducsYieldQuantityOverQuality,
                (config, value) => config.LargeProducsYieldQuantityOverQuality = value
            )
            .AddCheckbox(
                () => "Explosion Triggered Bombs",
                () => "Bombs within any explosion radius are immediately triggered.",
                config => config.ExplosionTriggeredBombs,
                (config, value) => config.ExplosionTriggeredBombs = value
            ).AddCheckbox(
                () => "Extended Foraging Perks",
                () => "Extends the perks from Botanist/Ecologist profession to Ginger and Coconuts shaken off of palm trees.",
                config => config.ExtendedForagingPerks,
                (config, value) => config.ExtendedForagingPerks = value
            );
    }
}