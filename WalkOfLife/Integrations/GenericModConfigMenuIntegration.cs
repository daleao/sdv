using System;
using StardewModdingAPI;
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

				// Super Mode
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
					0f,
					3f,
					0.2f
				)
				.AddNumberField(
					"Drain factor",
					"Lower numbers make Super Mode last longer.",
					config => (int) config.SuperModeDrainFactor,
					(config, value) => config.SuperModeDrainFactor = (uint) value,
					1,
					10
				)

				// prestige
				.AddLabel("Prestige Settings")
				.AddCheckbox(
					"Enable Prestige",
					"Must be enabled to allow all prestige modifications.",
					config => config.EnablePrestige,
					(config, value) => config.EnablePrestige = value
				)
				.AddNumberField(
					"Skill Reset Cost Multiplier",
					"Multiplies the base cost reseting a skill at the Statue of Prestige. Set to 0 to reset for free.",
					config => config.SkillResetCostMultiplier,
					(config, value) => config.SkillResetCostMultiplier = value,
					0f,
					2f,
					0.2f
				)
				.AddCheckbox(
					"Forget Recipes On Skill Reset",
					"Disable this to keep all skill recipes upon reseting.",
					config => config.ForgetRecipesOnSkillReset,
					(config, value) => config.ForgetRecipesOnSkillReset = value
				)
				.AddCheckbox(
					"Allow Multiple Resets Per Day",
					"Whether the player can reset more than one skill in a day.",
					config => config.AllowMultipleResetsPerDay,
					(config, value) => config.AllowMultipleResetsPerDay = value
				)
				.AddNumberField(
					"Base Skill Experience Multiplier",
					"Multiplies all skill experience gained from the start of the game.",
					config => config.BaseSkillExpMultiplier,
					(config, value) => config.BaseSkillExpMultiplier = value,
					0.2f,
					2f,
					0.2f
				)
				.AddNumberField(
					"Bonus Skill Experience Per Reset",
					"Multiplies all skill experience gained after each respective reset.",
					config => config.BonusSkillExpPerReset,
					(config, value) => config.BonusSkillExpPerReset = value,
					0f,
					2f,
					0.2f
				)
				.AddNumberField(
					"Required Experience Per Extended Level",
					"How much skill experience is required for each level-up beyond level 10.",
					config => (int) config.RequiredExpPerExtendedLevel,
					(config, value) => config.RequiredExpPerExtendedLevel = (uint) value,
					2000,
					20000,
					1000
				)

				// main
				.AddLabel("Profession Settings")
				.AddNumberField(
					"Forages needed for best quality",
					"Ecologists must forage this many items to reach iridium quality.",
					config => (int) config.ForagesNeededForBestQuality,
					(config, value) => config.ForagesNeededForBestQuality = (uint) value,
					0,
					1000
				)
				.AddNumberField(
					"Minerals needed for best quality",
					"Gemologists must mine this many minerals to reach iridium quality.",
					config => (int) config.ForagesNeededForBestQuality,
					(config, value) => config.ForagesNeededForBestQuality = (uint) value,
					0,
					1000
				)
				.AddNumberField(
					"Chance to start treasure hunt",
					"The chance that your Scavenger or Prospector hunt senses will start tingling.",
					config => (float) config.ChanceToStartTreasureHunt,
					(config, value) => config.ChanceToStartTreasureHunt = value,
					0f,
					1f,
					0.05f
				)
				.AddNumberField(
					"Treasure hunt handicap",
					"Increase this number if you find that treasure hunts end too quickly.",
					config => config.TreasureHuntHandicap,
					(config, value) => config.TreasureHuntHandicap = value,
					1f,
					10f,
					0.5f
				)
				.AddNumberField(
					"Treasure detection distance",
					"How close you must be to the treasure tile to reveal it's location, in tiles.",
					config => config.TreasureDetectionDistance,
					(config, value) => config.TreasureDetectionDistance = value,
					1f,
					10f,
					0.5f
				)
				.AddNumberField(
					"Trash needed per tax level",
					"Conservationists must collect this much trash for every 1% tax deduction the following season.",
					config => (int) config.TrashNeededPerTaxLevel,
					(config, value) => config.TrashNeededPerTaxLevel = (uint) value,
					10,
					1000
				)
				.AddNumberField(
					"Trash needed per friendship point",
					"Conservationists must collect this much trash for every 1 friendship point towards villagers.",
					config => (int) config.TrashNeededPerFriendshipPoint,
					(config, value) => config.TrashNeededPerFriendshipPoint = (uint) value,
					10,
					1000
				)
				.AddNumberField(
					"Tax deduction ceiling",
					"The maximum tax deduction allowed by the Ferngill Revenue Service.",
					config => config.TaxDeductionCeiling,
					(config, value) => config.TaxDeductionCeiling = value,
					0f,
					1f,
					0.05f
				);
		}
	}
}