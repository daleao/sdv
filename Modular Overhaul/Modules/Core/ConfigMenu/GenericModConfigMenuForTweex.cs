namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

using System.Collections.Generic;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for TWX.</summary>
    private void AddMiscOptions()
    {
        this
            .AddPage(OverhaulModule.Tweex.Namespace, I18n.Gmcm_Txs_Heading)

            .AddSectionTitle(I18n.Gmcm_Twx_Quality_Heading)
            .AddNumberField(
                I18n.Gmcm_Twx_Treeagingfactor_Title,
                I18n.Gmcm_Twx_Treeagingfactor_Desc,
                config => config.Tweex.TreeAgingFactor,
                (config, value) => config.Tweex.TreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Fruittreeagingfactor_Title,
                I18n.Gmcm_Twx_Fruittreeagingfactor_Desc,
                config => config.Tweex.FruitTreeAgingFactor,
                (config, value) => config.Tweex.FruitTreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Beehouseagingfactor_Title,
                I18n.Gmcm_Twx_Beehouseagingfactor_Desc,
                config => config.Tweex.BeeHouseAgingFactor,
                (config, value) => config.Tweex.BeeHouseAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Mushroomboxagingfactor_Title,
                I18n.Gmcm_Twx_Mushroomboxagingfactor_Desc,
                config => config.Tweex.MushroomBoxAgingFactor,
                (config, value) => config.Tweex.MushroomBoxAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Teabushagingfactor_Title,
                I18n.Gmcm_Twx_Teabushagingfactor_Desc,
                config => config.Tweex.TeaBushAgingFactor,
                (config, value) => config.Tweex.TeaBushAgingFactor = value,
                0.1f,
                2f)
            .AddCheckbox(
                I18n.Gmcm_Twx_Deterministicagequality_Title,
                I18n.Gmcm_Twx_Deterministicagequality_Desc,
                config => config.Tweex.DeterministicAgeQuality,
                (config, value) => config.Tweex.DeterministicAgeQuality = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Millspreservequality_Title,
                I18n.Gmcm_Twx_Millspreservequality_Desc,
                config => config.Tweex.MillsPreserveQuality,
                (config, value) => config.Tweex.MillsPreserveQuality = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Profs_Experience_Heading)
            .AddNumberField(
                I18n.Gmcm_Twx_Berrybushexpreward_Title,
                I18n.Gmcm_Twx_Berrybushexpreward_Desc,
                config => (int)config.Tweex.BerryBushExpReward,
                (config, value) => config.Tweex.BerryBushExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Mushroomboxexpreward_Title,
                I18n.Gmcm_Twx_Mushroomboxexpreward_Desc,
                config => (int)config.Tweex.MushroomBoxExpReward,
                (config, value) => config.Tweex.MushroomBoxExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Tapperexpreward_Title,
                I18n.Gmcm_Twx_Tapperexpreward_Desc,
                config => (int)config.Tweex.TapperExpReward,
                (config, value) => config.Tweex.TapperExpReward = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Other_Heading)
            .AddCheckbox(
                I18n.Gmcm_Twx_Preventfruittreewintergrowth_Title,
                I18n.Gmcm_Twx_Preventfruittreewintergrowth_Desc,
                config => config.Tweex.PreventFruitTreeWinterGrowth,
                (config, value) => config.Tweex.PreventFruitTreeWinterGrowth = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Largeproductsyieldquantityoverquality_Title,
                I18n.Gmcm_Twx_Largeproductsyieldquantityoverquality_Desc,
                config => config.Tweex.LargeProducsYieldQuantityOverQuality,
                (config, value) => config.Tweex.LargeProducsYieldQuantityOverQuality = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Explosiontriggeredbombs_Title,
                I18n.Gmcm_Twx_Explosiontriggeredbombs_Desc,
                config => config.Tweex.ExplosionTriggeredBombs,
                (config, value) => config.Tweex.ExplosionTriggeredBombs = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Legendaryfishalwaysbestquality_Title,
                I18n.Gmcm_Twx_Legendaryfishalwaysbestquality_Desc,
                config => config.Tweex.LegendaryFishAlwaysBestQuality,
                (config, value) => config.Tweex.LegendaryFishAlwaysBestQuality = value);

        var farmMaps = new List<string> { "IslandWest" };
        if (this.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            farmMaps.AddRange(new[] { "Custom_Garden", "Custom_GrampletonFields" });
        }

        if (this.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage"))
        {
            farmMaps.Add("Custom_Ridgeside_SummitFarm");
        }

        if (this.ModRegistry.IsLoaded("LemurKat.EastScarpe.SMAPI"))
        {
            farmMaps.Add("Custom_ESMeadowFarm");
        }

        this.AddMultiCheckboxOption(
            I18n.Gmcm_Twx_Spawncrowsonthesemaps_Title,
            farmMaps.ToArray(),
            map => TweexModule.Config.SpawnCrowsOnTheseMaps.Contains(map),
            (map, value) =>
            {
                if (value)
                {
                    TweexModule.Config.SpawnCrowsOnTheseMaps.Add(map);
                    if (map == "Custom_GrampletonFields")
                    {
                        TweexModule.Config.SpawnCrowsOnTheseMaps.Add("Custom_GrampletonFields_Small");
                    }
                }
                else
                {
                    TweexModule.Config.SpawnCrowsOnTheseMaps.Remove(map);
                    if (map == "Custom_GrampletonFields")
                    {
                        TweexModule.Config.SpawnCrowsOnTheseMaps.Remove("Custom_GrampletonFields_Small");
                    }
                }
            },
            _ => 2,
            map => _I18n.Get("gmcm.twx.maps." + map.ToLowerInvariant()));
    }
}
