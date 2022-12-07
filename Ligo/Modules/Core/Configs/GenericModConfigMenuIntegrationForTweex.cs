namespace DaLion.Ligo.Modules.Core.Configs;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the Tweex menu.</summary>
    private void RegisterTweex()
    {
        this._configMenu
            .AddPage(LigoModule.Tweex.Namespace, () => "Tweak Settings")

            .AddNumberField(
                () => "Tree Aging Factor",
                () => "The degree to which Tree age improves sap quality. Lower values mean that more time is needed for sap to improve. Set to zero to disable quality sap.",
                config => config.Tweex.TreeAgingFactor,
                (config, value) => config.Tweex.TreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                () => "Fruit Tree Aging Factor",
                () => "The degree to which Fruit Tree age improves fruit quality. Lower values mean that more time is needed for fruits to improve. Set to zero to disable quality fruits.",
                config => config.Tweex.FruitTreeAgingFactor,
                (config, value) => config.Tweex.FruitTreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                () => "Bee House Aging Factor",
                () => "The degree to which Bee House age improves honey quality. Lower values mean that more time is needed for honey to improve. Set to zero to disable quality honey.",
                config => config.Tweex.BeeHouseAgingFactor,
                (config, value) => config.Tweex.BeeHouseAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                () => "Mushroom Box Aging Factor",
                () => "The degree to which Mushroom Box age improves mushroom quality. Lower values mean that more time is needed for mushrooms to improve. Set to zero to disable quality mushrooms.",
                config => config.Tweex.MushroomBoxAgingFactor,
                (config, value) => config.Tweex.MushroomBoxAgingFactor = value,
                0.1f,
                2f)
            .AddCheckbox(
                () => "Deterministic Age Quality",
                () => "Whether age-dependent qualities should be deterministic (true) or stochastic (false).",
                config => config.Tweex.DeterministicAgeQuality,
                (config, value) => config.Tweex.DeterministicAgeQuality = value)
            .AddCheckbox(
                () => "Berry Bushes Reward Exp",
                () => "Gain foraging experience when a berry bush is harvested.",
                config => config.Tweex.BerryBushesRewardExp,
                (config, value) => config.Tweex.BerryBushesRewardExp = value)
            .AddCheckbox(
                () => "Mushroom Boxes Reward Exp",
                () => "Gain foraging experience when a mushroom box is harvested.",
                config => config.Tweex.MushroomBoxesRewardExp,
                (config, value) => config.Tweex.MushroomBoxesRewardExp = value)
            .AddCheckbox(
                () => "Tappers Reward Exp",
                () => "Gain foraging experience when a tapper is harvested.",
                config => config.Tweex.TappersRewardExp,
                (config, value) => config.Tweex.TappersRewardExp = value)
            .AddCheckbox(
                () => "Prevent Fruit Tree Growth in Winter",
                () => "Regular trees can't grow in winter. Why should fruit trees be any different?",
                config => config.Tweex.PreventFruitTreeGrowthInWinter,
                (config, value) => config.Tweex.PreventFruitTreeGrowthInWinter = value)
            .AddCheckbox(
                () => "Large Products Yield Quantity Over Quality",
                () =>
                    "Causes one large egg or milk to produce two mayonnaise / cheese but at regular quality, instead of one at gold quality.",
                config => config.Tweex.LargeProducsYieldQuantityOverQuality,
                (config, value) => config.Tweex.LargeProducsYieldQuantityOverQuality = value)
            .AddCheckbox(
                () => "Explosion Triggered Bombs",
                () => "Bombs within any explosion radius are immediately triggered.",
                config => config.Tweex.ExplosionTriggeredBombs,
                (config, value) => config.Tweex.ExplosionTriggeredBombs = value)
            .AddCheckbox(
                () => "Legendary Fish Always Best Quality",
                () => "Legendary fish are always iridium-quality.",
                config => config.Tweex.LegendaryFishAlwaysBestQuality,
                (config, value) => config.Tweex.LegendaryFishAlwaysBestQuality = value);
    }
}
