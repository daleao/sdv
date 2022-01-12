using System;
using StardewModdingAPI;
using TheLion.Stardew.Common.Integrations;
using TheLion.Stardew.Tools.Configs;

namespace TheLion.Stardew.Tools.Integrations;

/// <summary>Constructs the GenericModConfigMenu integration for Awesome Tools.</summary>
internal class GenericModConfigMenuIntegrationForAwesomeTools
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ToolConfig> _configMenu;

    /// <summary>API for fetching metadata about loaded mods.</summary>
    private readonly IModRegistry _modRegistry;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public GenericModConfigMenuIntegrationForAwesomeTools(IModRegistry modRegistry, IManifest manifest,
        Func<ToolConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
    {
        _modRegistry = modRegistry;
        _configMenu = new(modRegistry, manifest, log, getConfig, reset, saveAndApply);
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        // get config menu
        if (!_configMenu.IsLoaded)
            return;

        // register
        _configMenu
            .Register()

            // main
            .AddSectionTitle(() => "Main")
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
                config => (int) config.TicksBetweenWaves,
                (config, value) => config.TicksBetweenWaves = (uint) value,
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
            .AddPageLink("axe", () => "Axe Options", () => "Go to Axe options")
            .AddPageLink("pickaxe", () => "Pickaxe Options", () => "Go to Pickaxe options")

            // axe options
            .AddPage("axe", () => "Axe Options")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Enable Axe Charging",
                () => "Enables charging the Axe.",
                config => config.AxeConfig.EnableAxeCharging,
                (config, value) => config.AxeConfig.EnableAxeCharging = value
            )
            .AddNumberField(
                () => "Required Upgrade Level",
                () => "Your Axe must be at least this level in order to charge.",
                config => config.AxeConfig.RequiredUpgradeForCharging,
                (config, value) => config.AxeConfig.RequiredUpgradeForCharging = value,
                0,
                5
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[0],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[0] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[1],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[1] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[2],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[2] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Axe.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = value,
                0,
                10
            );

        if (ModEntry.HasToolMod)
            _configMenu.AddNumberField(
                () => ModEntry.ToolMod + " Radius",
                () => "The radius of affected tiles if using mods like Prismatic or Radioactive Tools.",
                config => config.AxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = value,
                0,
                10
            );

        _configMenu
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

            // pickaxe options
            .AddPage("pickaxe", () => "Pickaxe Options")
            .AddPageLink(string.Empty, () => "Back to Main Page")
            .AddCheckbox(
                () => "Enable Pickaxe Charging",
                () => "Enables charging the Pickxe.",
                config => config.PickaxeConfig.EnablePickaxeCharging,
                (config, value) => config.PickaxeConfig.EnablePickaxeCharging = value
            )
            .AddNumberField(
                () => "Required Upgrade Level",
                () => "Your Pickaxe must be at least this level in order to charge.",
                config => config.PickaxeConfig.RequiredUpgradeForCharging,
                (config, value) => config.PickaxeConfig.RequiredUpgradeForCharging = value,
                0,
                5
            )
            .AddNumberField(
                () => "Copper Radius",
                () => "The radius of affected tiles for the Copper Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[0],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[0] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Steel Radius",
                () => "The radius of affected tiles for the Steel Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[1],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[1] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Gold Radius",
                () => "The radius of affected tiles for the Gold Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[2],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[2] = value,
                0,
                10
            )
            .AddNumberField(
                () => "Iridium Radius",
                () => "The radius of affected tiles for the Iridium Pickaxe.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[3] = value,
                0,
                10
            );

        if (ModEntry.HasToolMod)
            _configMenu.AddNumberField(
                () => ModEntry.ToolMod + " Radius",
                () => "The radius of affected tiles if using mods like Prismatic or Radioactive Tools.",
                config => config.PickaxeConfig.RadiusAtEachPowerLevel[3],
                (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[3] = value,
                0,
                10
            );

        _configMenu
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
                () => "Clear Bushes",
                () => "Whether to clear bushes.",
                config => config.PickaxeConfig.ClearBushes,
                (config, value) => config.PickaxeConfig.ClearBushes = value
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
            );
    }
}