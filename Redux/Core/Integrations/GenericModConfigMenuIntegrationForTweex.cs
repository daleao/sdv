namespace DaLion.Redux.Core.Integrations;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the Tweex menu.</summary>
    private void RegisterTweex()
    {
        this._configMenu
            .AddPage(ReduxModule.Tweex.Name, () => "Tweak Settings")

            .AddCheckbox(
                () => "Age Tapper Trees",
                () => "Allows regular trees to age and improve their syrup quality every year.",
                config => config.Tweex.AgeImprovesTreeSap,
                (config, value) => config.Tweex.AgeImprovesTreeSap = value)
            .AddCheckbox(
                () => "Age Bee Houses",
                () => "Allows bee houses to age and improve their honey quality every year.",
                config => config.Tweex.AgeImprovesBeeHouses,
                (config, value) => config.Tweex.AgeImprovesBeeHouses = value)
            .AddNumberField(
                () => "Age Qualiy Improvement Multiplier",
                () =>
                    "Increases or decreases the rate at which age increase product quality for Bee House, Trees and Fruit Trees (higher is faster).",
                config => config.Tweex.AgeImproveQualityFactor,
                (config, value) => config.Tweex.AgeImproveQualityFactor = value,
                0.25f,
                4f)
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
                () => "Professional Foraging In Ginger Island",
                () =>
                    "Extends the perks from Botanist/Ecologist profession to dug-up Ginger and shaken-off Coconuts in Ginger Island.",
                config => config.Tweex.ProfessionalForagingInGingerIsland,
                (config, value) => config.Tweex.ProfessionalForagingInGingerIsland = value)
            .AddCheckbox(
                () => "Kegs Remember Honey Flower",
                () => "Allows Kegs to produce Flower Meads.",
                config => config.Tweex.KegsRememberHoneyFlower,
                (config, value) => config.Tweex.KegsRememberHoneyFlower = value)
            .AddCheckbox(
                () => "Explosion Triggered Bombs",
                () => "Bombs within any explosion radius are immediately triggered.",
                config => config.Tweex.ExplosionTriggeredBombs,
                (config, value) => config.Tweex.ExplosionTriggeredBombs = value);
    }
}
