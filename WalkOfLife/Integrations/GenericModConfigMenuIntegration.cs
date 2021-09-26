using StardewModdingAPI;
using System;
using TheLion.Stardew.Common.Integrations;

namespace TheLion.Stardew.Professions.Integrations
{
	/// <summary>Constructs the GenericModConfigMenu integration for Awesome Tools.</summary>
	internal class GenericModConfigMenuIntegrationForAwesomeTools
	{
		/// <summary>The Generic Mod Config Menu integration.</summary>
		private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

		/// <summary>Construct an instance.</summary>
		/// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
		/// <param name="manifest">The mod manifest.</param>
		/// <param name="getConfig">Get the current config model.</param>
		/// <param name="reset">Reset the config model to the default values.</param>
		/// <param name="saveAndApply">Save and apply the current config model.</param>
		/// <param name="log">Encapsulates monitoring and logging.</param>
		public GenericModConfigMenuIntegrationForAwesomeTools(IModRegistry modRegistry, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
		{
			_configMenu = new GenericModConfigMenuIntegration<ModConfig>(modRegistry, manifest, getConfig, reset, saveAndApply, log);
		}

		/// <summary>Register the config menu if available.</summary>
		public void Register()
		{
			// get config menu
			if (!_configMenu.IsLoaded)
				return;

			// register
			_configMenu
				.RegisterConfig()

				// main mod settings
				.AddLabel("Mod Settings")
				.AddKeyBinding(
					label: "Mod Key",
					description: "The key used by Prospector and Scavenger professions.",
					get: config => config.Modkey,
					set: (config, value) => config.Modkey = value
				)
				.AddCheckbox(
					label: "Enable IL code export",
					description: "If you get a 'failed to patch' error, enable this option and send me the output file along with your bug report.",
					get: config => config.EnableILCodeExport,
					set: (config, value) => config.EnableILCodeExport = value
				)

				// super mode
				.AddLabel("Super Mode Settings")
				.AddKeyBinding(
					label: "Super Mode key",
					description: "The key used to activate Super Mode.",
					get: config => config.Modkey,
					set: (config, value) => config.Modkey = value
				)
				.AddCheckbox(
					label: "Hold-to-activate",
					description: "If enabled, Super Mode will activate by holding the above key.",
					get: config => config.HoldKeyToActivateSuperMode,
					set: (config, value) => config.HoldKeyToActivateSuperMode = value
				)
				.AddNumberField(
					label: "Activation delay",
					description: "How long the key should be held before activatine Super Mode, in seconds.",
					get: config => config.SuperModeActivationDelay,
					set: (config, value) => config.SuperModeActivationDelay = value,
					min: 0,
					max: 5
				)
				.AddNumberField(
					label: "Drain factor",
					description: "Lower numbers make Super Mode last longer.",
					get: config => config.SuperModeDrainFactor,
					set: (config, value) => config.SuperModeDrainFactor = (uint)value,
					min: 1,
					max: 10
				)

				// main
				.AddLabel("Profession Settings")
				.AddNumberField(
					label: "Forages needed for best quality",
					description: "Ecologists must forage this many items to reach iridium quality.",
					get: config => config.ForagesNeededForBestQuality,
					set: (config, value) => config.ForagesNeededForBestQuality = (uint)value,
					min: 0,
					max: 1000
				)
				.AddNumberField(
					label: "Minerals needed for best quality",
					description: "Gemologists must mine this many minerals to reach iridium quality.",
					get: config => config.ForagesNeededForBestQuality,
					set: (config, value) => config.ForagesNeededForBestQuality = (uint)value,
					min: 0,
					max: 1000
				)
				.AddNumberField(
					label: "Chance to start treasure hunt",
					description: "The chance that your Scavenger or Prospector hunt senses will start tingling.",
					get: config => (float)config.ChanceToStartTreasureHunt,
					set: (config, value) => config.ChanceToStartTreasureHunt = (double)value,
					min: 0f,
					max: 1f
				)
				.AddNumberField(
					label: "Treasure hunt handicap",
					description: "Increase this number if you find that treasure hunts end too quickly.",
					get: config => config.TreasureHuntHandicap,
					set: (config, value) => config.TreasureHuntHandicap = value,
					min: 1f,
					max: 10f
				)
				.AddNumberField(
					label: "Treasure detection distance",
					description: "How close you must be to the treasure tile to reveal it's location, in tiles.",
					get: config => config.TreasureDetectionDistance,
					set: (config, value) => config.TreasureDetectionDistance = value,
					min: 1f,
					max: 10f
				)
				.AddNumberField(
					label: "Trash needed per tax level",
					description: "Conservationists must collect this much trash for every 1% tax deduction the following season.",
					get: config => config.TrashNeededPerTaxLevel,
					set: (config, value) => config.TrashNeededPerTaxLevel = (uint)value,
					min: 10,
					max: 1000
				)
				.AddNumberField(
					label: "Trash needed per friendship point",
					description: "Conservationists must collect this much trash for every 1 friendship point towards villagers.",
					get: config => config.TrashNeededPerFriendshipPoint,
					set: (config, value) => config.TrashNeededPerFriendshipPoint = (uint)value,
					min: 10,
					max: 1000
				)
				.AddNumberField(
					label: "Tax deduction ceiling",
					description: "The maximum tax deduction allowed by the Ferngill Revenue Service.",
					get: config => config.TaxDeductionCeiling,
					set: (config, value) => config.TaxDeductionCeiling = value,
					min: 0f,
					max: 1f
				);
		}
	}
}