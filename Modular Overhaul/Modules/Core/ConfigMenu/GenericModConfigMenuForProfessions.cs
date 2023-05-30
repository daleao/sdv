namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Events.Display;
using DaLion.Overhaul.Modules.Professions.Events.Player;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for PROFS.</summary>
    private void AddProfessionOptions()
    {
        this
            .AddPage(OverhaulModule.Professions.Namespace, I18n.Gmcm_Profs_Heading)

            // controls and ui settings
            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddKeyBinding(
                I18n.Gmcm_Controls_Modkey_Title,
                I18n.Gmcm_Profs_Controls_Modkey_Desc,
                config => config.Professions.ModKey,
                (config, value) => config.Professions.ModKey = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Ui_Heading)
            .AddCheckbox(
                I18n.Gmcm_Profs_Ui_Showfishcollecionmaxicon_Title,
                I18n.Gmcm_Profs_Ui_Showfishcollecionmaxicon_Desc,
                config => config.Professions.ShowFishCollectionMaxIcon,
                (config, value) => config.Professions.ShowFishCollectionMaxIcon = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Ui_Trackingpointerscale_Title,
                I18n.Gmcm_Profs_Ui_Trackingpointerscale_Desc,
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
                I18n.Gmcm_Profs_Ui_Trackingpointerbobrate_Title,
                I18n.Gmcm_Profs_Ui_Trackingpointerbobrate_Desc,
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
                I18n.Gmcm_Profs_Ui_Disablealwaystrack_Title,
                I18n.Gmcm_Profs_Ui_Disablealwaystrack_Desc,
                config => config.Professions.DisableAlwaysTrack,
                (config, value) => config.Professions.DisableAlwaysTrack = value)
            .AddHorizontalRule()

            // professions
            .AddSectionTitle(I18n.Gmcm_Profs_Professions_Heading)
            .AddCheckbox(
                I18n.Gmcm_Profs_Shouldjunimosinheritprofessions_Title,
                I18n.Gmcm_Profs_Shouldjunimosinheritprofessions_Desc,
                config => config.Professions.ShouldJunimosInheritProfessions,
                (config, value) => config.Professions.ShouldJunimosInheritProfessions = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_Laxownershiprequirements_Title,
                I18n.Gmcm_Profs_Laxownershiprequirements_Desc,
                config => config.Professions.LaxOwnershipRequirements,
                (config, value) => config.Professions.LaxOwnershipRequirements = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_Artisangoodsalwaysinputquality_Title,
                I18n.Gmcm_Profs_Artisangoodsalwaysinputquality_Desc,
                config => config.Professions.ArtisanGoodsAlwaysInputQuality,
                (config, value) => config.Professions.ArtisanGoodsAlwaysInputQuality = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_Beesareanimals_Title,
                I18n.Gmcm_Profs_Beesareanimals_Desc,
                config => config.Professions.BeesAreAnimals,
                (config, value) => config.Professions.BeesAreAnimals = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Foragesneededforbestquality_Title,
                I18n.Gmcm_Profs_Foragesneededforbestquality_Desc,
                config => (int)config.Professions.ForagesNeededForBestQuality,
                (config, value) => config.Professions.ForagesNeededForBestQuality = (uint)value,
                0,
                1000,
                10)
            .AddNumberField(
                I18n.Gmcm_Profs_Mineralsneededforbestquality_Title,
                I18n.Gmcm_Profs_Mineralsneededforbestquality_Desc,
                config => (int)config.Professions.MineralsNeededForBestQuality,
                (config, value) => config.Professions.MineralsNeededForBestQuality = (uint)value,
                0,
                1000,
                10)
            .AddCheckbox(
                I18n.Gmcm_Profs_Crystalariumsupgradewithgemologist_Title,
                I18n.Gmcm_Profs_Crystalariumsupgradewithgemologist_Desc,
                config => config.Professions.CrystalariumsUpgradeWithGemologist,
                (config, value) => config.Professions.CrystalariumsUpgradeWithGemologist = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Chancetostarttreasurehunt_Title,
                I18n.Gmcm_Profs_Chancetostarttreasurehunt_Desc,
                config => (float)config.Professions.ChanceToStartTreasureHunt,
                (config, value) => config.Professions.ChanceToStartTreasureHunt = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                I18n.Gmcm_Profs_Allowscavengerhuntsonfarm_Title,
                I18n.Gmcm_Profs_Allowscavengerhuntsonfarm_Desc,
                config => config.Professions.AllowScavengerHuntsOnFarm,
                (config, value) => config.Professions.AllowScavengerHuntsOnFarm = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Scavengerhunthandicap_Title,
                I18n.Gmcm_Profs_Scavengerhunthandicap_Desc,
                config => config.Professions.ScavengerHuntHandicap,
                (config, value) => config.Professions.ScavengerHuntHandicap = value,
                1f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Prospectorhunthandicap_Title,
                I18n.Gmcm_Profs_Prospectorhunthandicap_Desc,
                config => config.Professions.ProspectorHuntHandicap,
                (config, value) => config.Professions.ProspectorHuntHandicap = value,
                1f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Treasuredetectiondistance_Title,
                I18n.Gmcm_Profs_Treasuredetectiondistance_Desc,
                config => config.Professions.TreasureDetectionDistance,
                (config, value) => config.Professions.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                I18n.Gmcm_Profs_Spelunkerspeedceiling_Title,
                I18n.Gmcm_Profs_Spelunkerspeedceiling_Desc,
                config => (int)config.Professions.SpelunkerSpeedCeiling,
                (config, value) => config.Professions.SpelunkerSpeedCeiling = (uint)value,
                1,
                10)
            .AddCheckbox(
                I18n.Gmcm_Profs_Enablegetexcited_Title,
                I18n.Gmcm_Profs_Enablegetexcited_Desc,
                config => config.Professions.EnableGetExcited,
                (config, value) => config.Professions.EnableGetExcited = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Anglerpricebonusceiling_Title,
                I18n.Gmcm_Profs_Anglerpricebonusceiling_Desc,
                config => config.Professions.AnglerPriceBonusCeiling,
                (config, value) => config.Professions.AnglerPriceBonusCeiling = value,
                0.5f,
                2f,
                0.25f)
            .AddNumberField(
                I18n.Gmcm_Profs_Aquaristfishpondceiling_Title,
                I18n.Gmcm_Profs_Aquaristfishpondceiling_Desc,
                config => config.Professions.AquaristFishPondCeiling,
                (config, value) => config.Professions.AquaristFishPondCeiling = value,
                0,
                24)
            .AddNumberField(
                I18n.Gmcm_Profs_Legendarypondpopulationceiling_Title,
                I18n.Gmcm_Profs_Legendarypondpopulationceiling_Desc,
                config => (int)config.Professions.LegendaryPondPopulationCeiling,
                (config, value) =>
                {
                    config.Professions.LegendaryPondPopulationCeiling = (uint)value;
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    var buildings = Game1.getFarm().buildings;
                    for (var i = 0; i < buildings.Count; i++)
                    {
                        var building = buildings[i];
                        if (building is FishPond pond &&
                            (pond.IsOwnedBy(Game1.player) || config.Professions.LaxOwnershipRequirements) &&
                            !pond.isUnderConstruction())
                        {
                            pond.UpdateMaximumOccupancy();
                        }
                    }
                },
                4,
                12)
            .AddNumberField(
                I18n.Gmcm_Profs_Trashpertaxdeduction_Title,
                I18n.Gmcm_Profs_Trashpertaxdeduction_Desc,
                config => (int)config.Professions.TrashNeededPerTaxDeductionPct,
                (config, value) => config.Professions.TrashNeededPerTaxDeductionPct = (uint)value,
                10,
                1000)
            .AddNumberField(
                I18n.Gmcm_Profs_Trashperfriendshippoint_Title,
                I18n.Gmcm_Profs_Trashperfriendshippoint_Desc,
                config => (int)config.Professions.TrashNeededPerFriendshipPoint,
                (config, value) => config.Professions.TrashNeededPerFriendshipPoint = (uint)value,
                10,
                1000)
            .AddNumberField(
                I18n.Gmcm_Profs_Taxdeductionceiling_Title,
                I18n.Gmcm_Profs_Taxdeductionceiling_Desc,
                config => config.Professions.ConservationistTaxBonusCeiling,
                (config, value) => config.Professions.ConservationistTaxBonusCeiling = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Profs_Piperbuffceiling_Title,
                I18n.Gmcm_Profs_Piperbuffceiling_Desc,
                config => (int)config.Professions.PiperBuffCeiling,
                (config, value) => config.Professions.PiperBuffCeiling = (uint)value,
                10,
                1000)
            .AddHorizontalRule()

            // limit breaks
            .AddSectionTitle(I18n.Gmcm_Profs_Limit_Heading)
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
                I18n.Gmcm_Profs_Limit_Holdtoactivate_Title,
                I18n.Gmcm_Profs_Limit_Holdtoactivate_Desc,
                config => config.Professions.HoldKeyToLimitBreak,
                (config, value) => config.Professions.HoldKeyToLimitBreak = value)
            .AddKeyBinding(
                I18n.Gmcm_Profs_Limit_Activationkey_Title,
                I18n.Gmcm_Profs_Limit_Activationkey_Desc,
                config => config.Professions.LimitBreakKey,
                (config, value) => config.Professions.LimitBreakKey = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Activationdelay_Title,
                I18n.Gmcm_Profs_Limit_Activationdelay_Desc,
                config => config.Professions.LimitBreakHoldDelaySeconds,
                (config, value) => config.Professions.LimitBreakHoldDelaySeconds = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Gainfactor_Title,
                I18n.Gmcm_Profs_Limit_Gainfactor_Desc,
                config => (float)config.Professions.LimitGainFactor,
                (config, value) => config.Professions.LimitGainFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Drainfactor_Title,
                I18n.Gmcm_Profs_Limit_Drainfactor_Desc,
                config => (float)config.Professions.LimitDrainFactor,
                (config, value) => config.Professions.LimitDrainFactor = value,
                0.5f,
                2f)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Respectcost_Title,
                I18n.Gmcm_Profs_Limit_Respectcost_Desc,
                config => (int)config.Professions.LimitRespecCost,
                (config, value) => config.Professions.LimitRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Gaugexoffset_Title,
                I18n.Gmcm_Profs_Limit_Gaugeoffset_Desc,
                config => config.Professions.LimitGaugeXOffset,
                (config, value) => config.Professions.LimitGaugeXOffset = value,
                -3000,
                0)
            .AddNumberField(
                I18n.Gmcm_Profs_Limit_Gaugeyoffset_Title,
                I18n.Gmcm_Profs_Limit_Gaugeoffset_Desc,
                config => config.Professions.LimitGaugeYOffset,
                (config, value) => config.Professions.LimitGaugeYOffset = value,
                -1500,
                0)
            .AddHorizontalRule()

            // prestige
            .AddSectionTitle(I18n.Gmcm_Profs_Prestige_Heading)
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_Enable_Title,
                I18n.Gmcm_Profs_Prestige_Enable_Desc,
                config => config.Professions.EnablePrestige,
                (config, value) => config.Professions.EnablePrestige = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_Skillresetcostmultiplier_Title,
                I18n.Gmcm_Profs_Prestige_Skillresetcostmultiplier_Desc,
                config => config.Professions.SkillResetCostMultiplier,
                (config, value) => config.Professions.SkillResetCostMultiplier = value,
                0f,
                3f,
                0.1f)
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_Forgetrecipes_Title,
                I18n.Gmcm_Profs_Prestige_Forgetrecipes_Desc,
                config => config.Professions.ForgetRecipes,
                (config, value) => config.Professions.ForgetRecipes = value)
            .AddCheckbox(
                I18n.Gmcm_Profs_Prestige_Allowmultipleprestige_Title,
                I18n.Gmcm_Profs_Prestige_Allowmultipleprestige_Desc,
                config => config.Professions.AllowMultiplePrestige,
                (config, value) => config.Professions.AllowMultiplePrestige = value)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_Expmultiplier_Title,
                I18n.Gmcm_Profs_Prestige_Expmultiplier_Desc,
                config => config.Professions.PrestigeExpMultiplier,
                (config, value) => config.Professions.PrestigeExpMultiplier = value,
                -0.5f,
                2f,
                0.1f)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_Requiredexpperextendedlevel_Title,
                I18n.Gmcm_Profs_Prestige_Requiredexpperextendedlevel_Desc,
                config => (int)config.Professions.RequiredExpPerExtendedLevel,
                (config, value) => config.Professions.RequiredExpPerExtendedLevel = (uint)value,
                1000,
                10000,
                500)
            .AddNumberField(
                I18n.Gmcm_Profs_Prestige_Respeccost_Title,
                I18n.Gmcm_Profs_Prestige_Respeccost_Desc,
                config => (int)config.Professions.PrestigeRespecCost,
                (config, value) => config.Professions.PrestigeRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddDropdown(
                I18n.Gmcm_Profs_Prestige_Progressionstyle_Title,
                I18n.Gmcm_Profs_Prestige_Progressionstyle_Desc,
                config => config.Professions.ProgressionStyle.ToString(),
                (config, value) =>
                {
                    config.Professions.ProgressionStyle = Enum.Parse<Config.PrestigeProgressionStyle>(value);
                    ModHelper.GameContent.InvalidateCache(
                        $"{Manifest.UniqueID}/PrestigeProgression");
                },
                new[] { "StackedStars", "Gen3Ribbons", "Gen4Ribbons" },
                value => _I18n.Get("gmcm.profs.prestige.progressionstyle." + value.ToLowerInvariant()))
            .AddHorizontalRule()

            // experience settings
            .AddSectionTitle(I18n.Gmcm_Profs_Experience_Heading)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title("Farming"),
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc("farming"),
                config => config.Professions.BaseSkillExpMultipliers[0],
                (config, value) => config.Professions.BaseSkillExpMultipliers[0] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title("Fishing"),
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc("fishing"),
                config => config.Professions.BaseSkillExpMultipliers[1],
                (config, value) => config.Professions.BaseSkillExpMultipliers[1] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title("Foraging"),
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc("foraging"),
                config => config.Professions.BaseSkillExpMultipliers[2],
                (config, value) => config.Professions.BaseSkillExpMultipliers[2] = value,
                0.2f,
                2f,
                0.1f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title("Mining"),
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc("mining"),
                config => config.Professions.BaseSkillExpMultipliers[3],
                (config, value) => config.Professions.BaseSkillExpMultipliers[3] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title("Combat"),
                () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc("combat"),
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
                    () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Title(skill.DisplayName),
                    () => I18n.Gmcm_Profs_Experience_Baseexpmultiplier_Desc(skill.DisplayName.ToLowerInvariant()),
                    config => config.Professions.CustomSkillExpMultipliers[skillId],
                    (config, value) => config.Professions.CustomSkillExpMultipliers[skillId] = value,
                    0.2f,
                    2f,
                    0.1f);
        }
    }
}
