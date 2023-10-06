namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Tools;
using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Integrations.GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for TOLS.</summary>
    private void AddToolOptions()
    {
        var allowedUpgrades = new[] { "Copper", "Steel", "Gold", "Iridium" };

        var maxToolUpgrade = MoonMisadventuresIntegration.Instance?.IsLoaded == true
            ? 7
            : ToolsModule.Config.EnableForgeUpgrading
                ? 6
                : 5;
        if (maxToolUpgrade > 5)
        {
            allowedUpgrades.AddToArray("Radioactive");
        }

        if (maxToolUpgrade > 6)
        {
            allowedUpgrades.AddToArray("Mythicite");
        }

        allowedUpgrades.AddToArray("Reaching");

        this
            .AddPage(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Heading)

            // general
            .AddSectionTitle(I18n.Gmcm_Headings_General)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Playanimation_Title,
                I18n.Gmcm_Tols_Shockwave_Playanimation_Desc,
                config => config.Tools.PlayShockwaveAnimation,
                (config, value) => config.Tools.PlayShockwaveAnimation = value)
            .AddNumberField(
                I18n.Gmcm_Tols_Shockwave_TicksBetweenCrests_Title,
                I18n.Gmcm_Tols_Shockwave_TicksBetweenCrests_Desc,
                config => (int)config.Tools.TicksBetweenCrests,
                (config, value) => config.Tools.TicksBetweenCrests = (uint)value,
                0,
                10)
            .AddCheckbox(
                I18n.Gmcm_Tols_EnableForgeUpgrading_Title,
                I18n.Gmcm_Tols_EnableForgeUpgrading_Desc,
                config => config.Tools.EnableForgeUpgrading,
                (config, value) => config.Tools.EnableForgeUpgrading = value)
            .AddHorizontalRule()

            // controls
            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Charging_Hold_Title,
                I18n.Gmcm_Tols_Charging_Hold_Desc,
                config => config.Tools.HoldToCharge,
                (config, value) => config.Tools.HoldToCharge = value)
            .AddKeyBinding(
                I18n.Gmcm_Tols_Charging_Key_Title,
                I18n.Gmcm_Tols_Charging_Key_Desc,
                config => config.Tools.ChargeKey,
                (config, value) => config.Tools.ChargeKey = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_FaceMouseCursor_Title,
                I18n.Gmcm_Controls_FaceMouseCursor_Desc,
                config => config.Tools.FaceMouseCursor,
                (config, value) => config.Tools.FaceMouseCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_EnableAutoSelection_Title,
                () => I18n.Gmcm_Controls_EnableAutoSelection_Desc(),
                config => config.Tools.EnableAutoSelection,
                (config, value) =>
                {
                    config.Tools.EnableAutoSelection = value;
                    if (!value)
                    {
                        ToolsModule.State.SelectableToolByType.Clear();
                    }
                })
            .AddKeyBinding(
                I18n.Gmcm_Controls_SelectionKey_Title,
                () => I18n.Gmcm_Controls_SelectionKey_Desc(I18n.Gmcm_Tols_Tools().ToLowerInvariant()),
                config => config.Tools.SelectionKey,
                (config, value) => config.Tools.SelectionKey = value)
            .AddColorPicker(
                I18n.Gmcm_Controls_SelectionBorderColor_Title,
                () => I18n.Gmcm_Controls_SelectionBorderColor_Desc(I18n.Gmcm_Tols_Tools().ToLowerInvariant()),
                config => config.Tools.SelectionBorderColor,
                (config, value) => config.Tools.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddHorizontalRule()

            // interface
            .AddSectionTitle(I18n.Gmcm_Ui_Heading)
            .AddCheckbox(
                I18n.Gmcm_Ui_ColorCoded_Title,
                I18n.Gmcm_Tols_Ui_ColorCoded_Desc,
                config => config.Tools.ColorCodedForYourConvenience,
                (config, value) => config.Tools.ColorCodedForYourConvenience = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Ui_HideAffectedTiles_Title,
                I18n.Gmcm_Tols_Ui_HideAffectedTiles_Desc,
                config => config.Tools.HideAffectedTiles,
                (config, value) => config.Tools.HideAffectedTiles = value)
            .AddHorizontalRule()

            // page links
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Tols_Specific_Title,
                pages: new[] { "Axe", "Pickaxe", "Hoe", "WateringCan", "Scythe" },
                getPageId: tool => OverhaulModule.Tools.Namespace + $"/{tool}",
                getPageName: tool => _I18n.Get("gmcm.tols." + tool.ToLowerInvariant()) + I18n.Gmcm_Headings_Blank(),
                getColumnsFromWidth: _ => 2)

        #region axe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Axe", () => I18n.Gmcm_Tols_Axe() + I18n.Gmcm_Headings_Blank())
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Title,
                () => I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.BaseStaminaCostMultiplier,
                (config, value) => config.Tools.Axe.BaseStaminaCostMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Charging_Heading)
            .AddCheckbox(
                () => I18n.Gmcm_Tols_Charging_Enable_Title(I18n.Gmcm_Tols_Axe()),
                () => I18n.Gmcm_Tols_Charging_Enable_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.EnableCharging,
                (config, value) => config.Tools.Axe.EnableCharging = value)
            .AddDropdown(
                I18n.Gmcm_Tols_Charging_ReqUpgrade_Title,
                () => I18n.Gmcm_Tols_Charging_ReqUpgrade_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Axe.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => _I18n.Get("gmcm.tols.upgrades." + value.ToLowerInvariant()))
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_ChargedStaminaCostMultiplier_Title,
                I18n.Gmcm_Tols_Charging_ChargedStaminaCostMultiplier_Desc,
                config => config.Tools.Axe.ChargedStaminaCostMultiplier,
                (config, value) => config.Tools.Axe.ChargedStaminaCostMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_CopperRadius_Title,
                I18n.Gmcm_Tols_Charging_CopperRadius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10,
                1)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_SteelRadius_Title,
                I18n.Gmcm_Tols_Charging_SteelRadius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_GoldRadius_Title,
                I18n.Gmcm_Tols_Charging_GoldRadius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_IridiumRadius_Title,
                I18n.Gmcm_Tols_Charging_IridiumRadius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Title,
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Desc,
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 6)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Title,
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Desc,
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_EnchantedRadius_Title,
                () => I18n.Gmcm_Tols_Charging_EnchantedRadius_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[^1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[^1] = (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Shockwave_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearFruitTreeSeeds_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearFruitTreeSeeds_Desc,
                config => config.Tools.Axe.ClearFruitTreeSeeds,
                (config, value) => config.Tools.Axe.ClearFruitTreeSeeds = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearFruitTreeSaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearFruitTreeSaplings_Desc,
                config => config.Tools.Axe.ClearFruitTreeSaplings,
                (config, value) => config.Tools.Axe.ClearFruitTreeSaplings = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_CutGrownFruitTrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_CutGrownFruitTrees_Desc,
                config => config.Tools.Axe.CutGrownFruitTrees,
                (config, value) => config.Tools.Axe.CutGrownFruitTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSeeds_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSeeds_Desc,
                config => config.Tools.Axe.ClearTreeSeeds,
                (config, value) => config.Tools.Axe.ClearTreeSeeds = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSaplings_Desc,
                config => config.Tools.Axe.ClearTreeSaplings,
                (config, value) => config.Tools.Axe.ClearTreeSaplings = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_CutGrownTrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_CutGrownTrees_Desc,
                config => config.Tools.Axe.CutGrownTrees,
                (config, value) => config.Tools.Axe.CutGrownTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_CutTappedTrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_CutTappedTrees_Desc,
                config => config.Tools.Axe.CutTappedTrees,
                (config, value) => config.Tools.Axe.CutTappedTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_CutGiantCrops_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_CutGiantCrops_Desc,
                config => config.Tools.Axe.CutGiantCrops,
                (config, value) => config.Tools.Axe.CutGiantCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearBushes_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearBushes_Desc,
                config => config.Tools.Axe.ClearBushes,
                (config, value) => config.Tools.Axe.ClearBushes = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearLiveCrops_Title,
                I18n.Gmcm_Tols_Shockwave_ClearLiveCrops_Desc,
                config => config.Tools.Axe.ClearLiveCrops,
                (config, value) => config.Tools.Axe.ClearLiveCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearDeadCrops_Title,
                I18n.Gmcm_Tols_Shockwave_ClearDeadCrops_Desc,
                config => config.Tools.Axe.ClearDeadCrops,
                (config, value) => config.Tools.Axe.ClearDeadCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearDebris_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearDebris_Desc,
                config => config.Tools.Axe.ClearDebris,
                (config, value) => config.Tools.Axe.ClearDebris = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Enchantments_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Reaching_Title,
                () => I18n.Gmcm_Tols_Enchantments_Reaching_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.AllowReachingEnchantment,
                (config, value) => config.Tools.Axe.AllowReachingEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Master_Title,
                () => I18n.Gmcm_Tols_Enchantments_Master_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.AllowMasterEnchantment,
                (config, value) => config.Tools.Axe.AllowMasterEnchantment = value)
        #endregion axe settings

        #region piackaxe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Pickaxe", () => I18n.Gmcm_Tols_Pickaxe() + I18n.Gmcm_Headings_Blank())
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Title,
                () => I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.BaseStaminaCostMultiplier,
                (config, value) => config.Tools.Pick.BaseStaminaCostMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Charging_Heading)
            .AddCheckbox(
                () => I18n.Gmcm_Tols_Charging_Enable_Title(I18n.Gmcm_Tols_Pickaxe()),
                () => I18n.Gmcm_Tols_Charging_Enable_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.EnableCharging,
                (config, value) => config.Tools.Pick.EnableCharging = value)
            .AddDropdown(
                I18n.Gmcm_Tols_Charging_ReqUpgrade_Title,
                () => I18n.Gmcm_Tols_Charging_ReqUpgrade_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Pick.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Pick.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => _I18n.Get("gmcm.tols.upgrades." + value.ToLowerInvariant()))
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_ChargedStaminaCostMultiplier_Title,
                I18n.Gmcm_Tols_Charging_ChargedStaminaCostMultiplier_Desc,
                config => config.Tools.Pick.ChargedStaminaMultiplier,
                (config, value) => config.Tools.Pick.ChargedStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_CopperRadius_Title,
                I18n.Gmcm_Tols_Charging_CopperRadius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_SteelRadius_Title,
                I18n.Gmcm_Tols_Charging_SteelRadius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_GoldRadius_Title,
                I18n.Gmcm_Tols_Charging_GoldRadius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_IridiumRadius_Title,
                I18n.Gmcm_Tols_Charging_IridiumRadius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Title,
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Desc,
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 6)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Title,
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Desc,
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_EnchantedRadius_Title,
                () => I18n.Gmcm_Tols_Charging_EnchantedRadius_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[^1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[^1] = (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Shockwave_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_BreakBouldersAndMeteorites_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_BreakBouldersAndMeteorites_Desc,
                config => config.Tools.Pick.BreakBouldersAndMeteorites,
                (config, value) => config.Tools.Pick.BreakBouldersAndMeteorites = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_HarvestMineSpawns_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_HarvestMineSpawns_Desc,
                config => config.Tools.Pick.HarvestMineSpawns,
                (config, value) => config.Tools.Pick.HarvestMineSpawns = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_BreakMineContainers_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_BreakMineContainers_Desc,
                config => config.Tools.Pick.BreakMineContainers,
                (config, value) => config.Tools.Pick.BreakMineContainers = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_ClearObjects_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_ClearObjects_Desc,
                config => config.Tools.Pick.ClearObjects,
                (config, value) => config.Tools.Pick.ClearObjects = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_ClearFlooring_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_ClearFlooring_Desc,
                config => config.Tools.Pick.ClearFlooring,
                (config, value) => config.Tools.Pick.ClearFlooring = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_ClearDirt_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_ClearDirt_Desc,
                config => config.Tools.Pick.ClearDirt,
                (config, value) => config.Tools.Pick.ClearDirt = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearLiveCrops_Title,
                I18n.Gmcm_Tols_Shockwave_ClearLiveCrops_Desc,
                config => config.Tools.Pick.ClearLiveCrops,
                (config, value) => config.Tools.Pick.ClearLiveCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearDeadCrops_Title,
                I18n.Gmcm_Tols_Shockwave_ClearDeadCrops_Desc,
                config => config.Tools.Pick.ClearDeadCrops,
                (config, value) => config.Tools.Pick.ClearDeadCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_ClearDebris_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_ClearDebris_Desc,
                config => config.Tools.Pick.ClearDebris,
                (config, value) => config.Tools.Pick.ClearDebris = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Enchantments_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Reaching_Title,
                () => I18n.Gmcm_Tols_Enchantments_Reaching_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.AllowReachingEnchantment,
                (config, value) => config.Tools.Pick.AllowReachingEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Master_Title,
                () => I18n.Gmcm_Tols_Enchantments_Master_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.AllowMasterEnchantment,
                (config, value) => config.Tools.Pick.AllowMasterEnchantment = value)
        #endregion pickaxe settings

        #region hoe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Hoe", () => I18n.Gmcm_Tols_Hoe() + I18n.Gmcm_Headings_Blank())
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Title,
                () => I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                config => config.Tools.Hoe.BaseStaminaCostMultiplier,
                (config, value) => config.Tools.Hoe.BaseStaminaCostMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Tiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_CopperLength_Title,
                I18n.Gmcm_Tols_Tiles_CopperLength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_CopperRadius_Title,
                I18n.Gmcm_Tols_Tiles_CopperRadius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_SteelLength_Title,
                I18n.Gmcm_Tols_Tiles_SteelLength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_SteelRadius_Title,
                I18n.Gmcm_Tols_Tiles_SteelRadius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_GoldLength_Title,
                I18n.Gmcm_Tols_Tiles_GoldLength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_GoldRadius_Title,
                I18n.Gmcm_Tols_Tiles_GoldRadius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_IridiumLength_Title,
                I18n.Gmcm_Tols_Tiles_IridiumLength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_IridiumRadius_Title,
                I18n.Gmcm_Tols_Tiles_IridiumRadius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Tiles_RadioactiveLength_Title,
                    I18n.Gmcm_Tols_Tiles_RadioactiveLength_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Title,
                    I18n.Gmcm_Tols_Tiles_RadioactiveRadius_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Tiles_MythiciteLength_Title,
                    I18n.Gmcm_Tols_Tiles_MythiciteLength_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Title,
                    I18n.Gmcm_Tols_Tiles_MythiciteRadius_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddNumberField(
                    I18n.Gmcm_Tols_Tiles_EnchantedLength_Title,
                    () => I18n.Gmcm_Tols_Tiles_EnchantedLength_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Length = (uint)value,
                    1,
                    15)
            .AddNumberField(
                    I18n.Gmcm_Tols_Charging_EnchantedRadius_Title,
                    () => I18n.Gmcm_Tols_Tiles_EnchantedRadius_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Radius = (uint)value,
                    0,
                    7)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Enchantments_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Master_Title,
                () => I18n.Gmcm_Tols_Enchantments_Master_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                config => config.Tools.Hoe.AllowMasterEnchantment,
                (config, value) => config.Tools.Hoe.AllowMasterEnchantment = value)
        #endregion hoe settings

        #region watering can settings
            .AddPage(OverhaulModule.Tools.Namespace + "/WateringCan", () => I18n.Gmcm_Tols_Wateringcan() + I18n.Gmcm_Headings_Blank())
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Title,
                () => I18n.Gmcm_Tols_BaseStaminaCostMultiplier_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.BaseStaminaCostMultiplier,
                (config, value) => config.Tools.Can.BaseStaminaCostMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Tiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_CopperLength_Title,
                I18n.Gmcm_Tols_Tiles_CopperLength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_CopperRadius_Title,
                I18n.Gmcm_Tols_Tiles_CopperRadius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_SteelLength_Title,
                I18n.Gmcm_Tols_Tiles_SteelLength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_SteelRadius_Title,
                I18n.Gmcm_Tols_Tiles_SteelRadius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_GoldLength_Title,
                I18n.Gmcm_Tols_Tiles_GoldLength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_GoldRadius_Title,
                I18n.Gmcm_Tols_Tiles_GoldRadius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_IridiumLength_Title,
                I18n.Gmcm_Tols_Tiles_IridiumLength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_IridiumRadius_Title,
                I18n.Gmcm_Tols_Tiles_IridiumRadius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Tiles_RadioactiveLength_Title,
                    I18n.Gmcm_Tols_Tiles_RadioactiveLength_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_RadioactiveRadius_Title,
                    I18n.Gmcm_Tols_Tiles_RadioactiveRadius_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Tiles_MythiciteLength_Title,
                    I18n.Gmcm_Tols_Tiles_MythiciteLength_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_MythiciteRadius_Title,
                    I18n.Gmcm_Tols_Tiles_MythiciteRadius_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_EnchantedLength_Title,
                () => I18n.Gmcm_Tols_Tiles_EnchantedLength_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_EnchantedRadius_Title,
                () => I18n.Gmcm_Tols_Tiles_EnchantedRadius_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Radius = (uint)value,
                0,
                7)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Enchantments_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Master_Title,
                () => I18n.Gmcm_Tols_Enchantments_Master_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.AllowMasterEnchantment,
                (config, value) => config.Tools.Can.AllowMasterEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Swift_Title,
                () => I18n.Gmcm_Tols_Enchantments_Swift_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.AllowSwiftEnchantment,
                (config, value) => config.Tools.Can.AllowSwiftEnchantment = value)
            .AddNumberField(
                I18n.Gmcm_Tols_Can_ExpRewardChance_Title,
                I18n.Gmcm_Tols_Can_ExpRewardChance_Desc,
                config => config.Tools.Can.ExpRewardChance,
                (config, value) => config.Tools.Can.ExpRewardChance = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                I18n.Gmcm_Tols_Can_ExpRewardAmount_Title,
                I18n.Gmcm_Tols_Can_ExpRewardAmount_Desc,
                config => config.Tools.Can.ExpRewardAmount,
                (config, value) => config.Tools.Can.ExpRewardAmount = value,
                0,
                10)
            .AddCheckbox(
                I18n.Gmcm_Tols_Can_PreventSaltWaterRefill_Title,
                I18n.Gmcm_Tols_Can_PreventSaltWaterRefill_Desc,
                config => config.Tools.Can.PreventRefillWithSaltWater,
                (config, value) => config.Tools.Can.PreventRefillWithSaltWater = value)
        #endregion watering can settings

        #region scythe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Scythe", () => I18n.Gmcm_Tols_Scythe() + I18n.Gmcm_Headings_Blank())
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_ClearTreeSaplings_Desc,
                config => config.Tools.Scythe.ClearTreeSaplings,
                (config, value) => config.Tools.Scythe.ClearTreeSaplings = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Tiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_Scythe_RegularRadius_Title,
                I18n.Gmcm_Tols_Tiles_Scythe_RegularRadius_Desc,
                config => (int)config.Tools.Scythe.RegularRadius,
                (config, value) => config.Tools.Scythe.RegularRadius = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Tiles_Scythe_GoldRadius_Title,
                I18n.Gmcm_Tols_Tiles_Scythe_GoldRadius_Desc,
                config => (int)config.Tools.Scythe.GoldRadius,
                (config, value) => config.Tools.Scythe.GoldRadius = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Scythe_Harvest_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvest_Crops_Title,
                I18n.Gmcm_Tols_Scythe_Harvest_Crops_Desc,
                config => config.Tools.Scythe.HarvestCrops,
                (config, value) => config.Tools.Scythe.HarvestCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvest_Flowers_Title,
                I18n.Gmcm_Tols_Scythe_Harvest_Flowers_Desc,
                config => config.Tools.Scythe.HarvestFlowers,
                (config, value) => config.Tools.Scythe.HarvestFlowers = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvest_Forage_Title,
                I18n.Gmcm_Tols_Scythe_Harvest_Forage_Desc,
                config => config.Tools.Scythe.HarvestForage,
                (config, value) => config.Tools.Scythe.HarvestForage = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvest_Goldonly_Title,
                I18n.Gmcm_Tols_Scythe_Harvest_Goldonly_Desc,
                config => config.Tools.Scythe.GoldScytheOnly,
                (config, value) => config.Tools.Scythe.GoldScytheOnly = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Enchantments_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enchantments_Haymaker_Title,
                I18n.Gmcm_Tols_Enchantments_Haymaker_Desc,
                config => config.Tools.Scythe.AllowHaymakerEnchantment,
                (config, value) => config.Tools.Scythe.AllowHaymakerEnchantment = value);
        #endregion scythe settings
    }
}
