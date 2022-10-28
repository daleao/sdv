namespace DaLion.Redux.Core.Integrations;

#region using directives

using DaLion.Redux.Tools;
using DaLion.Redux.Tools.Events;
using HarmonyLib;
using Integrations = DaLion.Redux.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the config menu if available.</summary>
    private void RegisterTools()
    {
        var allowedUpgrades = new[] { "Copper", "Steel", "Gold", "Iridium" };
        if (Integrations.IsMoonMisadventuresLoaded)
        {
            allowedUpgrades.AddRangeToArray(new[] { "Radioactive", "Mythicite" });
        }

        this._configMenu
            .AddPage(ReduxModule.Tools.Name, () => "Tool Settings")

            // general
            .AddSectionTitle(() => "General Settings")
            .AddCheckbox(
                () => "Hide Affected Tiles",
                () => "Whether to hide affected tiles overlay while charging.",
                config => config.Tools.HideAffectedTiles,
                (config, value) => config.Tools.HideAffectedTiles = value)
            .AddNumberField(
                () => "Stamina Consumption Multiplier",
                () => "Adjusts the stamina cost of charging.",
                config => config.Tools.StaminaCostMultiplier,
                (config, value) => config.Tools.StaminaCostMultiplier = value,
                0f,
                10f,
                0.5f)
            .AddNumberField(
                () => "Shockwave Delay",
                () => "Affects the shockwave travel speed. Lower is faster. Set to 0 for instant.",
                config => (int)config.Tools.TicksBetweenWaves,
                (config, value) => config.Tools.TicksBetweenWaves = (uint)value,
                0,
                10)

            // keybinds
            .AddSectionTitle(() => "Control Settings")
            .AddCheckbox(
                () => "Require Modkey",
                () => "Whether charging requires holding down a mod key.",
                config => config.Tools.RequireModkey,
                (config, value) => config.Tools.RequireModkey = value)
            .AddKeyBinding(
                () => "Charging Modkey",
                () => "If 'RequireModkey' is enabled, you must hold this key to begin charging.",
                config => config.Tools.Modkey,
                (config, value) => config.Tools.Modkey = value)
            .AddCheckbox(
                () => "Face Towards Mouse Cursor",
                () =>
                    "If using mouse and keyboard, turn to face towards the current cursor position before swinging your tools.",
                config => config.Tools.FaceMouseCursor,
                (config, value) => config.Tools.FaceMouseCursor = value)
            // page links
            .AddPageLink(ReduxModule.Tools + "/Axe", () => "Axe Settings", () => "Go to Axe settings.")
            .AddPageLink(ReduxModule.Tools + "/Pick", () => "Pick Settings", () => "Go to Pick settings.")
            .AddPageLink(ReduxModule.Tools + "/Hoe", () => "Hoe Settings", () => "Go to Hoe settings.")
            .AddPageLink(ReduxModule.Tools + "/Can", () => "Watering Can Settings", () => "Go to Watering Can settings.")

            // axe settings
            .AddPage(ReduxModule.Tools + "/Axe", () => "Axe Settings")
            .AddPageLink(ReduxModule.Tools.Name, () => "Back to Tool settings")
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
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Axe.",
                config => config.Tools.Axe.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[0] = value,
                1,
                10)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Axe.",
                config => config.Tools.Axe.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[1] = value,
                1,
                10)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Axe.",
                config => config.Tools.Axe.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[2] = value,
                1,
                10)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Axe.",
                config => config.Tools.Axe.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[3] = value,
                1,
                10);

        if (Integrations.IsMoonMisadventuresLoaded)
        {
            this._configMenu
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Axe.",
                    config => config.Tools.Axe.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[4] = value,
                    1,
                    10)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Axe.",
                    config => config.Tools.Axe.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[5] = value,
                    1,
                    10);
        }

        this._configMenu
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Axe with Reaching Enchantment.",
                config => config.Tools.Axe.RadiusAtEachPowerLevel[Integrations.IsMoonMisadventuresLoaded ? 6 : 4],
                (config, value) => config.Tools.Axe.RadiusAtEachPowerLevel[Integrations.IsMoonMisadventuresLoaded ? 6 : 4] =
                    value,
                1,
                10)
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
            .AddPage(ReduxModule.Tools + "/Pick", () => "Pick Settings")
            .AddPageLink(ReduxModule.Tools.Name, () => "Back to Tool settings")
            .AddCheckbox(
                () => "Enable Pick Charging",
                () => "Enables charging the Pickxe.",
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
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Pick.",
                config => config.Tools.Pick.RadiusAtEachPowerLevel[0],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[0] = value,
                1,
                10)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Pick.",
                config => config.Tools.Pick.RadiusAtEachPowerLevel[1],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[1] = value,
                1,
                10)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Pick.",
                config => config.Tools.Pick.RadiusAtEachPowerLevel[2],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[2] = value,
                1,
                10)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Pick.",
                config => config.Tools.Pick.RadiusAtEachPowerLevel[3],
                (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[3] = value,
                1,
                10);

        if (Integrations.IsMoonMisadventuresLoaded)
        {
            this._configMenu
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Pick.",
                    config => config.Tools.Pick.RadiusAtEachPowerLevel[4],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[4] = value,
                    1,
                    10)
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Pick.",
                    config => config.Tools.Pick.RadiusAtEachPowerLevel[5],
                    (config, value) => config.Tools.Pick.RadiusAtEachPowerLevel[5] = value,
                    1,
                    10);
        }

        this._configMenu
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Pick with Reaching Enchantment.",
                config => config.Tools.Pick.RadiusAtEachPowerLevel[Integrations.IsMoonMisadventuresLoaded ? 6 : 4],
                (config, value) =>
                    config.Tools.Pick.RadiusAtEachPowerLevel[Integrations.IsMoonMisadventuresLoaded ? 6 : 4] = value,
                1,
                10)
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
            .AddPage(ReduxModule.Tools + "/Hoe", () => "Hoe Settings")
            .AddPageLink(ReduxModule.Tools.Name, () => "Back to Tool settings")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () =>
                    "Whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve performance.",
                config => config.Tools.Hoe.OverrideAffectedTiles,
                (config, value) => config.Tools.Hoe.OverrideAffectedTiles = value)
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Hoe.",
                config => config.Tools.Hoe.AffectedTiles[0][0],
                (config, value) => config.Tools.Hoe.AffectedTiles[0][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Hoe.",
                config => config.Tools.Hoe.AffectedTiles[0][1],
                (config, value) => config.Tools.Hoe.AffectedTiles[0][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Hoe.",
                config => config.Tools.Hoe.AffectedTiles[1][0],
                (config, value) => config.Tools.Hoe.AffectedTiles[1][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Hoe.",
                config => config.Tools.Hoe.AffectedTiles[1][1],
                (config, value) => config.Tools.Hoe.AffectedTiles[1][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Hoe.",
                config => config.Tools.Hoe.AffectedTiles[2][0],
                (config, value) => config.Tools.Hoe.AffectedTiles[2][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Hoe.",
                config => config.Tools.Hoe.AffectedTiles[2][1],
                (config, value) => config.Tools.Hoe.AffectedTiles[2][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Hoe.",
                config => config.Tools.Hoe.AffectedTiles[3][0],
                (config, value) => config.Tools.Hoe.AffectedTiles[3][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Hoe.",
                config => config.Tools.Hoe.AffectedTiles[3][1],
                (config, value) => config.Tools.Hoe.AffectedTiles[3][1] = value,
                0,
                7);

        switch (Integrations.IsMoonMisadventuresLoaded)
        {
            case false:
                this._configMenu
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                        config => config.Tools.Hoe.AffectedTiles[4][0],
                        (config, value) => config.Tools.Hoe.AffectedTiles[4][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Reaching Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                        config => config.Tools.Hoe.AffectedTiles[4][1],
                        (config, value) => config.Tools.Hoe.AffectedTiles[4][1] = value,
                        0,
                        7);
                break;
            case true:
                this._configMenu
                    .AddNumberField(
                        () => "Radioactive Length",
                        () => "The length of affected tiles for the Radioactive Hoe.",
                        config => config.Tools.Hoe.AffectedTiles[4][0],
                        (config, value) => config.Tools.Hoe.AffectedTiles[4][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Radioactive Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Radioactive Hoe.",
                        config => config.Tools.Hoe.AffectedTiles[4][1],
                        (config, value) => config.Tools.Hoe.AffectedTiles[4][1] = value,
                        0,
                        7)
                    .AddNumberField(
                        () => "Mythicite Length",
                        () => "The length of affected tiles for the Mythicite Hoe.",
                        config => config.Tools.Hoe.AffectedTiles[5][0],
                        (config, value) => config.Tools.Hoe.AffectedTiles[5][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Mythicite Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Mythicite Hoe.",
                        config => config.Tools.Hoe.AffectedTiles[5][1],
                        (config, value) => config.Tools.Hoe.AffectedTiles[5][1] = value,
                        0,
                        7)
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                        config => config.Tools.Hoe.AffectedTiles[6][0],
                        (config, value) => config.Tools.Hoe.AffectedTiles[6][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Reaching Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                        config => config.Tools.Hoe.AffectedTiles[6][1],
                        (config, value) => config.Tools.Hoe.AffectedTiles[6][1] = value,
                        0,
                        7);
                break;
        }

        this._configMenu
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Hoe can be enchanted with Master.",
                config => config.Tools.Hoe.AllowMasterEnchantment,
                (config, value) => config.Tools.Hoe.AllowMasterEnchantment = value)

            // can settings
            .AddPage(ReduxModule.Tools + "/Can", () => "Watering Can Settings")
            .AddPageLink(ReduxModule.Tools.Name, () => "Back to Tool settings")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () =>
                    "Whether to apply custom tile area for the Watering Can. Keep this at false if using defaults to improve performance.",
                config => config.Tools.Can.OverrideAffectedTiles,
                (config, value) => config.Tools.Can.OverrideAffectedTiles = value)
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Watering Can.",
                config => config.Tools.Can.AffectedTiles[0][0],
                (config, value) => config.Tools.Can.AffectedTiles[0][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Watering Can.",
                config => config.Tools.Can.AffectedTiles[0][1],
                (config, value) => config.Tools.Can.AffectedTiles[0][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Watering Can.",
                config => config.Tools.Can.AffectedTiles[1][0],
                (config, value) => config.Tools.Can.AffectedTiles[1][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Watering Can.",
                config => config.Tools.Can.AffectedTiles[1][1],
                (config, value) => config.Tools.Can.AffectedTiles[1][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Watering Can.",
                config => config.Tools.Can.AffectedTiles[2][0],
                (config, value) => config.Tools.Can.AffectedTiles[2][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Watering Can.",
                config => config.Tools.Can.AffectedTiles[2][1],
                (config, value) => config.Tools.Can.AffectedTiles[2][1] = value,
                0,
                7)
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Watering Can.",
                config => config.Tools.Can.AffectedTiles[3][0],
                (config, value) => config.Tools.Can.AffectedTiles[3][0] = value,
                1,
                15)
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Watering Can.",
                config => config.Tools.Can.AffectedTiles[3][1],
                (config, value) => config.Tools.Can.AffectedTiles[3][1] = value,
                0,
                7);

        switch (Integrations.IsMoonMisadventuresLoaded)
        {
            case false:
                this._configMenu
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                        config => config.Tools.Can.AffectedTiles[4][0],
                        (config, value) => config.Tools.Can.AffectedTiles[4][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Reaching Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                        config => config.Tools.Can.AffectedTiles[4][1],
                        (config, value) => config.Tools.Can.AffectedTiles[4][1] = value,
                        0,
                        7);
                break;
            case true:
                this._configMenu
                    .AddNumberField(
                        () => "Radioactive Length",
                        () => "The length of affected tiles for the Radioactive Watering Can.",
                        config => config.Tools.Can.AffectedTiles[4][0],
                        (config, value) => config.Tools.Can.AffectedTiles[4][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Radioactive Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Radioactive Watering Can.",
                        config => config.Tools.Can.AffectedTiles[4][1],
                        (config, value) => config.Tools.Can.AffectedTiles[4][1] = value,
                        0,
                        7)
                    .AddNumberField(
                        () => "Mythicite Length",
                        () => "The length of affected tiles for the Mythicite Watering Can.",
                        config => config.Tools.Can.AffectedTiles[5][0],
                        (config, value) => config.Tools.Can.AffectedTiles[5][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Mythicite Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Mythicite Watering Can.",
                        config => config.Tools.Can.AffectedTiles[5][1],
                        (config, value) => config.Tools.Can.AffectedTiles[5][1] = value,
                        0,
                        7)
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                        config => config.Tools.Can.AffectedTiles[6][0],
                        (config, value) => config.Tools.Can.AffectedTiles[6][0] = value,
                        1,
                        15)
                    .AddNumberField(
                        () => "Reaching Radius",
                        () =>
                            "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                        config => config.Tools.Can.AffectedTiles[6][1],
                        (config, value) => config.Tools.Can.AffectedTiles[6][1] = value,
                        0,
                        7);
                break;
        }

        this._configMenu
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Watering Can can be enchanted with Master.",
                config => config.Tools.Can.AllowMasterEnchantment,
                (config, value) => config.Tools.Can.AllowMasterEnchantment = value)
            .AddCheckbox(
                () => "Allow Swift Enchantment",
                () => "Whether the Watering Can can be enchanted with Swift.",
                config => config.Tools.Can.AllowSwiftEnchantment,
                (config, value) => config.Tools.Can.AllowSwiftEnchantment = value);
    }
}
