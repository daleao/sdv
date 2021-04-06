using StardewModdingAPI;
using System;
using TheLion.Common.Integrations;

namespace TheLion.AwesomeTools
{
	/// <summary>Constructs the GenericModConfigMenu integration for Awesome Tools.</summary>
	internal class GenericModConfigMenuIntegrationForAwesomeTools
	{
		/// <summary>The Generic Mod Config Menu integration.</summary>
		private readonly GenericModConfigMenuIntegration<ToolConfig> _configMenu;

		/// <summary>API for fetching metadata about loaded mods.</summary>
		private readonly IModRegistry _modRegistry;

		/// <summary>Construct an instance.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		/// <param name="monitor">Encapsulates monitoring and logging.</param>
		/// <param name="manifest">The mod manifest.</param>
		/// <param name="getConfig">Get the current config model.</param>
		/// <param name="reset">Reset the config model to the default values.</param>
		/// <param name="saveAndApply">Save and apply the current config model.</param>
		public GenericModConfigMenuIntegrationForAwesomeTools(IModRegistry modRegistry, IMonitor monitor, IManifest manifest, Func<ToolConfig> getConfig, Action reset, Action saveAndApply)
		{
			_modRegistry = modRegistry;
			_configMenu = new GenericModConfigMenuIntegration<ToolConfig>(modRegistry, monitor, manifest, getConfig, reset, saveAndApply);
		}

