namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

using Common.Integrations;
using Framework.Utility;

#endregion using directives

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

            // general mod settings
            .AddSectionTitle(() => "General Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used by Prospector and Scavenger professions.",
                config => config.ModKey,
                (config, value) => config.ModKey = value
            )
            .AddCheckbox(
                () => "Use Vintage UI Elements",
                () => "Enable this option if using the Vintage Interface v2 mod.",
                config => config.UseVintageInterface,
                (config, value) =>
                {
                    config.UseVintageInterface = value;
                    Textures.SuperModeGaugeTx = Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "SuperModeGauge"));
                    Textures.SkillBarTx = Game1.content.Load<Texture2D>(Path.Combine(ModEntry.Manifest.UniqueID, "SkillBars"));
                }
            );

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("cat.betterartisangoodicons"))
            _configMenu.AddDropdown(
                    () => "Honey Mead Style",
                    () => "The visual style for different honey mead icons, if using BetterArtisanGoodIcons.",
                    config => config.HoneyMeadStyle,
                    (config, value) =>
                    {
                        config.HoneyMeadStyle = value;
                        Textures.HoneyMeadTx = Game1.content.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/BetterHoneyMeadIcons");
                    },
                    new[] {"ColoredBottles", "ColoredCaps"},
                    value => value
            );
        
        _configMenu
            .AddCheckbox(
                () => "Enable Fish Pond Rebalance",
                () => "Allow Fish Ponds to produce bonus Roe or Ink in proportion to fish population.",
                config => config.EnableFishPondRebalance,
                (config, value) => config.EnableFishPondRebalance = value
            )

            // super mode
            .AddSectionTitle(() => "Super Mode Settings")
            .AddCheckbox(
                () => "Enable Super Mode",
                () => "Must be enabled to allow activating Super Mode. Super Stat continues to apply.",
                config => config.EnableSuperMode,
                (config, value) => config.EnableSuperMode = value
            )
            .AddKeyBinding(
                () => "Super Mode key",
                () => "The key used to activate Super Mode.",
                config => config.SuperModeKey,
                (config, value) => config.SuperModeKey = value
            )
            .AddCheckbox(
                () => "Hold-To-Activate",
                () => "If enabled, Super Mode will activate by holding the above key.",
                config => config.HoldKeyToActivateSuperMode,
                (config, value) => config.HoldKeyToActivateSuperMode = value
            )
            .AddNumberField(
                () => "Activation Delay",
                () => "How long the key should be held before activating Super Mode, in seconds.",
                config => config.SuperModeActivationDelay,
                (config, value) => config.SuperModeActivationDelay = value,
                0f,
                3f,
                0.2f
            )
            .AddNumberField(
                () => "Gain Factor",
                () => "Affects the rate at which one builds the Super Mode gauge. Increase this if you feel the gauge raises too slowly.",
                config => (float) config.SuperModeGainFactor,
                (config, value) => config.SuperModeGainFactor = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Drain Factor",
                () => "Affects the rate at which the Super Mode gauge depletes during Super Mode. Lower numbers make Super Mode last longer.",
                config => (float) config.SuperModeDrainFactor,
                (config, value) => config.SuperModeDrainFactor = value,
                1f,
                10f,
                0.5f
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
                () => "Forget Recipes on Skill Reset",
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
                () => "Monetary cost of changing the combat Super Mode. Set to 0 to change for free.",
                config => (int) config.ChangeUltCost,
                (config, value) => config.ChangeUltCost = (uint) value,
                0,
                100000,
                10000
            )

            // professions
            .AddSectionTitle(() => "Profession Settings")
            .AddNumberField(
                () => "Forages Needed for Best Quality",
                () => "Ecologists must forage this many items to reach iridium quality.",
                config => (int) config.ForagesNeededForBestQuality,
                (config, value) => config.ForagesNeededForBestQuality = (uint) value,
                0,
                1000
            )
            .AddNumberField(
                () => "Minerals Needed for Best Quality",
                () => "Gemologists must mine this many minerals to reach iridium quality.",
                config => (int) config.ForagesNeededForBestQuality,
                (config, value) => config.ForagesNeededForBestQuality = (uint) value,
                0,
                1000
            );

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            _configMenu.AddCheckbox(
                () => "Should Count Automated Harvests",
                () =>
                    "If enabled, forages and minerals harvested from automated machines will count towards Ecologist and Gemologist goals.",
                config => config.ShouldCountAutomatedHarvests,
                (config, value) => config.ShouldCountAutomatedHarvests = value
            );

        _configMenu
            .AddNumberField(
                () => "Chance to Start Treasure Hunt",
                () => "The chance that your Scavenger or Prospector hunt senses will start tingling.",
                config => (float) config.ChanceToStartTreasureHunt,
                (config, value) => config.ChanceToStartTreasureHunt = value,
                0f,
                1f,
                0.05f
            )
            .AddCheckbox(
                () => "Allow Scavenger Hunts on Farm",
                () => "Whether a Scavenger Hunt can trigger while entering a farm map.",
                config => config.AllowScavengerHuntsOnFarm,
                (config, value) => config.AllowScavengerHuntsOnFarm = value
            )
            .AddNumberField(
                () => "Scavenger Hunt Handicap",
                () => "Increase this number if you find that Scavenger hunts end too quickly.",
                config => config.ScavengerHuntHandicap,
                (config, value) => config.ScavengerHuntHandicap = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Prospector Hunt Handicap",
                () => "Increase this number if you find that Prospector hunts end too quickly.",
                config => config.ProspectorHuntHandicap,
                (config, value) => config.ProspectorHuntHandicap = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Treasure Detection Distance",
                () => "How close you must be to the treasure tile to reveal it's location, in tiles.",
                config => config.TreasureDetectionDistance,
                (config, value) => config.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f
            )
            .AddNumberField(
                () => "Spelunker Speed Cap",
                () => "The maximum speed a Spelunker can reach in the mines.",
                config => (int) config.SpelunkerSpeedCap,
                (config, value) => config.SpelunkerSpeedCap = (uint) value,
                1,
                10
            )
            .AddCheckbox(
                () => "Enable 'Get Excited' buff",
                () => "Toggles the 'Get Excited' buff when a Demolitionist is hit by an explosion.",
                config => config.EnableGetExcited,
                (config, value) => config.EnableGetExcited = value
            )
            .AddCheckbox(
                () => "Seaweed Is Junk",
                () => "Whether Seaweed and Algae are considered junk for fishing purposes.",
                config => config.SeaweedIsJunk,
                (config, value) => config.SeaweedIsJunk = value
            )
            .AddNumberField(
                () => "Angler Multiplier Ceiling",
                () => "If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.",
                config => config.AnglerMultiplierCeiling,
                (config, value) => config.AnglerMultiplierCeiling = value,
                0.5f,
                2f,
                0.1f
            )
            .AddNumberField(
                () => "Trash Needed Per Tax Level",
                () => "Conservationists must collect this much trash for every 1% tax deduction the following season.",
                config => (int) config.TrashNeededPerTaxLevel,
                (config, value) => config.TrashNeededPerTaxLevel = (uint) value,
                10,
                1000
            )
            .AddNumberField(
                () => "Trash Needed Per Friendship Point",
                () => "Conservationists must collect this much trash for every 1 friendship point towards villagers.",
                config => (int) config.TrashNeededPerFriendshipPoint,
                (config, value) => config.TrashNeededPerFriendshipPoint = (uint) value,
                10,
                1000
            )
            .AddNumberField(
                () => "Tax Deduction Ceiling",
                () => "The maximum tax deduction allowed by the Ferngill Revenue Service.",
                config => config.TaxDeductionCeiling,
                (config, value) => config.TaxDeductionCeiling = value,
                0f,
                1f,
                0.05f
            );

        if (!ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP")) return;

        _configMenu
            // SVE
            .AddSectionTitle(() => "SVE Settings")
            .AddCheckbox(
                () => "Use Galdoran Theme All Times",
                () => "Replicates SVE's config settings of the same name.",
                config => config.UseGaldoranThemeAllTimes,
                (config, value) => config.UseGaldoranThemeAllTimes = value
            )
            .AddCheckbox(
                () => "Disable Galdoran Theme",
                () => "Replicates SVE's config settings of the same name.",
                config => config.DisableGaldoranTheme,
                (config, value) => config.DisableGaldoranTheme = value
            );
    }
}