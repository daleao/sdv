namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.SMAPI;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for TWX.</summary>
    private void AddMiscOptions()
    {
        this
            .AddPage(OverhaulModule.Tweex.Namespace, I18n.Gmcm_Twx_Heading)

            .AddSectionTitle(I18n.Gmcm_Twx_Headings_Farming)
            .AddNumberField(
                I18n.Gmcm_Twx_Farming_CropWitherChance_Title,
                I18n.Gmcm_Twx_Farming_CropWitherChance_Desc,
                config => config.Tweex.CropWitherChance,
                (config, value) => config.Tweex.CropWitherChance = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Twx_Farming_PreventFruitTreeWinterGrowth_Title,
                I18n.Gmcm_Twx_Farming_PreventFruitTreeWinterGrowth_Desc,
                config => config.Tweex.PreventFruitTreeWinterGrowth,
                (config, value) => config.Tweex.PreventFruitTreeWinterGrowth = value)
            .AddNumberField(
                I18n.Gmcm_Twx_Aging_BeeHouse_Title,
                I18n.Gmcm_Twx_Aging_BeeHouse_Desc,
                config => config.Tweex.BeeHouseAgingFactor,
                (config, value) => config.Tweex.BeeHouseAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Aging_TeaBush_Title,
                I18n.Gmcm_Twx_Aging_TeaBush_Desc,
                config => config.Tweex.TeaBushAgingFactor,
                (config, value) => config.Tweex.TeaBushAgingFactor = value,
                0.1f,
                2f)
            .AddCheckbox(
                I18n.Gmcm_Twx_Farming_LargedairyYield_Title,
                I18n.Gmcm_Twx_Farming_LargedairyYield_Desc,
                config => config.Tweex.LargeDairyYieldsQuantityOverQuality,
                (config, value) => config.Tweex.LargeDairyYieldsQuantityOverQuality = value);

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
                I18n.Gmcm_Twx_Farming_SpawnCrows_Title,
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
                map => _I18n.Get("gmcm.twx.maps." + map.ToLowerInvariant()))
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Headings_Foraging)
            .AddNumberField(
                I18n.Gmcm_Twx_Aging_Tree_Title,
                I18n.Gmcm_Twx_Aging_Tree_Desc,
                config => config.Tweex.TreeAgingFactor,
                (config, value) => config.Tweex.TreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Aging_FruitTree_Title,
                I18n.Gmcm_Twx_Aging_FruitTree_Desc,
                config => config.Tweex.FruitTreeAgingFactor,
                (config, value) => config.Tweex.FruitTreeAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_BerryBush_Title,
                I18n.Gmcm_Twx_Experience_BerryBush_Desc,
                config => (int)config.Tweex.BerryBushExpReward,
                (config, value) => config.Tweex.BerryBushExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Aging_MushroomBox_Title,
                I18n.Gmcm_Twx_Aging_MushroomBox_Desc,
                config => config.Tweex.MushroomBoxAgingFactor,
                (config, value) => config.Tweex.MushroomBoxAgingFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_MushroomBox_Title,
                I18n.Gmcm_Twx_Experience_MushroomBox_Desc,
                config => (int)config.Tweex.MushroomBoxExpReward,
                (config, value) => config.Tweex.MushroomBoxExpReward = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Twx_Experience_Tapper_Title,
                I18n.Gmcm_Twx_Experience_Tapper_Desc,
                config => (int)config.Tweex.TapperExpReward,
                (config, value) => config.Tweex.TapperExpReward = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Headings_Fishing)
            .AddCheckbox(
                I18n.Gmcm_Twx_Fishing_TrashDoesNotConsumeBait_Title,
                I18n.Gmcm_Twx_Fishing_TrashDoesNotConsumeBait_Desc,
                config => config.Tweex.TrashDoesNotConsumeBait,
                (config, value) => config.Tweex.TrashDoesNotConsumeBait = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Headings_Mining)
            .AddCheckbox(
                I18n.Gmcm_Twx_Mining_ExplosionTriggeredBombs_Title,
                I18n.Gmcm_Twx_Mining_ExplosionTriggeredBombs_Desc,
                config => config.Tweex.ExplosionTriggeredBombs,
                (config, value) => config.Tweex.ExplosionTriggeredBombs = value)
            .AddCheckbox(
                I18n.Gmcm_Twx_Mining_ImmersiveGlowstoneProgression_Title,
                I18n.Gmcm_Twx_Mining_ImmersiveGlowstoneProgression_Desc,
                config => config.Tweex.ImmersiveGlowstoneProgression,
                (config, value) =>
                {
                    config.Tweex.ImmersiveGlowstoneProgression = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Twx_Headings_General)
            .AddCheckbox(
                I18n.Gmcm_Twx_Aging_Deterministic_Title,
                I18n.Gmcm_Twx_Aging_Deterministic_Desc,
                config => config.Tweex.DeterministicAgeQuality,
                (config, value) => config.Tweex.DeterministicAgeQuality = value);
    }
}
