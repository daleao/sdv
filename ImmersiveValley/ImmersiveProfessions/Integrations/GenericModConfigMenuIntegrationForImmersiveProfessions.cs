namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

using Common.Integrations;
using Framework.Utility;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Professions.</summary>
internal class GenericModConfigMenuIntegrationForImmersiveProfessions
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
    public GenericModConfigMenuIntegrationForImmersiveProfessions(IModRegistry modRegistry, IManifest manifest,
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
                () => "The key used by Prospector, Scavenger and Rascal professions.",
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
                    Textures.UltimateMeterTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
                    Textures.SkillBarTx = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
                }
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
                config => (int) config.MineralsNeededForBestQuality,
                (config, value) => config.MineralsNeededForBestQuality = (uint) value,
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
                () =>
                    "If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.",
                config => config.AnglerMultiplierCeiling,
                (config, value) => config.AnglerMultiplierCeiling = value,
                0.5f,
                2f
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
            )


            // ultimate
            .AddSectionTitle(() => "Ultimate Settings")
            .AddCheckbox(
                () => "Enable Ultimate",
                () => "Must be enabled to allow activating Ultimate. Super Stat continues to apply.",
                config => config.EnableUltimates,
                (config, value) => config.EnableUltimates = value
            )
            .AddKeyBinding(
                () => "Ultimate key",
                () => "The key used to activate Ultimate.",
                config => config.UltimateKey,
                (config, value) => config.UltimateKey = value
            )
            .AddCheckbox(
                () => "Hold-To-Activate",
                () => "If enabled, Ultimate will activate by holding the above key.",
                config => config.HoldKeyToActivateUltimate,
                (config, value) => config.HoldKeyToActivateUltimate = value
            )
            .AddNumberField(
                () => "Activation Delay",
                () => "How long the key should be held before activating Ultimate, in seconds.",
                config => config.UltimateActivationDelay,
                (config, value) => config.UltimateActivationDelay = value,
                0f,
                3f,
                0.2f
            )
            .AddNumberField(
                () => "Gain Factor",
                () =>
                    "Affects the rate at which one builds the Ultimate gauge. Increase this if you feel the gauge raises too slowly.",
                config => (float) config.UltimateGainFactor,
                (config, value) => config.UltimateGainFactor = value,
                0.1f,
                2f
            )
            .AddNumberField(
                () => "Drain Factor",
                () =>
                    "Affects the rate at which the Ultimate gauge depletes during Ultimate. Lower numbers make Ultimate last longer.",
                config => (float) config.UltimateDrainFactor,
                (config, value) => config.UltimateDrainFactor = value,
                0.1f,
                2f
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
                2f
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
                () => "Bonus Skill Experience After Reset",
                () => "Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.",
                config => config.BonusSkillExpPerReset,
                (config, value) => config.BonusSkillExpPerReset = value,
                0f,
                2f
            )
            .AddNumberField(
                () => "Required Experience Per Extended Level",
                () => "How much skill experience is required for each level-up beyond level 10.",
                config => (int) config.RequiredExpPerExtendedLevel,
                (config, value) => config.RequiredExpPerExtendedLevel = (uint) value,
                1000,
                10000,
                500
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
                () => "Monetary cost of changing the combat Ultimate. Set to 0 to change for free.",
                config => (int) config.ChangeUltCost,
                (config, value) => config.ChangeUltCost = (uint) value,
                0,
                100000,
                10000
            )

            // difficulty settings
            .AddSectionTitle(() => "Difficulty Settings")
            .AddNumberField(
                () => "Base Farming Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplierPerSkill[0],
                (config, value) => config.BaseSkillExpMultiplierPerSkill[0] = value,
                0.2f,
                2f
            )
            .AddNumberField(
                () => "Base Fishing Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplierPerSkill[1],
                (config, value) => config.BaseSkillExpMultiplierPerSkill[1] = value,
                0.2f,
                2f
            )
            .AddNumberField(
                () => "Base Foraging Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplierPerSkill[2],
                (config, value) => config.BaseSkillExpMultiplierPerSkill[2] = value,
                0.2f,
                2f
            )
            .AddNumberField(
                () => "Base Mining Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplierPerSkill[3],
                (config, value) => config.BaseSkillExpMultiplierPerSkill[3] = value,
                0.2f,
                2f
            )
            .AddNumberField(
                () => "Base Combat Experience Multiplier",
                () => "Multiplies all skill experience gained from the start of the game.",
                config => config.BaseSkillExpMultiplierPerSkill[4],
                (config, value) => config.BaseSkillExpMultiplierPerSkill[4] = value,
                0.2f,
                2f
            )
            .AddNumberField(
                () => "Monster Health Multiplier",
                () => "Increases the health of all enemies.",
                config => config.MonsterHealthMultiplier,
                (config, value) => config.MonsterHealthMultiplier = value,
                1f,
                3f
            )
            .AddNumberField(
                () => "Monster Damage Multiplier",
                () => "Increases the damage dealt by all enemies.",
                config => config.MonsterDamageMultiplier,
                (config, value) => config.MonsterDamageMultiplier = value,
                1f,
                3f
            )
            .AddNumberField(
                () => "Monster Defense Multiplier",
                () => "Increases the damage resistance of all enemies.",
                config => config.MonsterDefenseMultiplier,
                (config, value) => config.MonsterDefenseMultiplier = value,
                1f,
                3f
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