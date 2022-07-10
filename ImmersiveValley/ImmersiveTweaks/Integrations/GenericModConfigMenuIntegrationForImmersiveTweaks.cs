namespace DaLion.Stardew.Tweex.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;
using System;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Tweaks.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveTweaks
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveTweaks(IModRegistry modRegistry, IManifest manifest,
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
            .AddCheckbox(
                () => "Age Tapper Trees",
                () => "Allows regular trees to age and improve their syrup quality every year.",
                config => config.AgeSapTrees,
                (config, value) => config.AgeSapTrees = value
            )
            .AddCheckbox(
                () => "Age Bee Houses",
                () => "Allows bee houses to age and improve their honey quality every year.",
                config => config.AgeBeeHouses,
                (config, value) => config.AgeBeeHouses = value
            )
            .
            AddNumberField(
                () => "Age Qualiy Improvement Multiplier",
                () => "Increases or decreases the rate at which age increase product quality for Bee House, Trees and Fruit Trees (higher is faster).",
                config => config.AgeImproveQualityFactor,
                (config, value) => config.AgeImproveQualityFactor = value,
                0.25f,
                4f
            )
            .AddCheckbox(
                () => "Deterministic Age Quality",
                () => "Whether age-dependent qualities should be deterministic (true) or stochastic (false).",
                config => config.DeterministicAgeQuality,
                (config, value) => config.DeterministicAgeQuality = value
            )
            .AddCheckbox(
                () => "Berry Bushes Reward Exp",
                () => "Gain foraging experience when a berry bush is harvested.",
                config => config.BerryBushesRewardExp,
                (config, value) => config.BerryBushesRewardExp = value
            )
            .AddCheckbox(
                () => "Mushroom Boxes Reward Exp",
                () => "Gain foraging experience when a mushroom box is harvested.",
                config => config.MushroomBoxesRewardExp,
                (config, value) => config.MushroomBoxesRewardExp = value
            )
            .AddCheckbox(
                () => "Tappers Reward Exp",
                () => "Gain foraging experience when a tapper is harvested.",
                config => config.TappersRewardExp,
                (config, value) => config.TappersRewardExp = value
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
                () => "Professional Foraging In Ginger Island",
                () =>
                    "Extends the perks from Botanist/Ecologist profession to dug-up Ginger and shaken-off Coconuts in Ginger Island.",
                config => config.ProfessionalForagingInGingerIsland,
                (config, value) => config.ProfessionalForagingInGingerIsland = value
            )
            .AddCheckbox(
                () => "Explosion Triggered Bombs",
                () => "Bombs within any explosion radius are immediately triggered.",
                config => config.ExplosionTriggeredBombs,
                (config, value) => config.ExplosionTriggeredBombs = value
            );
    }
}