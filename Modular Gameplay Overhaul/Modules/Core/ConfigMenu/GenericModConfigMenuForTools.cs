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
    /// <summary>Register the config menu if available.</summary>
    private void RegisterTools()
    {
        var allowedUpgrades = new[] { "Copper", "Steel", "Gold", "Iridium" };
        var isMoonMisadventuresLoaded = MoonMisadventuresIntegration.Instance?.IsLoaded == true;
        if (isMoonMisadventuresLoaded)
        {
            allowedUpgrades.AddRangeToArray(new[] { "Radioactive", "Mythicite" });
        }

        this
            .AddPage(OverhaulModule.Tools.Namespace, () => "Tool Settings")

            // general
            .AddSectionTitle(() => "General Settings")
            .AddCheckbox(
                () => "Hide Affected Tiles",
                () => "Whether to hide affected tiles overlay while charging.",
                config => config.Tools.HideAffectedTiles,
                (config, value) => config.Tools.HideAffectedTiles = value)
            .AddNumberField(
                () => "Shockwave Delay",
                () => "Affects the shockwave travel speed. Lower is faster. Set to 0 for instant.",
                config => (int)config.Tools.TicksBetweenWaves,
                (config, value) => config.Tools.TicksBetweenWaves = (uint)value,
                0,
                10)
            .AddHorizontalRule()

            // controls
            .AddSectionTitle(() => "Control Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () =>
                    "The key used for indicating auto-selectable tools, as well as for charging resource tools, if either of those options is enabled.",
                config => config.Tools.ModKey,
                (config, value) => config.Tools.ModKey = value)
            .AddCheckbox(
                () => "Enable Auto-Selection",
                () =>
                    "The best among the selected tools will be automatically chosen for the target tile.",
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
                () => "Selection Border Color",
                () => "The color used to indicate a weapon or slingshot that may be auto-selected.",
                config => config.Tools.SelectionBorderColor,
                (config, value) => config.Tools.SelectionBorderColor = value,
                Color.Magenta,
                colorPickerStyle: (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)
            .AddCheckbox(
                () => "Charging Requires ModKey",
                () => "Whether charging requires holding down a mod key.",
                config => config.Tools.ChargingRequiresModKey,
                (config, value) => config.Tools.ChargingRequiresModKey = value)
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.Tools.FaceMouseCursor,
                (config, value) => config.Tools.FaceMouseCursor = value)
            .AddCheckbox(
                () => "Color-Coded For Convenience",
                () => "Whether to colorize tool names according to the tool's upgrade level.",
                config => config.Tools.ColorCodedForYourConvenience,
                (config, value) => config.Tools.ColorCodedForYourConvenience = value)
            .AddHorizontalRule()

            // page links
            .AddMultiPageLinkOption(
                getOptionName: () => "Specific tool settings:",
                pages: new[] { "Axe", "Pickaxe", "Hoe", "Watering Can", "Scythe" },
                getPageId: tool => OverhaulModule.Tools.Namespace + $"/{tool}",
                getPageName: tool => tool,
                getColumnsFromWidth: _ => 2)

            // axe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Axe", () => "Axe Settings")
            .AddPageLink(OverhaulModule.Tools.Namespace, () => "Back to Tool settings")
            .AddVerticalSpace()
            .AddNumberField(
                () => "Base Stamina Multiplier",
                () => "Adjusts the base stamina cost of the Axe.",
                config => config.Tools.Axe.BaseStaminaMultiplier,
                (config, value) => config.Tools.Axe.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Charging Settings")
            .AddCheckbox(
                () => "Enable Axe Charging",
                () => "Enables charging the Axe.",
                config => config.Tools.Axe.EnableCharging,
                (config, value) => config.Tools.Axe.EnableCharging = value)
            .AddDropdown(
                () => "Min. Upgrade For Charging",
                () => "Your Axe must be at least this level in order to charge.",
                config => config.Tools.Axe.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Axe.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => value)
            .AddNumberField(
                () => "Stamina Consumption Multiplier",
                () => "Adjusts the stamina cost of charging.",
                config => config.Tools.Axe.ChargedStaminaMultiplier,
                (config, value) => config.Tools.Axe.ChargedStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Axe.",
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10,
                1)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Axe.",
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Axe.",
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Axe.",
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (isMoonMisadventuresLoaded && ToolsModule.Config.Axe.RadiusAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Axe.",
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Axe.",
                    config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Axe with Reaching Enchantment.",
                config => (int)config.Tools.Axe.RadiusAtEachPowerLevel[isMoonMisadventuresLoaded ? 6 : 4],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[isMoonMisadventuresLoaded ? 6 : 4] =
                    (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Shockwave Settings")
            .AddCheckbox(
                () => "Clear Fruit Tree Seeds",
                () => "Whether to clear fruit tree seeds.",
                config => config.Tools.Axe.ClearFruitTreeSeeds,
                (config, value) => config.Tools.Axe.ClearFruitTreeSeeds = value)
            .AddCheckbox(
                () => "Clear Fruit Tree Saplings",
                () => "Whether to clear fruit trees that aren't fully grown.",
                config => config.Tools.Axe.ClearFruitTreeSaplings,
                (config, value) => config.Tools.Axe.ClearFruitTreeSaplings = value)
            .AddCheckbox(
                () => "Cut Grown Fruit Trees",
                () => "Whether to cut down fully-grown fruit trees.",
                config => config.Tools.Axe.CutGrownFruitTrees,
                (config, value) => config.Tools.Axe.CutGrownFruitTrees = value)
            .AddCheckbox(
                () => "Clear Tree Seeds",
                () => "Whether to clear non-fruit tree seeds.",
                config => config.Tools.Axe.ClearTreeSeeds,
                (config, value) => config.Tools.Axe.ClearTreeSeeds = value)
            .AddCheckbox(
                () => "Clear Tree Saplings",
                () => "Whether to clear non-fruit trees that aren't fully grown.",
                config => config.Tools.Axe.ClearTreeSaplings,
                (config, value) => config.Tools.Axe.ClearTreeSaplings = value)
            .AddCheckbox(
                () => "Cut Grown Trees",
                () => "Whether to cut down fully-grown non-fruit trees.",
                config => config.Tools.Axe.CutGrownTrees,
                (config, value) => config.Tools.Axe.CutGrownTrees = value)
            .AddCheckbox(
                () => "Cut Tapped Trees",
                () => "Whether to cut down non-fruit trees that have a tapper.",
                config => config.Tools.Axe.CutTappedTrees,
                (config, value) => config.Tools.Axe.CutTappedTrees = value)
            .AddCheckbox(
                () => "Cut Giant Crops",
                () => "Whether to harvest giant crops.",
                config => config.Tools.Axe.CutGiantCrops,
                (config, value) => config.Tools.Axe.CutGiantCrops = value)
            .AddCheckbox(
                () => "Clear Bushes",
                () => "Whether to clear bushes.",
                config => config.Tools.Axe.ClearBushes,
                (config, value) => config.Tools.Axe.ClearBushes = value)
            .AddCheckbox(
                () => "Clear Live Crops",
                () => "Whether to clear live crops.",
                config => config.Tools.Axe.ClearLiveCrops,
                (config, value) => config.Tools.Axe.ClearLiveCrops = value)
            .AddCheckbox(
                () => "Clear Dead Crops",
                () => "Whether to clear dead crops.",
                config => config.Tools.Axe.ClearDeadCrops,
                (config, value) => config.Tools.Axe.ClearDeadCrops = value)
            .AddCheckbox(
                () => "Clear Debris",
                () => "Whether to clear debris like twigs, giant stumps, fallen logs and weeds.",
                config => config.Tools.Axe.ClearDebris,
                (config, value) => config.Tools.Axe.ClearDebris = value)
            .AddCheckbox(
                () => "Play Shockwave Animation",
                () => "Whether to play the shockwave animation when the charged Axe is released.",
                config => config.Tools.Axe.PlayShockwaveAnimation,
                (config, value) => config.Tools.Axe.PlayShockwaveAnimation = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Allow Reaching Enchantment",
                () => "Whether the Axe can be enchanted with Reaching.",
                config => config.Tools.Axe.AllowReachingEnchantment,
                (config, value) => config.Tools.Axe.AllowReachingEnchantment = value)
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Axe can be enchanted with Master.",
                config => config.Tools.Axe.AllowMasterEnchantment,
                (config, value) => config.Tools.Axe.AllowMasterEnchantment = value)

            // pickaxe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Pickaxe", () => "Pick Settings")
            .AddPageLink(OverhaulModule.Tools.Namespace, () => "Back to Tool settings")
            .AddVerticalSpace()
            .AddNumberField(
                () => "Base Stamina Multiplier",
                () => "Adjusts the base stamina cost of the Pickaxe.",
                config => config.Tools.Pick.BaseStaminaMultiplier,
                (config, value) => config.Tools.Pick.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Charging Settings")
            .AddCheckbox(
                () => "Enable Pick Charging",
                () => "Enables charging the Pickaxe.",
                config => config.Tools.Pick.EnableCharging,
                (config, value) => config.Tools.Pick.EnableCharging = value)
            .AddDropdown(
                () => "Min. Upgrade For Charging",
                () => "Your Pick must be at least this level in order to charge.",
                config => config.Tools.Pick.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.Tools.Pick.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => value)
            .AddNumberField(
                () => "Stamina Consumption Multiplier",
                () => "Adjusts the stamina cost of charging.",
                config => config.Tools.Pick.ChargedStaminaMultiplier,
                (config, value) => config.Tools.Pick.ChargedStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Pick.",
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[0] = (uint)value,
                1,
                10)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Pick.",
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[1] = (uint)value,
                1,
                10)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Pick.",
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[2] = (uint)value,
                1,
                10)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Pick.",
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[3] = (uint)value,
                1,
                10);

        if (isMoonMisadventuresLoaded && ToolsModule.Config.Pick.RadiusAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Pick.",
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[4] = (uint)value,
                    1,
                    10)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Pick.",
                    config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[5] = (uint)value,
                    1,
                    10);
        }

        this
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Pick with Reaching Enchantment.",
                config => (int)config.Tools.Pick.RadiusAtEachPowerLevel[isMoonMisadventuresLoaded ? 6 : 4],
                (config, value) =>
                    config.Tools.Pick.RadiusAtEachPowerLevel[isMoonMisadventuresLoaded ? 6 : 4] = (uint)value,
                1,
                10)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Shockwave Settings")
            .AddCheckbox(
                () => "Break Boulders and Meteorites",
                () => "Whether to break boulders and meteorites.",
                config => config.Tools.Pick.BreakBouldersAndMeteorites,
                (config, value) => config.Tools.Pick.BreakBouldersAndMeteorites = value)
            .AddCheckbox(
                () => "Harvest Mine Spawns",
                () => "Whether to harvest spawned items in the mines.",
                config => config.Tools.Pick.HarvestMineSpawns,
                (config, value) => config.Tools.Pick.HarvestMineSpawns = value)
            .AddCheckbox(
                () => "Break Mine Containers",
                () => "Whether to break containers in the mine.",
                config => config.Tools.Pick.BreakMineContainers,
                (config, value) => config.Tools.Pick.BreakMineContainers = value)
            .AddCheckbox(
                () => "Clear Objects",
                () => "Whether to clear placed objects.",
                config => config.Tools.Pick.ClearObjects,
                (config, value) => config.Tools.Pick.ClearObjects = value)
            .AddCheckbox(
                () => "Clear Flooring",
                () => "Whether to clear placed paths & flooring.",
                config => config.Tools.Pick.ClearFlooring,
                (config, value) => config.Tools.Pick.ClearFlooring = value)
            .AddCheckbox(
                () => "Clear Dirt",
                () => "Whether to clear tilled dirt.",
                config => config.Tools.Pick.ClearDirt,
                (config, value) => config.Tools.Pick.ClearDirt = value)
            .AddCheckbox(
                () => "Clear Live Crops",
                () => "Whether to clear live crops.",
                config => config.Tools.Pick.ClearLiveCrops,
                (config, value) => config.Tools.Pick.ClearLiveCrops = value)
            .AddCheckbox(
                () => "Clear Dead Crops",
                () => "Whether to clear dead crops.",
                config => config.Tools.Pick.ClearDeadCrops,
                (config, value) => config.Tools.Pick.ClearDeadCrops = value)
            .AddCheckbox(
                () => "Clear Debris",
                () => "Whether to clear debris like stones, boulders and weeds.",
                config => config.Tools.Pick.ClearDebris,
                (config, value) => config.Tools.Pick.ClearDebris = value)
            .AddCheckbox(
                () => "Play Shockwave Animation",
                () => "Whether to play the shockwave animation when the charged Pick is released.",
                config => config.Tools.Pick.PlayShockwaveAnimation,
                (config, value) => config.Tools.Pick.PlayShockwaveAnimation = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Allow Reaching Enchantment",
                () => "Whether the Pick can be enchanted with Reaching.",
                config => config.Tools.Pick.AllowReachingEnchantment,
                (config, value) => config.Tools.Pick.AllowReachingEnchantment = value)
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Pick can be enchanted with Master.",
                config => config.Tools.Pick.AllowMasterEnchantment,
                (config, value) => config.Tools.Pick.AllowMasterEnchantment = value)

            // hoe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Hoe", () => "Hoe Settings")
            .AddPageLink(OverhaulModule.Tools.Namespace, () => "Back to Tool settings")
            .AddVerticalSpace()
            .AddNumberField(
                () => "Base Stamina Multiplier",
                () => "Adjusts the base stamina cost of the Hoe.",
                config => config.Tools.Hoe.BaseStaminaMultiplier,
                (config, value) => config.Tools.Hoe.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Area Of Effect Settings")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () =>
                    "Whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve performance.",
                config => config.Tools.Hoe.OverrideAffectedTiles,
                (config, value) => config.Tools.Hoe.OverrideAffectedTiles = value)
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Hoe.",
                config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (isMoonMisadventuresLoaded && ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    () => "Radioactive Length",
                    () => "The length of affected tiles for the Radioactive Hoe.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles to either side of the farmer for the Radioactive Hoe.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7)
                .AddNumberField(
                    () => "Mythicite Length",
                    () => "The length of affected tiles for the Mythicite Hoe.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles to either side of the farmer for the Mythicite Hoe.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7)
                .AddNumberField(
                    () => "Reaching Length",
                    () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[6].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[6].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Reaching Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[6].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[6].Radius = (uint)value,
                    0,
                    7);
        }
        else
        {
            this
                .AddNumberField(
                    () => "Reaching Length",
                    () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Reaching Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Hoe.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddHorizontalRule()

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Hoe can be enchanted with Master.",
                config => config.Tools.Hoe.AllowMasterEnchantment,
                (config, value) => config.Tools.Hoe.AllowMasterEnchantment = value)

            // can settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Watering Can", () => "Watering Can Settings")
            .AddPageLink(OverhaulModule.Tools.Namespace, () => "Back to Tool settings")
            .AddVerticalSpace()
            .AddNumberField(
                () => "Base Stamina Multiplier",
                () => "Adjusts the base stamina cost of the Watering Can.",
                config => config.Tools.Can.BaseStaminaMultiplier,
                (config, value) => config.Tools.Can.BaseStaminaMultiplier = value,
                0f,
                3f,
                0.2f)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Area Of Effect Settings")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () =>
                    "Whether to apply custom tile area for the Watering Can. Keep this at false if using defaults to improve performance.",
                config => config.Tools.Can.OverrideAffectedTiles,
                (config, value) => config.Tools.Can.OverrideAffectedTiles = value)
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[0].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[1].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[2].Radius = (uint)value,
                0,
                7)
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Length = (uint)value,
                1,
                15)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Watering Can.",
                config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius,
                (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[3].Radius = (uint)value,
                0,
                7);

        if (isMoonMisadventuresLoaded && ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel.Length > 5)
        {
            this
                .AddNumberField(
                    () => "Radioactive Length",
                    () => "The length of affected tiles for the Radioactive Watering Can.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Radioactive Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Radioactive Watering Can.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7)
                .AddNumberField(
                    () => "Mythicite Length",
                    () => "The length of affected tiles for the Mythicite Watering Can.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Mythicite Watering Can.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[5].Radius = (uint)value,
                    0,
                    7)
                .AddNumberField(
                    () => "Reaching Length",
                    () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[6].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[6].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Reaching Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[6].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[6].Radius = (uint)value,
                    0,
                    7);
        }
        else
        {
            this
                .AddNumberField(
                    () => "Reaching Length",
                    () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Length = (uint)value,
                    1,
                    15)
                .AddNumberField(
                    () => "Reaching Radius",
                    () =>
                        "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                    config => (int)config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius,
                    (config, value) => config.Tools.Can.AffectedTilesAtEachPowerLevel[4].Radius = (uint)value,
                    0,
                    7);
        }

        this
            .AddHorizontalRule()

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Watering Can can be enchanted with Master.",
                config => config.Tools.Can.AllowMasterEnchantment,
                (config, value) => config.Tools.Can.AllowMasterEnchantment = value)
            .AddCheckbox(
                () => "Allow Swift Enchantment",
                () => "Whether the Watering Can can be enchanted with Swift.",
                config => config.Tools.Can.AllowSwiftEnchantment,
                (config, value) => config.Tools.Can.AllowSwiftEnchantment = value)

            // scythe settings
            .AddPage(OverhaulModule.Tools.Namespace + "/Scythe", () => "Scythe Settings")
            .AddPageLink(OverhaulModule.Tools.Namespace, () => "Back to Tool settings")
            .AddVerticalSpace()
            .AddSectionTitle(() => "Area Of Effect Settings")
            .AddNumberField(
                () => "Regular Scythe Radius",
                () => "Sets the area of effect of the regular Scythe.",
                config => (int)config.Tools.Scythe.RegularRadius,
                (config, value) => config.Tools.Scythe.RegularRadius = (uint)value,
                0,
                10)
            .AddNumberField(
                () => "Golden Scythe Radius",
                () => "Sets the area of effect of the Golden Scythe.",
                config => (int)config.Tools.Scythe.GoldRadius,
                (config, value) => config.Tools.Scythe.GoldRadius = (uint)value,
                0,
                10)
            .AddCheckbox(
                () => "Clear Tree Saplings",
                () => "Whether to clear tree saplings with the Scythe.",
                config => config.Tools.Scythe.ClearTreeSaplings,
                (config, value) => config.Tools.Scythe.ClearTreeSaplings = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Harvesting Settings")
            .AddCheckbox(
                () => "Harvest Crops",
                () => "Whether to harvest crops with the Scythe.",
                config => config.Tools.Scythe.HarvestCrops,
                (config, value) => config.Tools.Scythe.HarvestCrops = value)
            .AddCheckbox(
                () => "Harvest Flowers",
                () => "Whether to harvest crops with the Scythe.",
                config => config.Tools.Scythe.HarvestFlowers,
                (config, value) => config.Tools.Scythe.HarvestFlowers = value)
            .AddCheckbox(
                () => "Harvest Forage",
                () => "Whether to harvest forage with the Scythe.",
                config => config.Tools.Scythe.HarvestForage,
                (config, value) => config.Tools.Scythe.HarvestForage = value)
            .AddCheckbox(
                () => "Golden Scythe Only",
                () => "Whether to limit crop and flower harvesting to the Golden Scythe.",
                config => config.Tools.Scythe.GoldScytheOnly,
                (config, value) => config.Tools.Scythe.GoldScytheOnly = value)
            .AddHorizontalRule()

            .AddSectionTitle(() => "Enchantment Settings")
            .AddCheckbox(
                () => "Allow Haymaker Enchantment",
                () => "Whether the Scythe can be enchanted with Haymaker.",
                config => config.Tools.Scythe.AllowHaymakerEnchantment,
                (config, value) => config.Tools.Scythe.AllowHaymakerEnchantment = value);
    }
}
