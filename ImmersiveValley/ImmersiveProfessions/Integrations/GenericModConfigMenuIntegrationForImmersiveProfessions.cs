namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Extensions.Collections;
using Common.Integrations.GenericModConfigMenu;
using StardewValley.Buildings;
using System;
using System.Linq;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration for Immersive Professions.</summary>
internal sealed class GenericModConfigMenuIntegrationForImmersiveProfessions
{
    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly GenericModConfigMenuIntegration<ModConfig> _configMenu;

    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="getConfig">Get the current config model.</param>
    /// <param name="reset">Reset the config model to the default values.</param>
    /// <param name="saveAndApply">Save and apply the current config model.</param>
    public GenericModConfigMenuIntegrationForImmersiveProfessions(IModRegistry modRegistry, IManifest manifest,
        Func<ModConfig> getConfig, Action reset, Action saveAndApply)
    {
        _configMenu =
            new(modRegistry, manifest, getConfig, reset, saveAndApply);
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

            // controls and ui settings
            .AddSectionTitle(() => "Controls and UI Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used by Prospector, Scavenger and Rascal professions to enable active effects.",
                config => config.ModKey,
                (config, value) => config.ModKey = value
            )
            .AddDropdown(
                () => "Vintage Interface Style",
                () => "You generally don't need to change this unless you want to override the automatic setting.",
                config => config.VintageInterfaceSupport.ToString(),
                (config, value) =>
                {
                    config.VintageInterfaceSupport = Enum.Parse<ModConfig.VintageInterfaceStyle>(value);
                    ModEntry.ModHelper.GameContent.InvalidateCache($"{ModEntry.Manifest.UniqueID}/SkillBars");
                    ModEntry.ModHelper.GameContent.InvalidateCache($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
                },
                new[] { "Automatic", "Brown", "Pink", "Off" },
                null
            )
            .AddDropdown(
                () => "Progression Style",
                () => "Determines the sprite that appears next to skill bars.",
                config => config.PrestigeProgressionStyle.ToString(),
                (config, value) =>
                {
                    config.PrestigeProgressionStyle = Enum.Parse<ModConfig.ProgressionStyle>(value);
                    ModEntry.ModHelper.GameContent.InvalidateCache($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
                },
                new[] { "StackedStars", "Gen3Ribbons", "Gen4Ribbons" },
                value => value switch
                    {
                        "StackedStars" => "Stacked Stars",
                        "Gen3Ribbons" => "Gen 3 Ribbons",
                        "Gen4Ribbons" => "Gen 4 Ribbons",
                        _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(nameof(value), value, null)
                    }
            )

            // professions
            .AddSectionTitle(() => "Profession Settings")
            .AddNumberField(
                () => "Forages Needed for Best Quality",
                () => "Ecologists must forage this many items to reach iridium quality.",
                config => (int)config.ForagesNeededForBestQuality,
                (config, value) => config.ForagesNeededForBestQuality = (uint)value,
                0,
                1000,
                10
            )
            .AddNumberField(
                () => "Minerals Needed for Best Quality",
                () => "Gemologists must mine this many minerals to reach iridium quality.",
                config => (int)config.MineralsNeededForBestQuality,
                (config, value) => config.MineralsNeededForBestQuality = (uint)value,
                0,
                1000,
                10
            );

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            _configMenu.AddCheckbox(
                () => "Should Count Automated Harvests",
                () =>
                    "If enabled, forages and minerals harvested from automated machines will count towards Ecologist and Gemologist goals.",
                config => config.LaxOwnershipRequirements,
                (config, value) => config.LaxOwnershipRequirements = value
            );

        _configMenu
            .AddNumberField(
                () => "Tracking Pointer Scale",
                () => "Changes the size of the pointer used to track objects by Prospector and Scavenger professions.",
                config => config.TrackPointerScale,
                (config, value) => config.TrackPointerScale = value,
                0.2f,
                2f,
                0.2f
            )
            .AddNumberField(
                () => "Track Pointer Bobbing Rate",
                () => "Changes the speed at which the tracking pointer bounces up and down (higher is faster).",
                config => config.TrackPointerBobbingRate,
                (config, value) => config.TrackPointerBobbingRate = value,
                0.5f,
                2f,
                0.05f
            )
            .AddCheckbox(
                () => "Disable Constant Tracking Arrows",
                () => "If enabled, Prospector and Scavenger will only track off-screen objects while ModKey is held.",
                config => config.DisableAlwaysTrack,
                (config, value) => config.DisableAlwaysTrack = value
            )
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
                config => config.SeaweedIsTrash,
                (config, value) => config.SeaweedIsTrash = value
            )
            .AddNumberField(
                () => "Angler Multiplier Cap",
                () =>
                    "If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.",
                config => config.AnglerMultiplierCap,
                (config, value) => config.AnglerMultiplierCap = value,
                0.5f,
                2f
            )
            .AddNumberField(
                () => "Legendary Pond Population Cap",
                () => "The maximum population of Aquarist Fish Ponds with legendary fish.",
                config => (int) config.LegendaryPondPopulationCap,
                (config, value) =>
                {
                    config.LegendaryPondPopulationCap = (uint) value;
                    if (Context.IsWorldReady)
                        Game1.getFarm().buildings.OfType<FishPond>()
                            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());
                },
                4,
                12
            )
            .AddNumberField(
                () => "Trash Needed Per Tax Bonus Percent",
                () => "Conservationists must collect this much trash for every 1% tax deduction the following season.",
                config => (int) config.TrashNeededPerTaxBonusPct,
                (config, value) => config.TrashNeededPerTaxBonusPct = (uint) value,
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
                () => "Tax Deduction Cap",
                () => "The maximum tax deduction allowed by the Ferngill Revenue Service.",
                config => config.ConservationistTaxBonusCeiling,
                (config, value) => config.ConservationistTaxBonusCeiling = value,
                0f,
                1f,
                0.05f
            )

            // ultimate
            .AddSectionTitle(() => "Special Ability Settings")
            .AddCheckbox(
                () => "Enable Special Abilities",
                () => "Must be enabled to allow activating special abilities.",
                config => config.EnableSpecials,
                (config, value) => config.EnableSpecials = value
            )
            .AddKeyBinding(
                () => "Activation Key",
                () => "The key used to activate the special ability.",
                config => config.SpecialActivationKey,
                (config, value) => config.SpecialActivationKey = value
            )
            .AddCheckbox(
                () => "Hold-To-Activate",
                () => "If enabled, the special ability will be activated only after a short delay.",
                config => config.HoldKeyToActivateSpecial,
                (config, value) => config.HoldKeyToActivateSpecial = value
            )
            .AddNumberField(
                () => "Activation Delay",
                () => "How long the key should be held before the special ability is activated, in seconds.",
                config => config.SpecialActivationDelay,
                (config, value) => config.SpecialActivationDelay = value,
                0f,
                3f,
                0.2f
            )
            .AddNumberField(
                () => "Gain Factor",
                () =>
                    "Affects the rate at which one builds the Ultimate gauge. Increase this if you feel the gauge raises too slowly.",
                config => (float) config.SpecialGainFactor,
                (config, value) => config.SpecialGainFactor = value,
                0.1f,
                2f
            )
            .AddNumberField(
                () => "Drain Factor",
                () =>
                    "Affects the rate at which the Ultimate gauge depletes during Ultimate. Lower numbers make Ultimate last longer.",
                config => (float) config.SpecialDrainFactor,
                (config, value) => config.SpecialDrainFactor = value,
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
                () => "Disable this to keep all skill recipes upon skill reseting.",
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
            );
    }
}