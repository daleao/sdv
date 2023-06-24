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
                I18n.Gmcm_Twx_Quality_Treeagingfactor_Title,
                I18n.Gmcm_Twx_Quality_Treeagingfactor_Desc,
                config => config.Tweex.TreeAgingFactor,
                (config, value) => config.Tweex.TreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Quality_Fruittreeagingfactor_Title,
                I18n.Gmcm_Twx_Quality_Fruittreeagingfactor_Desc,
                config => config.Tweex.FruitTreeAgingFactor,
                (config, value) => config.Tweex.FruitTreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Quality_Beehouseagingfactor_Title,
                I18n.Gmcm_Twx_Quality_Beehouseagingfactor_Desc,
                config => config.Tweex.BeeHouseAgingFactor,
                (config, value) => config.Tweex.BeeHouseAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Quality_Mushroomboxagingfactor_Title,
                I18n.Gmcm_Twx_Quality_Mushroomboxagingfactor_Desc,
                config => config.Tweex.MushroomBoxAgingFactor,
                (config, value) => config.Tweex.MushroomBoxAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Quality_Teabushagingfactor_Title,
                I18n.Gmcm_Twx_Quality_Teabushagingfactor_Desc,
                config => config.Tweex.TeaBushAgingFactor,
                (config, value) => config.Tweex.TeaBushAgingFactor = value,
                0.1f,
                2f)
            .AddCheckbox(
                I18n.Gmcm_Twx_Quality_Deterministicagequality_Title,
                I18n.Gmcm_Twx_Quality_Deterministicagequality_Desc,
                config => config.Tweex.DeterministicAgeQuality,
                (config, value) => config.Tweex.DeterministicAgeQuality = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Experience_Heading)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_Berrybushexpreward_Title,
                I18n.Gmcm_Twx_Experience_Berrybushexpreward_Desc,
                config => (int)config.Tweex.BerryBushExpReward,
                (config, value) => config.Tweex.BerryBushExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_Mushroomboxexpreward_Title,
                I18n.Gmcm_Twx_Experience_Mushroomboxexpreward_Desc,
                config => (int)config.Tweex.MushroomBoxExpReward,
                (config, value) => config.Tweex.MushroomBoxExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_Tapperexpreward_Title,
                I18n.Gmcm_Twx_Experience_Tapperexpreward_Desc,
                config => (int)config.Tweex.TapperExpReward,
                (config, value) => config.Tweex.TapperExpReward = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Gameplay_Heading)
            .AddNumberField(
                I18n.Gmcm_Twx_Gameplay_Cropwitherchance_Title,
                I18n.Gmcm_Twx_Gameplay_Cropwitherchance_Desc,
                config => config.Tweex.CropWitherChance,
                (config, value) => config.Tweex.CropWitherChance = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Twx_Gameplay_Preventfruittreewintergrowth_Title,
                I18n.Gmcm_Twx_Gameplay_Preventfruittreewintergrowth_Desc,
                config => config.Tweex.PreventFruitTreeWinterGrowth,
                (config, value) => config.Tweex.PreventFruitTreeWinterGrowth = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Gameplay_Largedairyyieldsquantityoverquality_Title,
                I18n.Gmcm_Twx_Gameplay_Largedairyyieldsquantityoverquality_Desc,
                config => config.Tweex.LargeDairyYieldsQuantityOverQuality,
                (config, value) => config.Tweex.LargeDairyYieldsQuantityOverQuality = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Gameplay_Trashdoesnotconsumebait_Title,
                I18n.Gmcm_Twx_Gameplay_Trashdoesnotconsumebait_Desc,
                config => config.Tweex.TrashDoesNotConsumeBait,
                (config, value) => config.Tweex.TrashDoesNotConsumeBait = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Gameplay_Explosiontriggeredbombs_Title,
                I18n.Gmcm_Twx_Gameplay_Explosiontriggeredbombs_Desc,
                config => config.Tweex.ExplosionTriggeredBombs,
                (config, value) => config.Tweex.ExplosionTriggeredBombs = value);

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
            I18n.Gmcm_Twx_Gameplay_Spawncrowsonthesemaps_Title,
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
