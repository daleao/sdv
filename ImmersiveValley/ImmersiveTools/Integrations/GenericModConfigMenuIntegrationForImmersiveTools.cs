namespace DaLion.Stardew.Tools.Integrations;

#region using directives

using Common.Integrations.GenericModConfigMenu;
using Configs;
using Framework;
using HarmonyLib;
using System;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Tools.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveTools
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ToolConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public GenericModConfigMenuIntegrationForImmersiveTools(IModRegistry modRegistry, IManifest manifest,
        Func<ToolConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu = new(modRegistry, manifest, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        var allowedUpgrades = new[] { "Copper", "Steel", "Gold", "Iridium" };
        if (ModEntry.IsMoonMisadventuresLoaded) allowedUpgrades.AddRangeToArray(new[] { "Radioactive", "Mythicite" });

        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()

            // general
            .AddSectionTitle(() => "General")
            .AddCheckbox(
                () => "Hide Affected Tiles",
                () => "Whether to hide affected tiles overlay while charging.",
                config => config.HideAffectedTiles,
                (config, value) => config.HideAffectedTiles = value
            )
            .AddNumberField(
                () => "Stamina Consumption Multiplier",
                () => "Adjusts the stamina cost of charging.",
                config => config.StaminaCostMultiplier,
                (config, value) => config.StaminaCostMultiplier = value,
                0f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Shockwave Delay",
                () => "Affects the shockwave travel speed. Lower is faster. Set to 0 for instant.",
                config => (int)config.TicksBetweenWaves,
                (config, value) => config.TicksBetweenWaves = (uint)value,
                0,
                10
            )

            // keybinds
            .AddSectionTitle(() => "Controls")
            .AddCheckbox(
                () => "Require Modkey",
                () => "Whether charging requires holding down a mod key.",
                config => config.RequireModkey,
                (config, value) => config.RequireModkey = value
            )
            .AddKeyBinding(
                () => "Charging Modkey",
                () => "If 'RequireModkey' is enabled, you must hold this key to begin charging.",
                config => config.Modkey,
                (config, value) => config.Modkey = value
            )

            // page links
            .AddPageLink("axe", () => "Axe Settings", () => "Go to Axe settings.")
            .AddPageLink("pickaxe", () => "Pickaxe Settings", () => "Go to Pickaxe settings.")
            .AddPageLink("hoe", () => "Hoe Settings", () => "Go to Hoe settings.")
            .AddPageLink("can", () => "Watering Can Settings", () => "Go to Watering Can settings.")

            // axe settings
            .AddPage("axe", () => "Axe Settings")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Enable Axe Charging",
                () => "Enables charging the Axe.",
                config => config.AxeConfig.EnableCharging,
                (config, value) => config.AxeConfig.EnableCharging = value
            )
            .AddDropdown(
                () => "Min. Upgrade For Charging",
                () => "Your Axe must be at least this level in order to charge.",
                config => config.AxeConfig.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.AxeConfig.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => value
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[0],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[0] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[1],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[1] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[2],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[2] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = value,
                1,
                10
            );

        if (ModEntry.IsMoonMisadventuresLoaded)
            _configMenu
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Axe.",
                    config => config.AxeConfig.RadiusAtEachPowerLevel[4],
                    (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[4] = value,
                    1,
                    10
                )
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Axe.",
                    config => config.AxeConfig.RadiusAtEachPowerLevel[5],
                    (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[5] = value,
                    1,
                    10
                );

        _configMenu
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Axe with Reaching Enchantment.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[ModEntry.IsMoonMisadventuresLoaded ? 6 : 4],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[ModEntry.IsMoonMisadventuresLoaded ? 6 : 4] = value,
                1,
                10
            )
            .AddCheckbox(
                () => "Clear Fruit Tree Seeds",
                () => "Whether to clear fruit tree seeds.",
                config => config.AxeConfig.ClearFruitTreeSeeds,
                (config, value) => config.AxeConfig.ClearFruitTreeSeeds = value
            )
            .AddCheckbox(
                () => "Clear Fruit Tree Saplings",
                () => "Whether to clear fruit trees that aren't fully grown.",
                config => config.AxeConfig.ClearFruitTreeSaplings,
                (config, value) => config.AxeConfig.ClearFruitTreeSaplings = value
            )
            .AddCheckbox(
                () => "Cut Grown Fruit Trees",
                () => "Whether to cut down fully-grown fruit trees.",
                config => config.AxeConfig.CutGrownFruitTrees,
                (config, value) => config.AxeConfig.CutGrownFruitTrees = value
            )
            .AddCheckbox(
                () => "Clear Tree Seeds",
                () => "Whether to clear non-fruit tree seeds.",
                config => config.AxeConfig.ClearTreeSeeds,
                (config, value) => config.AxeConfig.ClearTreeSeeds = value
            )
            .AddCheckbox(
                () => "Clear Tree Saplings",
                () => "Whether to clear non-fruit trees that aren't fully grown.",
                config => config.AxeConfig.ClearTreeSaplings,
                (config, value) => config.AxeConfig.ClearTreeSaplings = value
            )
            .AddCheckbox(
                () => "Cut Grown Trees",
                () => "Whether to cut down fully-grown non-fruit trees.",
                config => config.AxeConfig.CutGrownTrees,
                (config, value) => config.AxeConfig.CutGrownTrees = value
            )
            .AddCheckbox(
                () => "Cut Tapped Trees",
                () => "Whether to cut down non-fruit trees that have a tapper.",
                config => config.AxeConfig.CutTappedTrees,
                (config, value) => config.AxeConfig.CutTappedTrees = value
            )
            .AddCheckbox(
                () => "Cut Giant Crops",
                () => "Whether to harvest giant crops.",
                config => config.AxeConfig.CutGiantCrops,
                (config, value) => config.AxeConfig.CutGiantCrops = value
            )
            .AddCheckbox(
                () => "Clear Bushes",
                () => "Whether to clear bushes.",
                config => config.AxeConfig.ClearBushes,
                (config, value) => config.AxeConfig.ClearBushes = value
            )
            .AddCheckbox(
                () => "Clear Live Crops",
                () => "Whether to clear live crops.",
                config => config.AxeConfig.ClearLiveCrops,
                (config, value) => config.AxeConfig.ClearLiveCrops = value
            )
            .AddCheckbox(
                () => "Clear Dead Crops",
                () => "Whether to clear dead crops.",
                config => config.AxeConfig.ClearDeadCrops,
                (config, value) => config.AxeConfig.ClearDeadCrops = value
            )
            .AddCheckbox(
                () => "Clear Debris",
                () => "Whether to clear debris like twigs, giant stumps, fallen logs and weeds.",
                config => config.AxeConfig.ClearDebris,
                (config, value) => config.AxeConfig.ClearDebris = value
            )
            .AddCheckbox(
                () => "Play Shockwave Animation",
                () => "Whether to play the shockwave animation when the charged Axe is released.",
                config => config.AxeConfig.PlayShockwaveAnimation,
                (config, value) => config.AxeConfig.PlayShockwaveAnimation = value
            )
            .AddCheckbox(
                () => "Allow Reaching Enchantment",
                () => "Whether the Axe can be enchanted with Reaching.",
                config => config.AxeConfig.AllowReachingEnchantment,
                (config, value) => config.AxeConfig.AllowReachingEnchantment = value
            )
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Axe can be enchanted with Master.",
                config => config.AxeConfig.AllowMasterEnchantment,
                (config, value) => config.AxeConfig.AllowMasterEnchantment = value
            )

            // pickaxe settings
            .AddPage("pickaxe", () => "Pickaxe Settings")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Enable Pickaxe Charging",
                () => "Enables charging the Pickxe.",
                config => config.PickaxeConfig.EnableCharging,
                (config, value) => config.PickaxeConfig.EnableCharging = value
            )
            .AddDropdown(
                () => "Min. Upgrade For Charging",
                () => "Your Pickaxe must be at least this level in order to charge.",
                config => config.PickaxeConfig.RequiredUpgradeForCharging.ToString(),
                (config, value) => config.PickaxeConfig.RequiredUpgradeForCharging = Enum.Parse<UpgradeLevel>(value),
                allowedUpgrades,
                value => value
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[0],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[0] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[1],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[1] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[2],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[2] = value,
                1,
                10
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[3] = value,
                1,
                10
            );

        if (ModEntry.IsMoonMisadventuresLoaded)
            _configMenu
                .AddNumberField(
                    () => "Radioactive Radius",
                    () => "The radius of affected tiles for the Radioactive Pickaxe.",
                    config => config.PickaxeConfig.RadiusAtEachPowerLevel[4],
                    (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[4] = value,
                    1,
                    10
                )
                .AddNumberField(
                    () => "Mythicite Radius",
                    () => "The radius of affected tiles for the Mythicite Pickaxe.",
                    config => config.PickaxeConfig.RadiusAtEachPowerLevel[5],
                    (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[5] = value,
                    1,
                    10
                );

        _configMenu
            .AddNumberField(
                () => "Reaching Radius",
                () => "The radius of affected tiles for the Pickaxe with Reaching Enchantment.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[ModEntry.IsMoonMisadventuresLoaded ? 6 : 4],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[ModEntry.IsMoonMisadventuresLoaded ? 6 : 4] = value,
                1,
                10
            )
            .AddCheckbox(
                () => "Break Boulders and Meteorites",
                () => "Whether to break boulders and meteorites.",
                config => config.PickaxeConfig.BreakBouldersAndMeteorites,
                (config, value) => config.PickaxeConfig.BreakBouldersAndMeteorites = value
            )
            .AddCheckbox(
                () => "Harvest Mine Spawns",
                () => "Whether to harvest spawned items in the mines.",
                config => config.PickaxeConfig.HarvestMineSpawns,
                (config, value) => config.PickaxeConfig.HarvestMineSpawns = value
            )
            .AddCheckbox(
                () => "Break Mine Containers",
                () => "Whether to break containers in the mine.",
                config => config.PickaxeConfig.BreakMineContainers,
                (config, value) => config.PickaxeConfig.BreakMineContainers = value
            )
            .AddCheckbox(
                () => "Clear Objects",
                () => "Whether to clear placed objects.",
                config => config.PickaxeConfig.ClearObjects,
                (config, value) => config.PickaxeConfig.ClearObjects = value
            )
            .AddCheckbox(
                () => "Clear Flooring",
                () => "Whether to clear placed paths & flooring.",
                config => config.PickaxeConfig.ClearFlooring,
                (config, value) => config.PickaxeConfig.ClearFlooring = value
            )
            .AddCheckbox(
                () => "Clear Dirt",
                () => "Whether to clear tilled dirt.",
                config => config.PickaxeConfig.ClearDirt,
                (config, value) => config.PickaxeConfig.ClearDirt = value
            )
            .AddCheckbox(
                () => "Clear Live Crops",
                () => "Whether to clear live crops.",
                config => config.PickaxeConfig.ClearLiveCrops,
                (config, value) => config.PickaxeConfig.ClearLiveCrops = value
            )
            .AddCheckbox(
                () => "Clear Dead Crops",
                () => "Whether to clear dead crops.",
                config => config.PickaxeConfig.ClearDeadCrops,
                (config, value) => config.PickaxeConfig.ClearDeadCrops = value
            )
            .AddCheckbox(
                () => "Clear Debris",
                () => "Whether to clear debris like stones, boulders and weeds.",
                config => config.PickaxeConfig.ClearDebris,
                (config, value) => config.PickaxeConfig.ClearDebris = value
            )
            .AddCheckbox(
                () => "Play Shockwave Animation",
                () => "Whether to play the shockwave animation when the charged Pickaxe is released.",
                config => config.PickaxeConfig.PlayShockwaveAnimation,
                (config, value) => config.PickaxeConfig.PlayShockwaveAnimation = value
            )
            .AddCheckbox(
                () => "Allow Reaching Enchantment",
                () => "Whether the Pickaxe can be enchanted with Reaching.",
                config => config.PickaxeConfig.AllowReachingEnchantment,
                (config, value) => config.PickaxeConfig.AllowReachingEnchantment = value
            )
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Pickaxe can be enchanted with Master.",
                config => config.PickaxeConfig.AllowMasterEnchantment,
                (config, value) => config.PickaxeConfig.AllowMasterEnchantment = value
            )

            // hoe settings
            .AddPage("hoe", () => "Hoe Settings")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () => "Whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve performance.",
                config => config.HoeConfig.OverrideAffectedTiles,
                (config, value) => config.HoeConfig.OverrideAffectedTiles = value
            )
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Hoe.",
                config => config.HoeConfig.AffectedTiles[0][0],
                (config, value) => config.HoeConfig.AffectedTiles[0][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Hoe.",
                config => config.HoeConfig.AffectedTiles[0][1],
                (config, value) => config.HoeConfig.AffectedTiles[0][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Hoe.",
                config => config.HoeConfig.AffectedTiles[1][0],
                (config, value) => config.HoeConfig.AffectedTiles[1][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Hoe.",
                config => config.HoeConfig.AffectedTiles[1][1],
                (config, value) => config.HoeConfig.AffectedTiles[1][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Hoe.",
                config => config.HoeConfig.AffectedTiles[2][0],
                (config, value) => config.HoeConfig.AffectedTiles[2][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Hoe.",
                config => config.HoeConfig.AffectedTiles[2][1],
                (config, value) => config.HoeConfig.AffectedTiles[2][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Hoe.",
                config => config.HoeConfig.AffectedTiles[3][0],
                (config, value) => config.HoeConfig.AffectedTiles[3][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Hoe.",
                config => config.HoeConfig.AffectedTiles[3][1],
                (config, value) => config.HoeConfig.AffectedTiles[3][1] = value,
                0,
                7
            );

        switch (ModEntry.IsMoonMisadventuresLoaded)
        {
            case false:
                _configMenu
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                        config => config.HoeConfig.AffectedTiles[4][0],
                        (config, value) => config.HoeConfig.AffectedTiles[4][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Reaching Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                        config => config.HoeConfig.AffectedTiles[4][1],
                        (config, value) => config.HoeConfig.AffectedTiles[4][1] = value,
                        0,
                        7
                    );
                break;
            case true:
                _configMenu
                    .AddNumberField(
                        () => "Radioactive Length",
                        () => "The length of affected tiles for the Radioactive Hoe.",
                        config => config.HoeConfig.AffectedTiles[4][0],
                        (config, value) => config.HoeConfig.AffectedTiles[4][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Radioactive Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Radioactive Hoe.",
                        config => config.HoeConfig.AffectedTiles[4][1],
                        (config, value) => config.HoeConfig.AffectedTiles[4][1] = value,
                        0,
                        7
                    )
                    .AddNumberField(
                        () => "Mythicite Length",
                        () => "The length of affected tiles for the Mythicite Hoe.",
                        config => config.HoeConfig.AffectedTiles[5][0],
                        (config, value) => config.HoeConfig.AffectedTiles[5][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Mythicite Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Mythicite Hoe.",
                        config => config.HoeConfig.AffectedTiles[5][1],
                        (config, value) => config.HoeConfig.AffectedTiles[5][1] = value,
                        0,
                        7
                    )
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Hoe when Reaching Enchantment is applied.",
                        config => config.HoeConfig.AffectedTiles[6][0],
                        (config, value) => config.HoeConfig.AffectedTiles[6][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Reaching Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Hoe when Reaching Enchantment is applied.",
                        config => config.HoeConfig.AffectedTiles[6][1],
                        (config, value) => config.HoeConfig.AffectedTiles[6][1] = value,
                        0,
                        7
                    );
                break;
        }

        _configMenu
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Hoe can be enchanted with Master.",
                config => config.HoeConfig.AllowMasterEnchantment,
                (config, value) => config.HoeConfig.AllowMasterEnchantment = value
            )

            // can settings
            .AddPage("can", () => "Watering Can Settings")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Override Affected Tiles",
                () => "Whether to apply custom tile area for the Watering Can. Keep this at false if using defaults to improve performance.",
                config => config.WateringCanConfig.OverrideAffectedTiles,
                (config, value) => config.WateringCanConfig.OverrideAffectedTiles = value
            )
            .AddNumberField(
                () => "Copper Length",
                () => "The length of affected tiles for the Copper Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[0][0],
                (config, value) => config.WateringCanConfig.AffectedTiles[0][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles to either side of the farmer for the Copper Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[0][1],
                (config, value) => config.WateringCanConfig.AffectedTiles[0][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Steel Length",
                () => "The length of affected tiles for the Steel Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[1][0],
                (config, value) => config.WateringCanConfig.AffectedTiles[1][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles to either side of the farmer for the Steel Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[1][1],
                (config, value) => config.WateringCanConfig.AffectedTiles[1][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Gold Length",
                () => "The length of affected tiles for the Gold Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[2][0],
                (config, value) => config.WateringCanConfig.AffectedTiles[2][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles to either side of the farmer for the Gold Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[2][1],
                (config, value) => config.WateringCanConfig.AffectedTiles[2][1] = value,
                0,
                7
            )
            .AddNumberField(
                () => "Iridium Length",
                () => "The length of affected tiles for the Iridium Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[3][0],
                (config, value) => config.WateringCanConfig.AffectedTiles[3][0] = value,
                1,
                15
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles to either side of the farmer for the Iridium Watering Can.",
                config => config.WateringCanConfig.AffectedTiles[3][1],
                (config, value) => config.WateringCanConfig.AffectedTiles[3][1] = value,
                0,
                7
            );

        switch (ModEntry.IsMoonMisadventuresLoaded)
        {
            case false:
                _configMenu
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                        config => config.WateringCanConfig.AffectedTiles[4][0],
                        (config, value) => config.WateringCanConfig.AffectedTiles[4][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Reaching Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                        config => config.WateringCanConfig.AffectedTiles[4][1],
                        (config, value) => config.WateringCanConfig.AffectedTiles[4][1] = value,
                        0,
                        7
                    );
                break;
            case true:
                _configMenu
                    .AddNumberField(
                        () => "Radioactive Length",
                        () => "The length of affected tiles for the Radioactive Watering Can.",
                        config => config.WateringCanConfig.AffectedTiles[4][0],
                        (config, value) => config.WateringCanConfig.AffectedTiles[4][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Radioactive Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Radioactive Watering Can.",
                        config => config.WateringCanConfig.AffectedTiles[4][1],
                        (config, value) => config.WateringCanConfig.AffectedTiles[4][1] = value,
                        0,
                        7
                    )
                    .AddNumberField(
                        () => "Mythicite Length",
                        () => "The length of affected tiles for the Mythicite Watering Can.",
                        config => config.WateringCanConfig.AffectedTiles[5][0],
                        (config, value) => config.WateringCanConfig.AffectedTiles[5][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Mythicite Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Mythicite Watering Can.",
                        config => config.WateringCanConfig.AffectedTiles[5][1],
                        (config, value) => config.WateringCanConfig.AffectedTiles[5][1] = value,
                        0,
                        7
                    )
                    .AddNumberField(
                        () => "Enchanted Length",
                        () => "The length of affected tiles for the Watering Can when Reaching Enchantment is applied.",
                        config => config.WateringCanConfig.AffectedTiles[6][0],
                        (config, value) => config.WateringCanConfig.AffectedTiles[6][0] = value,
                        1,
                        15
                    )
                    .AddNumberField(
                        () => "Reaching Radius",
                        () => "The radius of affected tiles to either side of the farmer for the Watering Can when Reaching Enchantment is applied.",
                        config => config.WateringCanConfig.AffectedTiles[6][1],
                        (config, value) => config.WateringCanConfig.AffectedTiles[6][1] = value,
                        0,
                        7
                    );
                break;
        }

        _configMenu
            .AddCheckbox(
                () => "Allow Master Enchantment",
                () => "Whether the Watering Can can be enchanted with Master.",
                config => config.WateringCanConfig.AllowMasterEnchantment,
                (config, value) => config.WateringCanConfig.AllowMasterEnchantment = value
            )
            .AddCheckbox(
                () => "Allow Swift Enchantment",
                () => "Whether the Watering Can can be enchanted with Swift.",
                config => config.WateringCanConfig.AllowSwiftEnchantment,
                (config, value) => config.WateringCanConfig.AllowSwiftEnchantment = value
            );
    }
}