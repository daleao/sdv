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
            .AddNumberField(
                I18n.Gmcm_Tols_Ticksbetweenwaves_Title,
                I18n.Gmcm_Tols_Ticksbetweenwaves_Desc,
                config => (int)config.Tools.TicksBetweenWaves,
                (config, value) => config.Tools.TicksBetweenWaves = (uint)value,
                0,
                10)
            .AddCheckbox(
                I18n.Gmcm_Tols_Enableforgeupgrading_Title,
                I18n.Gmcm_Tols_Enableforgeupgrading_Desc,
                config => config.Tools.EnableForgeUpgrading,
                (config, value) => config.Tools.EnableForgeUpgrading = value)
            .AddHorizontalRule()

            // controls
            .AddSectionTitle(I18n.Gmcm_Controls_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Controls_Holdtocharge_Title,
                I18n.Gmcm_Tols_Controls_Holdtocharge_Desc,
                config => config.Tools.HoldToCharge,
                (config, value) => config.Tools.HoldToCharge = value)
            .AddKeyBinding(
                I18n.Gmcm_Controls_Modkey_Title,
                I18n.Gmcm_Tols_Controls_Modkey_Desc,
                config => config.Tools.ModKey,
                (config, value) => config.Tools.ModKey = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_Facemousecursor_Title,
                I18n.Gmcm_Controls_Facemousecursor_Desc,
                config => config.Tools.FaceMouseCursor,
                (config, value) => config.Tools.FaceMouseCursor = value)
            .AddCheckbox(
                I18n.Gmcm_Controls_Enableautoselection_Title,
                () => I18n.Gmcm_Controls_Enableautoselection_Desc(I18n.Gmcm_Tols_Tools().ToLowerInvariant()),
                config => config.Tools.EnableAutoSelection,
                (config, value) =>
                {
                    config.Tools.EnableAutoSelection = value;
                    if (!value)
                    {
                        ToolsModule.State.SelectableToolByType.Clear();
                    }
                })
            .AddColorPicker(
                I18n.Gmcm_Controls_Selectionbordercolor_Title,
                () => I18n.Gmcm_Controls_Selectionbordercolor_Desc(I18n.Gmcm_Tols_Tools().ToLowerInvariant()),
                config => config.Tools.SelectionBorderColor,
                (config, value) => config.Tools.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Interface_Heading)
            .AddCheckbox(
                I18n.Gmcm_Interface_Colorcodedforyourconvenience_Title,
                I18n.Gmcm_Tols_Interface_Colorcodedforyourconvenience_Desc,
                config => config.Tools.ColorCodedForYourConvenience,
                (config, value) => config.Tools.ColorCodedForYourConvenience = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Interface_Hideaffectedtiles_Title,
                I18n.Gmcm_Tols_Interface_Hideaffectedtiles_Desc,
                config => config.Tools.HideAffectedTiles,
                (config, value) => config.Tools.HideAffectedTiles = value)
            .AddHorizontalRule()

            // page links
            .AddMultiPageLinkOption(
                getOptionName: I18n.Gmcm_Tols_Specificsettings,
                pages: new[] { "Axe", "Pickaxe", "Hoe", "WateringCan", "Scythe" },
                getPageId: tool => OverhaulModule.Tools.Namespace + $"/{tool}",
                getPageName: tool => _I18n.Get("gmcm.tols." + tool.ToLowerInvariant()),
                getColumnsFromWidth: _ => 2)

            // axe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Axe", I18n.Gmcm_Tols_Specificsettings_Axe)
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_Basestaminamultiplier_Title,
                () => I18n.Gmcm_Tols_Basestaminamultiplier_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.BaseStaminaMultiplier,
                (config, value) => config.Tools.Axe.BaseStaminaMultiplier = value,
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
                I18n.Gmcm_Tols_Charging_Requpgradeforcharging_Title,
                () => I18n.Gmcm_Tols_Charging_Requpgradeforcharging_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Axe.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => _I18n.Get("gmcm.tols.upgrades." + value.ToLowerInvariant()))
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Chargedstaminamultiplier_Title,
                I18n.Gmcm_Tols_Charging_Chargedstaminamultiplier_Desc,
                config => config.Tools.Axe.ChargedStaminaMultiplier,
                (config, value) => config.Tools.Axe.ChargedStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Copperradius_Title,
                I18n.Gmcm_Tols_Charging_Copperradius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10,
                1)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Steelradius_Title,
                I18n.Gmcm_Tols_Charging_Steelradius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Goldradius_Title,
                I18n.Gmcm_Tols_Charging_Goldradius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Iridiumradius_Title,
                I18n.Gmcm_Tols_Charging_Iridiumradius_Desc,
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Title,
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Desc,
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 6)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Title,
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Desc,
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Enchantedradius_Title,
                () => I18n.Gmcm_Tols_Charging_Enchantedradius_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[^1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[^1] = (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Shockwave_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Clearfruittreeseeds_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Clearfruittreeseeds_Desc,
                config => config.Tools.Axe.ClearFruitTreeSeeds,
                (config, value) => config.Tools.Axe.ClearFruitTreeSeeds = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Clearfruittreesaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Clearfruittreesaplings_Desc,
                config => config.Tools.Axe.ClearFruitTreeSaplings,
                (config, value) => config.Tools.Axe.ClearFruitTreeSaplings = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgrownfruittrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgrownfruittrees_Desc,
                config => config.Tools.Axe.CutGrownFruitTrees,
                (config, value) => config.Tools.Axe.CutGrownFruitTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreeseeds_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreeseeds_Desc,
                config => config.Tools.Axe.ClearTreeSeeds,
                (config, value) => config.Tools.Axe.ClearTreeSeeds = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreesaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreesaplings_Desc,
                config => config.Tools.Axe.ClearTreeSaplings,
                (config, value) => config.Tools.Axe.ClearTreeSaplings = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgrowntrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgrowntrees_Desc,
                config => config.Tools.Axe.CutGrownTrees,
                (config, value) => config.Tools.Axe.CutGrownTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cuttappedtrees_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cuttappedtrees_Desc,
                config => config.Tools.Axe.CutTappedTrees,
                (config, value) => config.Tools.Axe.CutTappedTrees = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgiantcrops_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cutgiantcrops_Desc,
                config => config.Tools.Axe.CutGiantCrops,
                (config, value) => config.Tools.Axe.CutGiantCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Clearbushes_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Clearbushes_Desc,
                config => config.Tools.Axe.ClearBushes,
                (config, value) => config.Tools.Axe.ClearBushes = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Clearlivecrops_Title,
                I18n.Gmcm_Tols_Shockwave_Clearlivecrops_Desc,
                config => config.Tools.Axe.ClearLiveCrops,
                (config, value) => config.Tools.Axe.ClearLiveCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Cleardeadcrops_Title,
                I18n.Gmcm_Tols_Shockwave_Cleardeadcrops_Desc,
                config => config.Tools.Axe.ClearDeadCrops,
                (config, value) => config.Tools.Axe.ClearDeadCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Cleardebris_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cleardebris_Desc,
                config => config.Tools.Axe.ClearDebris,
                (config, value) => config.Tools.Axe.ClearDebris = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Playanimation_Title,
                I18n.Gmcm_Tols_Shockwave_Playanimation_Desc,
                config => config.Tools.Axe.PlayShockwaveAnimation,
                (config, value) => config.Tools.Axe.PlayShockwaveAnimation = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Allowenchantment_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Reaching_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Reaching_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.AllowReachingEnchantment,
                (config, value) => config.Tools.Axe.AllowReachingEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Master_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Master_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Axe.AllowMasterEnchantment,
                (config, value) => config.Tools.Axe.AllowMasterEnchantment = value)

            // pickaxe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Pickaxe", I18n.Gmcm_Tols_Specificsettings_Pickaxe)
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_Basestaminamultiplier_Title,
                () => I18n.Gmcm_Tols_Basestaminamultiplier_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.BaseStaminaMultiplier,
                (config, value) => config.Tools.Pick.BaseStaminaMultiplier = value,
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
                I18n.Gmcm_Tols_Charging_Requpgradeforcharging_Title,
                () => I18n.Gmcm_Tols_Charging_Requpgradeforcharging_Desc(I18n.Gmcm_Tols_Axe().ToLowerInvariant()),
                config => config.Tools.Pick.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Pick.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => _I18n.Get("gmcm.tols.upgrades." + value.ToLowerInvariant()))
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Chargedstaminamultiplier_Title,
                I18n.Gmcm_Tols_Charging_Chargedstaminamultiplier_Desc,
                config => config.Tools.Pick.ChargedStaminaMultiplier,
                (config, value) => config.Tools.Pick.ChargedStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Copperradius_Title,
                I18n.Gmcm_Tols_Charging_Copperradius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Steelradius_Title,
                I18n.Gmcm_Tols_Charging_Steelradius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Goldradius_Title,
                I18n.Gmcm_Tols_Charging_Goldradius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Iridiumradius_Title,
                I18n.Gmcm_Tols_Charging_Iridiumradius_Desc,
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Title,
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Desc,
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 6)
        {
            this.AddNumberField(
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Title,
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Desc,
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Enchantedradius_Title,
                () => I18n.Gmcm_Tols_Charging_Enchantedradius_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[^1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[^1] = (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Shockwave_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Breakbouldersandmeteorites_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Breakbouldersandmeteorites_Desc,
                config => config.Tools.Pick.BreakBouldersAndMeteorites,
                (config, value) => config.Tools.Pick.BreakBouldersAndMeteorites = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Harvestminespawns_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Harvestminespawns_Desc,
                config => config.Tools.Pick.HarvestMineSpawns,
                (config, value) => config.Tools.Pick.HarvestMineSpawns = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Breakminecontainers_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Breakminecontainers_Desc,
                config => config.Tools.Pick.BreakMineContainers,
                (config, value) => config.Tools.Pick.BreakMineContainers = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Clearobjects_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Clearobjects_Desc,
                config => config.Tools.Pick.ClearObjects,
                (config, value) => config.Tools.Pick.ClearObjects = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Clearflooring_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Clearflooring_Desc,
                config => config.Tools.Pick.ClearFlooring,
                (config, value) => config.Tools.Pick.ClearFlooring = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Pick_Cleardirt_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Cleardirt_Desc,
                config => config.Tools.Pick.ClearDirt,
                (config, value) => config.Tools.Pick.ClearDirt = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Clearlivecrops_Title,
                I18n.Gmcm_Tols_Shockwave_Clearlivecrops_Desc,
                config => config.Tools.Pick.ClearLiveCrops,
                (config, value) => config.Tools.Pick.ClearLiveCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Cleardeadcrops_Title,
                I18n.Gmcm_Tols_Shockwave_Cleardeadcrops_Desc,
                config => config.Tools.Pick.ClearDeadCrops,
                (config, value) => config.Tools.Pick.ClearDeadCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Cleardebris_Title,
                I18n.Gmcm_Tols_Shockwave_Pick_Cleardebris_Desc,
                config => config.Tools.Pick.ClearDebris,
                (config, value) => config.Tools.Pick.ClearDebris = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Playanimation_Title,
                I18n.Gmcm_Tols_Shockwave_Playanimation_Desc,
                config => config.Tools.Pick.PlayShockwaveAnimation,
                (config, value) => config.Tools.Pick.PlayShockwaveAnimation = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Allowenchantment_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Reaching_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Reaching_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.AllowReachingEnchantment,
                (config, value) => config.Tools.Pick.AllowReachingEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Master_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Master_Desc(I18n.Gmcm_Tols_Pickaxe().ToLowerInvariant()),
                config => config.Tools.Pick.AllowMasterEnchantment,
                (config, value) => config.Tools.Pick.AllowMasterEnchantment = value)

            // hoe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Hoe", I18n.Gmcm_Tols_Specificsettings_Hoe)
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_Basestaminamultiplier_Title,
                () => I18n.Gmcm_Tols_Basestaminamultiplier_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                config => config.Tools.Hoe.BaseStaminaMultiplier,
                (config, value) => config.Tools.Hoe.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Affectedtiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Copperlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Copperlength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Copperradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Copperradius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Steellength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Steellength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Steelradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Steelradius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Goldlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Goldlength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Goldradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Goldradius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Iridiumlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Iridiumlength_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Iridiumradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Iridiumradius_Desc,
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Affectedtiles_Radioactivelength_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Radioactivelength_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Radioactiveradius_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Affectedtiles_Mythicitelength_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Mythicitelength_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Mythiciteradius_Desc,
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddNumberField(
                    I18n.Gmcm_Tols_Affectedtiles_Enchantedlength_Title,
                    () => I18n.Gmcm_Tols_Affectedtiles_Enchantedlength_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Length = (uint)value,
                    1,
                    15)
            .AddNumberField(
                    I18n.Gmcm_Tols_Charging_Enchantedradius_Title,
                    () => I18n.Gmcm_Tols_Affectedtiles_Enchantedradius_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[^1].Radius = (uint)value,
                    0,
                    7)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Allowenchantment_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Master_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Master_Desc(I18n.Gmcm_Tols_Hoe().ToLowerInvariant()),
                config => config.Tools.Hoe.AllowMasterEnchantment,
                (config, value) => config.Tools.Hoe.AllowMasterEnchantment = value)

            // can settings
            .AddPage(OverhaulModule.Tools.Namespace + "/WateringCan", I18n.Gmcm_Tols_Specificsettings_Wateringcan)
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddNumberField(
                I18n.Gmcm_Tols_Basestaminamultiplier_Title,
                () => I18n.Gmcm_Tols_Basestaminamultiplier_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.BaseStaminaMultiplier,
                (config, value) => config.Tools.Can.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Affectedtiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Copperlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Copperlength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Copperradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Copperradius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Steellength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Steellength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Steelradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Steelradius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Goldlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Goldlength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Goldradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Goldradius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Iridiumlength_Title,
                I18n.Gmcm_Tols_Affectedtiles_Iridiumlength_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Iridiumradius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Iridiumradius_Desc,
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (maxToolUpgrade > 5 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Affectedtiles_Radioactivelength_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Radioactivelength_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_Radioactiveradius_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Radioactiveradius_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        if (maxToolUpgrade > 6 && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 6)
        {
            this
                .AddNumberField(
                    I18n.Gmcm_Tols_Affectedtiles_Mythicitelength_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Mythicitelength_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    I18n.Gmcm_Tols_Charging_Mythiciteradius_Title,
                    I18n.Gmcm_Tols_Affectedtiles_Mythiciteradius_Desc,
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Enchantedlength_Title,
                () => I18n.Gmcm_Tols_Affectedtiles_Enchantedlength_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                I18n.Gmcm_Tols_Charging_Enchantedradius_Title,
                () => I18n.Gmcm_Tols_Affectedtiles_Enchantedradius_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[^1].Radius = (uint)value,
                0,
                7)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Allowenchantment_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Master_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Master_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.AllowMasterEnchantment,
                (config, value) => config.Tools.Can.AllowMasterEnchantment = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Swift_Title,
                () => I18n.Gmcm_Tols_Allowenchantment_Swift_Desc(I18n.Gmcm_Tols_Wateringcan().ToLowerInvariant()),
                config => config.Tools.Can.AllowSwiftEnchantment,
                (config, value) => config.Tools.Can.AllowSwiftEnchantment = value)

            // scythe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Scythe", I18n.Gmcm_Tols_Specificsettings_Scythe)
            .AddPageLink(OverhaulModule.Tools.Namespace, I18n.Gmcm_Tols_Back)
            .AddVerticalSpace()
            .AddCheckbox(
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreesaplings_Title,
                I18n.Gmcm_Tols_Shockwave_Axe_Cleartreesaplings_Desc,
                config => config.Tools.Scythe.ClearTreeSaplings,
                (config, value) => config.Tools.Scythe.ClearTreeSaplings = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Affectedtiles_Heading)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Scythe_Regular_Radius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Scythe_Regular_Radius_Desc,
                config => (int)config.Tools.Scythe.RegularRadius,
                (config, value) => config.Tools.Scythe.RegularRadius = (uint)value,
                0,
                10)
            .AddNumberField(
                I18n.Gmcm_Tols_Affectedtiles_Scythe_Gold_Radius_Title,
                I18n.Gmcm_Tols_Affectedtiles_Scythe_Gold_Radius_Desc,
                config => (int)config.Tools.Scythe.GoldRadius,
                (config, value) => config.Tools.Scythe.GoldRadius = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Scythe_Harvesting_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvesting_Crops_Title,
                I18n.Gmcm_Tols_Scythe_Harvesting_Crops_Desc,
                config => config.Tools.Scythe.HarvestCrops,
                (config, value) => config.Tools.Scythe.HarvestCrops = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvesting_Flowers_Title,
                I18n.Gmcm_Tols_Scythe_Harvesting_Flowers_Desc,
                config => config.Tools.Scythe.HarvestFlowers,
                (config, value) => config.Tools.Scythe.HarvestFlowers = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvesting_Forage_Title,
                I18n.Gmcm_Tols_Scythe_Harvesting_Forage_Desc,
                config => config.Tools.Scythe.HarvestForage,
                (config, value) => config.Tools.Scythe.HarvestForage = value)
            .AddCheckbox(
                I18n.Gmcm_Tols_Scythe_Harvesting_Goldonly_Title,
                I18n.Gmcm_Tols_Scythe_Harvesting_Goldonly_Desc,
                config => config.Tools.Scythe.GoldScytheOnly,
                (config, value) => config.Tools.Scythe.GoldScytheOnly = value)
            .AddHorizontalRule()

            .AddSectionTitle(I18n.Gmcm_Tols_Allowenchantment_Heading)
            .AddCheckbox(
                I18n.Gmcm_Tols_Allowenchantment_Haymaker_Title,
                I18n.Gmcm_Tols_Allowenchantment_Haymaker_Desc,
                config => config.Tools.Scythe.AllowHaymakerEnchantment,
                (config, value) => config.Tools.Scythe.AllowHaymakerEnchantment = value);
    }
}
