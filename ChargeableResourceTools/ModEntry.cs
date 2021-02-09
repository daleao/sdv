using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System.Linq;
using System.Reflection;
using TheLion.AwesomeTools.Framework;

namespace TheLion.AwesomeTools
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static IModRegistry ModRegistry { get; set; }
		public static IReflectionHelper Reflection { get; set; }
		public static ModConfig Config { get; set; }
		public static bool IsDoingShockwave { get; set; } = false;

		private EffectsManager _manager;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get and verify configs.json
			Config = Helper.ReadConfig<ModConfig>();
			VerifyModConfig();

			// get mod registry
			ModRegistry = Helper.ModRegistry;

			// get reflection interface
			Reflection = Helper.Reflection;

			// hook events
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.Input.ButtonReleased += OnButtonReleased;

			// create and patch Harmony instance
			var harmony = HarmonyInstance.Create("thelion.AwesomeTools");
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			// add commands for debugging (or cheating)
			Helper.ConsoleCommands.Add("player_settoolsupgrade", "Set the upgrade level of all upgradeable tools in the player's inventory." + PrintCommandUsage(), SetToolsUpgrade);
		}

		/// <summary>The event called after the first game update, once all mods are loaded.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// instantiate awesome tool effects
			_manager = new EffectsManager(Config, ModRegistry);

			// add Generic Mod Config Menu integration
			new GenericModConfigMenuIntegrationForAwesomeTools(
				getConfig: () => Config,
				reset: () =>
				{
					Config = new ModConfig();
					Helper.WriteConfig(Config);
				},
				saveAndApply: () =>
				{
					Helper.WriteConfig(Config);
				},
				modRegistry: ModRegistry,
				monitor: Monitor,
				manifest: ModManifest
			).Register();
		}

		/// <summary>Raised after the player pressed/released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			if (Game1.activeClickableMenu != null || !e.Button.IsUseToolButton())
			{
				return;
			}

			Farmer who = Game1.player;
			Tool tool = who.CurrentTool;
			if ((tool is Axe || tool is Pickaxe) && who.toolPower > 0)
			{
				GameLocation location = Game1.currentLocation;
				Vector2 actionTile = new Vector2((int)(who.GetToolLocation().X / Game1.tileSize), (int)(who.GetToolLocation().Y / Game1.tileSize));
				_manager.DoShockwaveEffect(actionTile, tool, location, who);
			}
		}

		private void VerifyModConfig()
		{
			if (Config.AxeConfig.RadiusAtEachLevel.Any(i => i < 0))
			{
				Monitor.Log("Found illegal negative value for shockwave radius in configs.json AxeConfig.RadiusAtEachLevel. Default values will be restored.", LogLevel.Warn);
				Config.AxeConfig.RadiusAtEachLevel = new int[] { 1, 2, 3, 4, 5 };
				Helper.WriteConfig(Config);
			}
			else if (Config.AxeConfig.RadiusAtEachLevel.Length > 5)
			{
				Monitor.Log("Too many values in configs.json AxeConfig.RadiusAtEachLevel. Default values will be restored.", LogLevel.Warn);
				Config.AxeConfig.RadiusAtEachLevel = new int[] { 1, 2, 3, 4, 5 };
				Helper.WriteConfig(Config);
			}
			
			if (Config.PickaxeConfig.RadiusAtEachLevel.Any(i => i < 0))
			{
				Monitor.Log("Found illegal negative value for shockwave radius in configs.json PickaxeConfig.RadiusAtEachLevel. Default values will be restored.", LogLevel.Warn);
				Config.PickaxeConfig.RadiusAtEachLevel = new int[] { 1, 2, 3, 4, 5 };
				Helper.WriteConfig(Config);
			}
			else if (Config.PickaxeConfig.RadiusAtEachLevel.Length > 5)
			{
				Monitor.Log("Too many values in configs.json PickaxeConfig.RadiusAtEachLevel. Default values will be restored.", LogLevel.Warn);
				Config.PickaxeConfig.RadiusAtEachLevel = new int[] { 1, 2, 3, 4, 5 };
				Helper.WriteConfig(Config);
			}
		}

		/// <summary>Set the upgrade level of all upgradeable tools in the player's inventory.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments.</param>
		private void SetToolsUpgrade(string command, string[] args)
		{
			if (args.Length < 1)
			{
				Monitor.Log("Missing argument.", LogLevel.Info);
				return;
			}

			int upgradeLevel = args[0] switch
			{
				"copper" => 1,
				"steel" => 2,
				"gold" => 3,
				"iridium" => 4,
				"prismatic" => 5,
				"radioactive" => 5,
				_ => -1
			};

			if (upgradeLevel < 0)
			{
				if (int.TryParse(args[0], out int i) && i <= 5)
				{
					upgradeLevel = i;
				}
				else
				{
					Monitor.Log("Invalid argument." + PrintCommandUsage(), LogLevel.Info);
					return;
				}
			}

			if (upgradeLevel == 5 && !(ModRegistry.IsLoaded("stokastic.PrismaticTools") || ModRegistry.IsLoaded("kakashigr.RadioactiveTools")))
			{
				Monitor.Log("Could not find 'Prismatic Tools' or 'Radioactive Tools' in the current mod registry.", LogLevel.Info);
				return;
			}

			foreach (Item item in Game1.player.Items)
			{
				if (item is Axe || item is Hoe || item is Pickaxe || item is WateringCan)
				{
					(item as Tool).UpgradeLevel = upgradeLevel;
				}
			}
		}

		private string PrintCommandUsage()
		{
			string result = "\n\nUsage: player_upgradetools < level >\n - level: one of 'copper', 'steel', 'gold', 'iridium'";
			if (ModRegistry.IsLoaded("stokastic.PrismaticTools"))
			{
				result += ", 'prismatic'";
			}
			else if (ModRegistry.IsLoaded("kakashigr.RadioactiveTools"))
			{
				result += ", 'radioactive'";
			}
			
			return result;
		}
	}
}
