using System.Reflection;
using Microsoft.Xna.Framework;

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

using TheLion.AwesomeTools.Framework;

namespace TheLion.AwesomeTools
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static IReflectionHelper Reflection { get; set; }
		public static ModConfig Config { get; set; }
		public static bool IsDoingShockwave { get; set; } = false;

		private EffectsManager _manager;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get configs.json
			Config = Helper.ReadConfig<ModConfig>();

			// get reflection interface
			Reflection = Helper.Reflection;

			// hook events
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.Input.ButtonReleased += OnInputButtonReleased;

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
			_manager = new EffectsManager(Config, Helper.ModRegistry);

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
				modRegistry: Helper.ModRegistry,
				monitor: Monitor,
				manifest: ModManifest
			).Register();
		}

		/// <summary>Raised after the player pressed/released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnInputButtonReleased(object sender, ButtonReleasedEventArgs e)
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
				_ => -1
			};

			if (upgradeLevel < 0)
			{
				if (int.TryParse(args[0], out int i) && i <= 4)
				{
					upgradeLevel = i;
				}
				else
				{
					Monitor.Log("Invalid argument." + PrintCommandUsage(), LogLevel.Info);
					return;
				}
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
			return "\n\nUsage: player_upgradetools < level >\n - level: one of 'copper', 'steel', 'gold', 'iridium'";
		}

	}
}
