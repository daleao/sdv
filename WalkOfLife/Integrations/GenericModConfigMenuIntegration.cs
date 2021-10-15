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
		public GenericModConfigMenuIntegrationForAwesomeTools(IModRegistry modRegistry, IManifest manifest,
			Func<ModConfig> getConfig, Action reset, Action saveAndApply, Action<string, LogLevel> log)
		{
			_configMenu =
				new(modRegistry, manifest, getConfig, reset, saveAndApply,
					log);
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
					"Mod Key",
					"The key used by Prospector and Scavenger professions.",
					config => config.Modkey,
					(config, value) => config.Modkey = value
				)
				.AddCheckbox(
					"Enable IL code export",
					"If you get a 'failed to patch' error, enable this option and send me the output file along with your bug report.",
					config => config.EnableILCodeExport,
					(config, value) => config.EnableILCodeExport = value
				)

				// super mode
				.AddLabel("Super Mode Settings")
				.AddKeyBinding(
					"Super Mode key",
					"The key used to activate Super Mode.",
					config => config.Modkey,
					(config, value) => config.Modkey = value
				)
				.AddCheckbox(
					"Hold-to-activate",
					"If enabled, Super Mode will activate by holding the above key.",
					config => config.HoldKeyToActivateSuperMode,
					(config, value) => config.HoldKeyToActivateSuperMode = value
				)
				.AddNumberField(
					"Activation delay",
					"How long the key should be held before activating Super Mode, in seconds.",
					config => config.SuperModeActivationDelay,
					(config, value) => config.SuperModeActivationDelay = value,
					0,
					5
				)
				.AddNumberField(
					"Drain factor",
					"Lower numbers make Super Mode last longer.",
					config => config.SuperModeDrainFactor,
					(config, value) => config.SuperModeDrainFactor = (uint)value,
					1,
					10
				)

				// main
				.AddLabel("Profession Settings")
				.AddNumberField(
					"Forages needed for best quality",
					"Ecologists must forage this many items to reach iridium quality.",
					config => config.ForagesNeededForBestQuality,
					(config, value) => config.ForagesNeededForBestQuality = (uint)value,
					0,
					1000
				)
				.AddNumberField(
					"Minerals needed for best quality",
					"Gemologists must mine this many minerals to reach iridium quality.",
					config => config.ForagesNeededForBestQuality,
					(config, value) => config.ForagesNeededForBestQuality = (uint)value,
					0,
					1000
				)
				.AddNumberField(
					"Chance to start treasure hunt",
					"The chance that your Scavenger or Prospector hunt senses will start tingling.",
					config => (float)config.ChanceToStartTreasureHunt,
					(config, value) => config.ChanceToStartTreasureHunt = value,
					0f,
					1f
				)
				.AddNumberField(
					"Treasure hunt handicap",
					"Increase this number if you find that treasure hunts end too quickly.",
					config => config.TreasureHuntHandicap,
					(config, value) => config.TreasureHuntHandicap = value,
					1f,
					10f
				)
				.AddNumberField(
					"Treasure detection distance",
					"How close you must be to the treasure tile to reveal it's location, in tiles.",
					config => config.TreasureDetectionDistance,
					(config, value) => config.TreasureDetectionDistance = value,
					1f,
					10f
				)
				.AddNumberField(
					"Trash needed per tax level",
					"Conservationists must collect this much trash for every 1% tax deduction the following season.",
					config => config.TrashNeededPerTaxLevel,
					(config, value) => config.TrashNeededPerTaxLevel = (uint)value,
					10,
					1000
				)
				.AddNumberField(
					"Trash needed per friendship point",
					"Conservationists must collect this much trash for every 1 friendship point towards villagers.",
					config => config.TrashNeededPerFriendshipPoint,
					(config, value) => config.TrashNeededPerFriendshipPoint = (uint)value,
					10,
					1000
				)
				.AddNumberField(
					"Tax deduction ceiling",
					"The maximum tax deduction allowed by the Ferngill Revenue Service.",
					config => config.TaxDeductionCeiling,
					(config, value) => config.TaxDeductionCeiling = value,
					0f,
					1f
				);
		}
	}
}