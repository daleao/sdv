using StardewModdingAPI;
using System;
using TheLion.Stardew.Common.Integrations;

namespace TheLion.Stardew.Professions.Integrations;

/// <summary>Constructs the GenericModConfigMenu integration for Awesome Tools.</summary>
internal class GenericModConfigMenuIntegrationForAwesomeProfessions
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
    public GenericModConfigMenuIntegrationForAwesomeProfessions(IModRegistry modRegistry, IManifest manifest,
        Action<string, LogLevel> log, Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu =
            new(modRegistry, manifest, log, getConfig, reset, saveAndApply);
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

            // main mod settings
            .AddSectionTitle(() => "Mod Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used by Prospector and Scavenger professions.",
                config => config.Modkey,
                (config, value) => config.Modkey = value
            )
            .AddCheckbox(
                () => "Use Vintage Skill Bars",
                () => "Enable this option if using the Vintage Interface mod.",
                config => config.UseVintageSkillBars,
                (config, value) => config.UseVintageSkillBars = value
            )

            // Super Mode
            .AddSectionTitle(() => "Super Mode Settings")
            .AddKeyBinding(
                () => "Super Mode key",
                () => "The key used to activate Super Mode.",
                config => config.Modkey,
                (config, value) => config.Modkey = value
            )
            .AddCheckbox(
                () => "Hold-to-activate",
                () => "If enabled, Super Mode will activate by holding the above key.",
                config => config.HoldKeyToActivateSuperMode,
                (config, value) => config.HoldKeyToActivateSuperMode = value
            )
            .AddNumberField(
                () => "Activation delay",
                () => "How long the key should be held before activating Super Mode, in seconds.",
                config => config.SuperModeActivationDelay,
                (config, value) => config.SuperModeActivationDelay = value,
                0f,
                3f,
                0.2f
            )
            .AddNumberField(
                () => "Drain factor",
                () => "Lower numbers make Super Mode last longer.",
                config => (int) config.SuperModeDrainFactor,
                (config, value) => config.SuperModeDrainFactor = (uint) value,
                1,
                10
            )

            // prestige
            .AddSectionTitle(() => "Prestige Settings")
            .AddCheckbox(
                () => "Enable Prestige",
                () => "Must be enabled to allow all prestige modifications.",
                config => config.EnablePrestige,
                (config, value) => config.EnablePrestige = value
            )
            .AddNumberField(
                () => "Skill Reset Cost Multiplier",
                () =>
                    "Multiplies the base cost reseting a skill at the Statue of Prestige. Set to 0 to reset for free.",
                config => config.SkillResetCostMultiplier,
                (config, value) => config.SkillResetCostMultiplier = value,
                0f,
                2f,
                0.2f
            )
            .AddCheckbox(
                () => "Forget Recipes On Skill Reset",
                () => "Disable this to keep all skill recipes upon reseting.",
                config => config.ForgetRecipesOnSkillReset,
                (config, value) => config.ForgetRecipesOnSkillReset = value
            )
            .AddCheckbox(
                () => "Allow Multiple Prestiges Per Day",
                () => "Whether the player can use the Statue of Prestige more than once in a day.",
                config => config.AllowPrestigeMultiplePerDay,
                (config, value) => config.AllowPrestigeMultiplePerDay = value
            )
            .AddNumberField(
                () => "Base Skill Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplier,
                (config, value) => config.BaseSkillExpMultiplier = value,
                0.2f,
                2f,
                0.2f
            )
            .AddNumberField(
                () => "Bonus Skill Experience Per Reset",
                () => "Multiplies all skill experience gained after each respective reset.",
                config => config.BonusSkillExpPerReset,
                (config, value) => config.BonusSkillExpPerReset = value,
                0f,
                2f,
                0.2f
            )
            .AddNumberField(
                () => "Required Experience Per Extended Level",
                () => "How much skill experience is required for each level-up beyond level 10.",
                config => (int) config.RequiredExpPerExtendedLevel,
                (config, value) => config.RequiredExpPerExtendedLevel = (uint) value,
                5000,
                25000,
                1000
            )
            .AddNumberField(
                () => "Cost of Prestige Respec",
                () =>
                    "Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.",
                config => (int) config.PrestigeRespecCost,
                (config, value) => config.PrestigeRespecCost = (uint) value,
                0,
                100000,
                10000
            )
            .AddNumberField(
                () => "Cost of Changing Ultimate",
                () => "Monetary cost of changing the combat ultimate. Set to 0 to change for free.",
                config => (int) config.ChangeUltCost,
                (config, value) => config.ChangeUltCost = (uint) value,
                0,
                100000,
                10000
            )

            // main
            .AddSectionTitle(() => "Profession Settings")
            .AddNumberField(
                () => "Forages needed for best quality",
                () => "Ecologists must forage this many items to reach iridium quality.",
                config => (int) config.ForagesNeededForBestQuality,
                (config, value) => config.ForagesNeededForBestQuality = (uint) value,
                0,
                1000
            )
            .AddNumberField(
                () => "Minerals needed for best quality",
                () => "Gemologists must mine this many minerals to reach iridium quality.",
                config => (int) config.ForagesNeededForBestQuality,
                (config, value) => config.ForagesNeededForBestQuality = (uint) value,
                0,
                1000
            );

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate"))
        {
            _configMenu.AddCheckbox(
                () => "Should Count Automated Harvests",
                () =>
                    "If enabled, forages and minerals harvested from automated machines will count towards Ecologist and Gemologist goals.",
                config => config.ShouldCountAutomatedHarvests,
                (config, value) => config.ShouldCountAutomatedHarvests = value
            );
        }

        _configMenu
            .AddNumberField(
                () => "Chance to start treasure hunt",
                () => "The chance that your Scavenger or Prospector hunt senses will start tingling.",
                config => (float)config.ChanceToStartTreasureHunt,
                (config, value) => config.ChanceToStartTreasureHunt = value,
                0f,
                1f,
                0.05f
            )
            .AddCheckbox(
                () => "Allow Scavenger hunts on farm",
                () => "Whether a Scavenger Hunt can trigger while entering a farm map.",
                config => config.AllowScavengerHuntsOnFarm,
                (config, value) => config.AllowScavengerHuntsOnFarm = value
            )
            .AddNumberField(
                () => "Scavenger Hunt handicap",
                () => "Increase this number if you find that Scavenger hunts end too quickly.",
                config => config.ScavengerHuntHandicap,
                (config, value) => config.ScavengerHuntHandicap = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Prospector Hunt handicap",
                () => "Increase this number if you find that Prospector hunts end too quickly.",
                config => config.ProspectorHuntHandicap,
                (config, value) => config.ProspectorHuntHandicap = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Treasure detection distance",
                () => "How close you must be to the treasure tile to reveal it's location, in tiles.",
                config => config.TreasureDetectionDistance,
                (config, value) => config.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Spelunker speed cap",
                () => "The maximum speed a Spelunker can reach in the mines.",
                config => config.SpelunkerSpeedCap,
                (config, value) => config.SpelunkerSpeedCap = value,
                1,
                10
            )
            .AddCheckbox(
                () => "Enable 'Get Excited' buff",
                () => "Toggles the 'Get Excited' buff when a Demolitionist is hit by an explosion.",
                config => config.EnableGetExcited,
                (config, value) => config.EnableGetExcited = value
            )
            .AddNumberField(
                () => "Trash needed per tax level",
                () => "Conservationists must collect this much trash for every 1% tax deduction the following season.",
                config => (int)config.TrashNeededPerTaxLevel,
                (config, value) => config.TrashNeededPerTaxLevel = (uint)value,
                10,
                1000
            )
            .AddNumberField(
                () => "Trash needed per friendship point",
                () => "Conservationists must collect this much trash for every 1 friendship point towards villagers.",
                config => (int)config.TrashNeededPerFriendshipPoint,
                (config, value) => config.TrashNeededPerFriendshipPoint = (uint)value,
                10,
                1000
            )
            .AddNumberField(
                () => "Tax deduction ceiling",
                () => "The maximum tax deduction allowed by the Ferngill Revenue Service.",
                config => config.TaxDeductionCeiling,
                (config, value) => config.TaxDeductionCeiling = value,
                0f,
                1f,
                0.05f
            );
    }
}