		/// <summary>Register the config menu if available.</summary>
		public void Register()
		{
			// get config menu
			var menu = _configMenu;
			if (!menu.IsLoaded)
				return;

			// register
			menu
				.RegisterConfig()

				// main
				.AddLabel("Main")
				.AddNumberField(
					label: "Stamina Consumption Multiplier",
					description: "Adjusts the stamina cost of charging.",
					get: config => config.StaminaCostMultiplier,
					set: (config, value) => config.StaminaCostMultiplier = (float)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Shockwave Delay",
					description: "The delay between releasing the tool button and triggering the shockwave. Adjust this if you find that the shockwave happens to soon or too late.",
					get: config => config.ShockwaveDelay,
					set: (config, value) => config.ShockwaveDelay = (int)value,
					min: 0,
					max: 300
				)

				// keybinds
				.AddLabel("Controls")
				.AddCheckbox(
					label: "Require Modkey",
					description: "Whether to require an additional Modkey to start charging.",
					get: config => config.RequireModkey,
					set: (config, value) => config.RequireModkey = value
				)
				.AddKeyBinding(
					label: "Charging Modkey",
					description: "If 'RequireModkey' is enabled, press this Modkey to allow charging.",
					get: config => config.Modkey,
					set: (config, value) => config.Modkey = value
				)
				.AddPageLabel("Go to Axe options", page: "Axe Options")
				.AddPageLabel("Go to Pickaxe options", page: "Pickaxe Options")

				// axe options
				.AddNewPage("Axe Options")
				.AddPageLabel("Back to main page")
				.AddCheckbox(
					label: "Enable Axe Charging",
					description: "Enables charging the Axe.",
					get: config => config.AxeConfig.EnableAxeCharging,
					set: (config, value) => config.AxeConfig.EnableAxeCharging = value
				)
				.AddNumberField(
					label: "Required Upgrade Level",
					description: "Your Axe must be at least this level in order to charge.",
					get: config => config.AxeConfig.RequiredUpgradeForCharging,
					set: (config, value) => config.AxeConfig.RequiredUpgradeForCharging = (int)value,
					min: 0,
					max: 5
				)
				.AddNumberField(
					label: "Copper Radius",
					description: "The radius of affected tiles for the Copper Axe.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[0],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[0] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Steel Radius",
					description: "The radius of affected tiles for the Steel Axe.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[1],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[1] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Gold Radius",
					description: "The radius of affected tiles for the Gold Axe.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[2],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[2] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Iridium Radius",
					description: "The radius of affected tiles for the Iridium Axe.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[3],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = (int)value,
					min: 0,
					max: 10
				);

			if (Utility.HasHigherLevelToolMod(_modRegistry, out string whichMod))
			{
				menu.AddNumberField(
					label: whichMod + " Radius",
					description: "The radius of affected tiles if using mods like Prismatic or Radioactive Tools.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[3],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = (int)value,
					min: 0,
					max: 10
				);
			}

			menu
				.AddCheckbox(
					label: "Show Axe Affected Tiles",
					description: "Whether to show affected tiles overlay while charging.",
					get: config => config.AxeConfig.ShowAxeAffectedTiles,
					set: (config, value) => config.AxeConfig.ShowAxeAffectedTiles = value
				)
				.AddCheckbox(
					label: "Clear Fruit Tree Seeds",
					description: "Whether to clear fruit tree seeds.",
					get: config => config.AxeConfig.ClearFruitTreeSeeds,
					set: (config, value) => config.AxeConfig.ClearFruitTreeSeeds = value
				)
				.AddCheckbox(
					label: "Clear Fruit Tree Saplings",
					description: "Whether to clear fruit trees that aren't fully grown.",
					get: config => config.AxeConfig.ClearFruitTreeSaplings,
					set: (config, value) => config.AxeConfig.ClearFruitTreeSaplings = value
				)
				.AddCheckbox(
					label: "Cut Grown Fruit Trees",
					description: "Whether to cut down fully-grown fruit trees.",
					get: config => config.AxeConfig.CutGrownFruitTrees,
					set: (config, value) => config.AxeConfig.CutGrownFruitTrees = value
				)
				.AddCheckbox(
					label: "Clear Tree Seeds",
					description: "Whether to clear non-fruit tree seeds.",
					get: config => config.AxeConfig.ClearTreeSeeds,
					set: (config, value) => config.AxeConfig.ClearTreeSeeds = value
				)
				.AddCheckbox(
					label: "Clear Tree Saplings",
					description: "Whether to clear non-fruit trees that aren't fully grown.",
					get: config => config.AxeConfig.ClearTreeSaplings,
					set: (config, value) => config.AxeConfig.ClearTreeSaplings = value
				)
				.AddCheckbox(
					label: "Cut Grown Trees",
					description: "Whether to cut down fully-grown non-fruit trees.",
					get: config => config.AxeConfig.CutGrownTrees,
					set: (config, value) => config.AxeConfig.CutGrownTrees = value
				)
				.AddCheckbox(
					label: "Cut Tapped Trees",
					description: "Whether to cut down non-fruit trees that have a tapper.",
					get: config => config.AxeConfig.CutTappedTrees,
					set: (config, value) => config.AxeConfig.CutTappedTrees = value
				)
				.AddCheckbox(
					label: "Cut Giant Crops",
					description: "Whether to harvest giant crops.",
					get: config => config.AxeConfig.CutGiantCrops,
					set: (config, value) => config.AxeConfig.CutGiantCrops = value
				)
				.AddCheckbox(
					label: "Clear Bushes",
					description: "Whether to clear bushes.",
					get: config => config.AxeConfig.ClearBushes,
					set: (config, value) => config.AxeConfig.ClearBushes = value
				)
				.AddCheckbox(
					label: "Clear Live Crops",
					description: "Whether to clear live crops.",
					get: config => config.AxeConfig.ClearLiveCrops,
					set: (config, value) => config.AxeConfig.ClearLiveCrops = value
				)
				.AddCheckbox(
					label: "Clear Dead Crops",
					description: "Whether to clear dead crops.",
					get: config => config.AxeConfig.ClearDeadCrops,
					set: (config, value) => config.AxeConfig.ClearDeadCrops = value
				)
				.AddCheckbox(
					label: "Clear Debris",
					description: "Whether to clear debris like twigs, giant stumps, fallen logs and weeds.",
					get: config => config.AxeConfig.ClearDebris,
					set: (config, value) => config.AxeConfig.ClearDebris = value
				)

				// pickaxe options
				.AddNewPage("Pickaxe Options")
				.AddPageLabel("Back to main page")
				.AddCheckbox(
					label: "Enable Pickaxe Charging",
					description: "Enables charging the Pickxe.",
					get: config => config.PickaxeConfig.EnablePickaxeCharging,
					set: (config, value) => config.PickaxeConfig.EnablePickaxeCharging = value
				)
				.AddNumberField(
					label: "Required Upgrade Level",
					description: "Your Pickaxe must be at least this level in order to charge.",
					get: config => config.PickaxeConfig.RequiredUpgradeForCharging,
					set: (config, value) => config.PickaxeConfig.RequiredUpgradeForCharging = (int)value,
					min: 0,
					max: 5
				)
				.AddNumberField(
					label: "Copper Radius",
					description: "The radius of affected tiles for the Copper Pickaxe.",
					get: config => config.PickaxeConfig.RadiusAtEachPowerLevel[0],
					set: (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[0] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Steel Radius",
					description: "The radius of affected tiles for the Steel Pickaxe.",
					get: config => config.PickaxeConfig.RadiusAtEachPowerLevel[1],
					set: (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[1] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Gold Radius",
					description: "The radius of affected tiles for the Gold Pickaxe.",
					get: config => config.PickaxeConfig.RadiusAtEachPowerLevel[2],
					set: (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[2] = (int)value,
					min: 0,
					max: 10
				)
				.AddNumberField(
					label: "Iridium Radius",
					description: "The radius of affected tiles for the Iridium Pickaxe.",
					get: config => config.PickaxeConfig.RadiusAtEachPowerLevel[3],
					set: (config, value) => config.PickaxeConfig.RadiusAtEachPowerLevel[3] = (int)value,
					min: 0,
					max: 10
				);

			if (Utility.HasHigherLevelToolMod(_modRegistry, out whichMod))
			{
				menu.AddNumberField(
					label: whichMod + " Radius",
					description: "The radius of affected tiles if using mods like Prismatic or Radioactive Tools.",
					get: config => config.AxeConfig.RadiusAtEachPowerLevel[3],
					set: (config, value) => config.AxeConfig.RadiusAtEachPowerLevel[3] = (int)value,
					min: 0,
					max: 10
				);
			}

			menu
				.AddCheckbox(
					label: "Show Pickaxe Affected Tiles",
					description: "Whether to show affected tiles overlay while charging.",
					get: config => config.PickaxeConfig.ShowPickaxeAffectedTiles,
					set: (config, value) => config.PickaxeConfig.ShowPickaxeAffectedTiles = value
				)
				.AddCheckbox(
					label: "Break Boulders and Meteorites",
					description: "Whether to break boulders and meteorites.",
					get: config => config.PickaxeConfig.BreakBouldersAndMeteorites,
					set: (config, value) => config.PickaxeConfig.BreakBouldersAndMeteorites = value
				)
				.AddCheckbox(
					label: "Harvest Mine Spawns",
					description: "Whether to harvest spawned items in the mines.",
					get: config => config.PickaxeConfig.HarvestMineSpawns,
					set: (config, value) => config.PickaxeConfig.HarvestMineSpawns = value
				)
				.AddCheckbox(
					label: "Break Mine Containers",
					description: "Whether to break containers in the mine.",
					get: config => config.PickaxeConfig.BreakMineContainers,
					set: (config, value) => config.PickaxeConfig.BreakMineContainers = value
				)
				.AddCheckbox(
					label: "Clear Objects",
					description: "Whether to clear placed objects.",
					get: config => config.PickaxeConfig.ClearObjects,
					set: (config, value) => config.PickaxeConfig.ClearObjects = value
				)
				.AddCheckbox(
					label: "Clear Flooring",
					description: "Whether to clear placed paths & flooring.",
					get: config => config.PickaxeConfig.ClearFlooring,
					set: (config, value) => config.PickaxeConfig.ClearFlooring = value
				)
				.AddCheckbox(
					label: "Clear Dirt",
					description: "Whether to clear tilled dirt.",
					get: config => config.PickaxeConfig.ClearDirt,
					set: (config, value) => config.PickaxeConfig.ClearDirt = value
				)
				.AddCheckbox(
					label: "Clear Bushes",
					description: "Whether to clear bushes.",
					get: config => config.PickaxeConfig.ClearBushes,
					set: (config, value) => config.PickaxeConfig.ClearBushes = value
				)
				.AddCheckbox(
					label: "Clear Live Crops",
					description: "Whether to clear live crops.",
					get: config => config.PickaxeConfig.ClearLiveCrops,
					set: (config, value) => config.PickaxeConfig.ClearLiveCrops = value
				)
				.AddCheckbox(
					label: "Clear Dead Crops",
					description: "Whether to clear dead crops.",
					get: config => config.PickaxeConfig.ClearDeadCrops,
					set: (config, value) => config.PickaxeConfig.ClearDeadCrops = value
				)
				.AddCheckbox(
					label: "Clear Debris",
					description: "Whether to clear debris like stones, boulders and weeds.",
					get: config => config.PickaxeConfig.ClearDebris,
					set: (config, value) => config.PickaxeConfig.ClearDebris = value
				);
		}
	}
}