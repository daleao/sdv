namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderingHud;
using DaLion.Overhaul.Modules.Professions.Events.Player.Warped;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for PROFS.</summary>
    private void AddProfessionOptions()
    {
        this
            .AddPage(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Heading)

            // professions
            .AddSectionTitle(I18n.Gmcm_Profs_General_Heading)
            .AddCheckbox(
                I18n.Gmcm_Profs_ShouldJunimosInheritProfessions_Title,
                I18n.Gmcm_Profs_ShouldJunimosInheritProfessions_Desc,
                config => config.Professions.ShouldJunimosInheritProfessions,
                (config, value) => config.Professions.ShouldJunimosInheritProfessions = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_LaxOwnershipRequirements_Title,
                I18n.Gmcm_Profs_LaxOwnershipRequirements_Desc,
                config => config.Professions.LaxOwnershipRequirements,
                (config, value) => config.Professions.LaxOwnershipRequirements = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_ArtisanGoodsAlwaysInputQuality_Title,
                I18n.Gmcm_Profs_ArtisanGoodsAlwaysInputQuality_Desc,
                config => config.Professions.ArtisanGoodsAlwaysInputQuality,
                (config, value) => config.Professions.ArtisanGoodsAlwaysInputQuality = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_BeesAreAnimals_Title,
                I18n.Gmcm_Profs_BeesAreAnimals_Desc,
                config => config.Professions.BeesAreAnimals,
                (config, value) => config.Professions.BeesAreAnimals = value)
            .AddNumberField(
                I18n.Gmcm_Profs_ForagesNeededForBestQuality_Title,
                I18n.Gmcm_Profs_ForagesNeededForBestQuality_Desc,
                config => (int)config.Professions.ForagesNeededForBestQuality,
                (config, value) => config.Professions.ForagesNeededForBestQuality = (uint)value,
                0,
                1000,
                10)
            .AddNumberField(
                I18n.Gmcm_Profs_MineralsNeededForBestQuality_Title,
                I18n.Gmcm_Profs_MineralsNeededForBestQuality_Desc,
                config => (int)config.Professions.MineralsNeededForBestQuality,
                (config, value) => config.Professions.MineralsNeededForBestQuality = (uint)value,
                0,
                1000,
                10)
            .AddCheckbox(
                I18n.Gmcm_Profs_CrystalariumUpgradesWithGemologist_Title,
                I18n.Gmcm_Profs_CrystalariumUpgradesWithGemologist_Desc,
                config => config.Professions.CrystalariumUpgradesWithGemologist,
                (config, value) => config.Professions.CrystalariumUpgradesWithGemologist = value)
            .AddNumberField(
                I18n.Gmcm_Profs_ChanceToStartTreasureHunt_Title,
                I18n.Gmcm_Profs_ChanceToStartTreasureHunt_Desc,
                config => (float)config.Professions.ChanceToStartTreasureHunt,
                (config, value) => config.Professions.ChanceToStartTreasureHunt = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Profs_AllowScavengerHuntsOnFarm_Title,
                I18n.Gmcm_Profs_AllowScavengerHuntsOnFarm_Desc,
                config => config.Professions.AllowScavengerHuntsOnFarm,
                (config, value) => config.Professions.AllowScavengerHuntsOnFarm = value)
            .AddNumberField(
                I18n.Gmcm_Profs_ScavengerHuntHandicap_Title,
                I18n.Gmcm_Profs_ScavengerHuntHandicap_Desc,
                config => config.Professions.ScavengerHuntHandicap,
                (config, value) => config.Professions.ScavengerHuntHandicap = value,
                1f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_ProspectorHuntHandicap_Title,
                I18n.Gmcm_Profs_ProspectorHuntHandicap_Desc,
                config => config.Professions.ProspectorHuntHandicap,
                (config, value) => config.Professions.ProspectorHuntHandicap = value,
                1f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_TreasureDetectionDistance_Title,
                I18n.Gmcm_Profs_TreasureDetectionDistance_Desc,
                config => config.Professions.TreasureDetectionDistance,
                (config, value) => config.Professions.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                I18n.Gmcm_Profs_SpelunkerSpeedCeiling_Title,
                I18n.Gmcm_Profs_SpelunkerSpeedCeiling_Desc,
                config => (int)config.Professions.SpelunkerSpeedCeiling,
                (config, value) => config.Professions.SpelunkerSpeedCeiling = (uint)value,
                0,
                10)
            .AddCheckbox(
                I18n.Gmcm_Profs_EnableGetExcited_Title,
                I18n.Gmcm_Profs_EnableGetExcited_Desc,
                config => config.Professions.DemolitionistGetExcited,
                (config, value) => config.Professions.DemolitionistGetExcited = value)
            .AddNumberField(
                I18n.Gmcm_Profs_AnglerPriceBonusCeiling_Title,
                I18n.Gmcm_Profs_AnglerPriceBonusCeiling_Desc,
                config => config.Professions.AnglerPriceBonusCeiling,
                (config, value) => config.Professions.AnglerPriceBonusCeiling = value,
                0.5f,
                2f,
                0.25f)
            .AddNumberField(
                I18n.Gmcm_Profs_AquaristFishPondCeiling_Title,
                I18n.Gmcm_Profs_AquaristFishPondCeiling_Desc,
                config => config.Professions.AquaristFishPondCeiling,
                (config, value) => config.Professions.AquaristFishPondCeiling = value,
                0,
                24)
            .AddNumberField(
                I18n.Gmcm_Profs_TrashPerTaxDeduction_Title,
                I18n.Gmcm_Profs_TrashPerTaxDeduction_Desc,
                config => (int)config.Professions.TrashNeededPerTaxDeduction,
                (config, value) => config.Professions.TrashNeededPerTaxDeduction = (uint)value,
                10,
                1000)
            .AddNumberField(
                I18n.Gmcm_Profs_TrashPerFriendshipPoint_Title,
                I18n.Gmcm_Profs_TrashPerFriendshipPoint_Desc,
                config => (int)config.Professions.TrashNeededPerFriendshipPoint,
                (config, value) => config.Professions.TrashNeededPerFriendshipPoint = (uint)value,
                10,
                1000)
            .AddNumberField(
                I18n.Gmcm_Profs_TaxDeductionCeiling_Title,
                I18n.Gmcm_Profs_TaxDeductionCeiling_Desc,
                config => config.Professions.ConservationistTaxDeductionCeiling,
                (config, value) => config.Professions.ConservationistTaxDeductionCeiling = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Profs_PiperBuffCeiling_Title,
                I18n.Gmcm_Profs_PiperBuffCeiling_Desc,
                config => (int)config.Professions.PiperBuffCeiling,
                (config, value) => config.Professions.PiperBuffCeiling = (uint)value,
                10,
                1000)
            .AddHorizontalRule()

            // page links
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Cmbt_Items_Title,
                pages: new[] { "Limit", "Prestige", "Experience", "ControlsAndUi" },
                getPageId: page => OverhaulModule.Professions.Namespace + $"/{page}",
                getPageName: page => page == "ControlsAndUi" ? I18n.Gmcm_Headings_ControlsAndUi() : _I18n.Get("gmcm.profs." + page.ToLowerInvariant() + ".heading"),
                getColumnsFromWidth: _ => 2)

        #region limit break

            .AddPage(OverhaulModule.Professions.Namespace + "/Limit", I18n.Gmcm_Profs_Limit_Heading)
            .AddPageLink(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Profs_Limit_Enable_Title,
                I18n.Gmcm_Profs_Limit_Enable_Desc,
                config => config.Professions.EnableLimitBreaks,
                (config, value) =>
                {
                    config.Professions.EnableLimitBreaks = value;
                    if (!value && Game1.player.Get_Ultimate() is { } ultimate)
                    {
                        ultimate.ChargeValue = 0d;
                        EventManager.DisableWithAttribute<UltimateEventAttribute>();
                    }
                    else if (value && Game1.player.Get_Ultimate() is not null)
                    {
                        Game1.player.RevalidateUltimate();
                        EventManager.Enable<UltimateWarpedEvent>();
                        if (Game1.currentLocation.IsDungeon())
                        {
                            EventManager.Enable<UltimateMeterRenderingHudEvent>();
                        }
                    }
                })
            .AddCheckbox(
                I18n.Gmcm_Profs_Limit_HoldToActivate_Title,
                I18n.Gmcm_Profs_Limit_HoldToActivate_Desc,
                config => config.Professions.HoldKeyToLimitBreak,
                (config, value) => config.Professions.HoldKeyToLimitBreak = value)
            .AddKeyBinding(
                I18n.Gmcm_Profs_Limit_ActivationKey_Title,
                I18n.Gmcm_Profs_Limit_ActivationKey_Desc,
                config => config.Professions.LimitBreakKey,
                (config, value) => config.Professions.LimitBreakKey = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_ActivationDelay_Title,
                I18n.Gmcm_Profs_Limit_ActivationDelay_Desc,
                config => config.Professions.LimitBreakHoldDelaySeconds,
                (config, value) => config.Professions.LimitBreakHoldDelaySeconds = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_GainFactor_Title,
                I18n.Gmcm_Profs_Limit_GainFactor_Desc,
                config => (float)config.Professions.LimitGainFactor,
                (config, value) => config.Professions.LimitGainFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_DrainFactor_Title,
                I18n.Gmcm_Profs_Limit_DrainFactor_Desc,
                config => (float)config.Professions.LimitDrainFactor,
                (config, value) => config.Professions.LimitDrainFactor = value,
                0.5f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_RespecCost_Title,
                I18n.Gmcm_Profs_Limit_RespecCost_Desc,
                config => (int)config.Professions.LimitRespecCost,
                (config, value) => config.Professions.LimitRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_GaugeXOffset_Title,
                I18n.Gmcm_Profs_Limit_GaugeOffset_Desc,
                config => config.Professions.LimitGaugeXOffset,
                (config, value) => config.Professions.LimitGaugeXOffset = value,
                -3000,
                0)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_GaugeYOffset_Title,
                I18n.Gmcm_Profs_Limit_GaugeOffset_Desc,
                config => config.Professions.LimitGaugeYOffset,
                (config, value) => config.Professions.LimitGaugeYOffset = value,
                -1500,
                0)

        #endregion limit break

        #region prestige

            .AddPage(OverhaulModule.Professions.Namespace + "/Prestige", I18n.Gmcm_Profs_Prestige_Heading)
            .AddPageLink(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_Enable_Title,
                I18n.Gmcm_Profs_Prestige_Enable_Desc,
                config => config.Professions.EnablePrestige,
                (config, value) => config.Professions.EnablePrestige = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_SkillResetCostMultiplier_Title,
                I18n.Gmcm_Profs_Prestige_SkillResetCostMultiplier_Desc,
                config => config.Professions.SkillResetCostMultiplier,
                (config, value) => config.Professions.SkillResetCostMultiplier = value,
                0f,
                3f,
                0.1f)
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_ForgetRecipes_Title,
                I18n.Gmcm_Profs_Prestige_ForgetRecipes_Desc,
                config => config.Professions.ForgetRecipesOnSkillReset,
                (config, value) => config.Professions.ForgetRecipesOnSkillReset = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_AllowMultiplePrestige_Title,
                I18n.Gmcm_Profs_Prestige_AllowMultiplePrestige_Desc,
                config => config.Professions.AllowMultipleResets,
                (config, value) => config.Professions.AllowMultipleResets = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_ExpMultiplier_Title,
                I18n.Gmcm_Profs_Prestige_ExpMultiplier_Desc,
                config => config.Professions.PrestigeExpMultiplier,
                (config, value) => config.Professions.PrestigeExpMultiplier = value,
                -0.5f,
                2f,
                0.1f)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_RespecCost_Title,
                I18n.Gmcm_Profs_Prestige_RespecCost_Desc,
                config => (int)config.Professions.PrestigeRespecCost,
                (config, value) => config.Professions.PrestigeRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddDropdown(
                I18n.Gmcm_Profs_Prestige_ProgressionStyle_Title,
                I18n.Gmcm_Profs_Prestige_ProgressionStyle_Desc,
                config => config.Professions.ProgressionStyle.ToString(),
                (config, value) =>
                {
                    config.Professions.ProgressionStyle = Enum.Parse<Config.PrestigeProgressionStyle>(value);
                    ModHelper.GameContent.InvalidateCache(
                        $"{Manifest.UniqueID}/PrestigeProgression");
                },
                new[] { "StackedStars", "Gen3Ribbons", "Gen4Ribbons" },
                value => _I18n.Get("gmcm.profs.prestige.progression_style." + value.ToLowerInvariant()))
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_RaisedLevelCap_Title,
                I18n.Gmcm_Profs_Prestige_RaisedLevelCap_Desc,
                config => config.Professions.EnableExtendedProgession,
                (config, value) => config.Professions.EnableExtendedProgession = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_RequiredExpPerExtendedLevel_Title,
                I18n.Gmcm_Profs_Prestige_RequiredExpPerExtendedLevel_Desc,
                config => (int)config.Professions.RequiredExpPerExtendedLevel,
                (config, value) => config.Professions.RequiredExpPerExtendedLevel = (uint)value,
                1000,
                10000,
                500)

        #endregion prestige

        #region experience

            .AddPage(OverhaulModule.Professions.Namespace + "/Experience", I18n.Gmcm_Profs_Experience_Heading)
            .AddPageLink(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Back)
            .AddVerticalSpace()
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title("Farming"),
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc("farming"),
                config => config.Professions.BaseSkillExpMultipliers[0],
                (config, value) => config.Professions.BaseSkillExpMultipliers[0] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title("Fishing"),
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc("fishing"),
                config => config.Professions.BaseSkillExpMultipliers[1],
                (config, value) => config.Professions.BaseSkillExpMultipliers[1] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title("Foraging"),
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc("foraging"),
                config => config.Professions.BaseSkillExpMultipliers[2],
                (config, value) => config.Professions.BaseSkillExpMultipliers[2] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title("Mining"),
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc("mining"),
                config => config.Professions.BaseSkillExpMultipliers[3],
                (config, value) => config.Professions.BaseSkillExpMultipliers[3] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title("Combat"),
                () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc("combat"),
                config => config.Professions.BaseSkillExpMultipliers[4],
                (config, value) => config.Professions.BaseSkillExpMultipliers[4] = value,
                0.2f,
                2f,
                0.1f);

        foreach (var (skillId, _) in ProfessionsModule.Config.CustomSkillExpMultipliers)
        {
            if (!SCSkill.Loaded.TryGetValue(skillId, out var skill))
            {
                continue;
            }

            this
                .AddNumberField(
                    () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Title(skill.DisplayName),
                    () => I18n.Gmcm_Profs_Experience_BaseExpMultiplier_Desc(skill.DisplayName.ToLowerInvariant()),
                    config => config.Professions.CustomSkillExpMultipliers[skillId],
                    (config, value) => config.Professions.CustomSkillExpMultipliers[skillId] = value,
                    0.2f,
                    2f,
                    0.1f);
        }

        #endregion experience

        #region controls & ui

        this
            .AddPage(OverhaulModule.Professions.Namespace + "/ControlsAndUi", I18n.Gmcm_Headings_ControlsAndUi)
            .AddPageLink(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Back)
            .AddVerticalSpace()

            // controls
            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddKeyBinding(
                I18n.Gmcm_Controls_Modkey_Title,
                I18n.Gmcm_Profs_Controls_Modkey_Desc,
                config => config.Professions.ModKey,
                (config, value) => config.Professions.ModKey = value)
            .AddHorizontalRule()

            // interface
            .AddSectionTitle(I18n.Gmcm_Ui_Heading)
            .AddCheckbox(
                I18n.Gmcm_Profs_Ui_ShowFishCollecionMaxIcon_Title,
                I18n.Gmcm_Profs_Ui_ShowFishCollecionMaxIcon_Desc,
                config => config.Professions.ShowFishCollectionMaxIcon,
                (config, value) => config.Professions.ShowFishCollectionMaxIcon = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Ui_TrackingPointerScale_Title,
                I18n.Gmcm_Profs_Ui_TrackingPointerScale_Desc,
                config => config.Professions.TrackingPointerScale,
                (config, value) =>
                {
                    config.Professions.TrackingPointerScale = value;
                    if (HudPointer.Instance.IsValueCreated)
                    {
                        HudPointer.Instance.Value.Scale = value;
                    }
                },
                0.2f,
                2f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Ui_TrackingPointerBobRate_Title,
                I18n.Gmcm_Profs_Ui_TrackingPointerBobRate_Desc,
                config => config.Professions.TrackingPointerBobRate,
                (config, value) =>
                {
                    config.Professions.TrackingPointerBobRate = value;
                    if (HudPointer.Instance.IsValueCreated)
                    {
                        HudPointer.Instance.Value.BobRate = value;
                    }
                },
                0.5f,
                2f,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Profs_Ui_DisableAlwaysTrack_Title,
                I18n.Gmcm_Profs_Ui_DisableAlwaysTrack_Desc,
                config => config.Professions.DisableAlwaysTrack,
                (config, value) => config.Professions.DisableAlwaysTrack = value);

        #endregion controls & ui
    }
}
