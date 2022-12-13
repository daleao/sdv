namespace DaLion.Overhaul.Modules.Core.Configs;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuIntegration
{
    /// <summary>Register the config menu if available.</summary>
    private void RegisterProfessions()
    {
        this._configMenu
            .AddPage(OverhaulModule.Professions.Namespace, () => "Profession Settings")

            // controls and ui settings
            .AddSectionTitle(() => "Controls and UI Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used by Prospector, Scavenger and Rascal professions to enable active effects.",
                config => config.Professions.ModKey,
                (config, value) => config.Professions.ModKey = value)
            .AddDropdown(
                () => "Progression Style",
                () => "Determines the sprite that appears next to skill bars.",
                config => config.Professions.PrestigeProgressionStyle.ToString(),
                (config, value) =>
                {
                    config.Professions.PrestigeProgressionStyle = Enum.Parse<Professions.ProfessionsConfig.ProgressionStyle>(value);
                    ModHelper.GameContent.InvalidateCacheAndLocalized(
                        $"{Manifest.UniqueID}/PrestigeProgression");
                },
                new[] { "StackedStars", "Gen3Ribbons", "Gen4Ribbons" },
                value => value switch
                {
                    "StackedStars" => "Stacked Stars",
                    "Gen3Ribbons" => "Gen 3 Ribbons",
                    "Gen4Ribbons" => "Gen 4 Ribbons",
                    _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(nameof(value), value, null),
                })

            // professions
            .AddSectionTitle(() => "Profession Settings")
            .AddNumberField(
                () => "Forages Needed for Best Quality",
                () => "Ecologists must forage this many items to reach iridium quality.",
                config => (int)config.Professions.ForagesNeededForBestQuality,
                (config, value) => config.Professions.ForagesNeededForBestQuality = (uint)value,
                0,
                1000,
                10)
            .AddNumberField(
                () => "Minerals Needed for Best Quality",
                () => "Gemologists must mine this many minerals to reach iridium quality.",
                config => (int)config.Professions.MineralsNeededForBestQuality,
                (config, value) => config.Professions.MineralsNeededForBestQuality = (uint)value,
                0,
                1000,
                10);

        if (ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate"))
        {
            this._configMenu.AddCheckbox(
                () => "Lax Ownership Requirements",
                () =>
                    "If enabled, machine and building ownerhsip will be ignored when determining whether to apply certain profession bonuses.",
                config => config.Professions.LaxOwnershipRequirements,
                (config, value) => config.Professions.LaxOwnershipRequirements = value);
        }

        this._configMenu
            .AddNumberField(
                () => "Tracking Pointer Scale",
                () => "Changes the size of the pointer used to track objects by Prospector and Scavenger professions.",
                config => config.Professions.TrackPointerScale,
                (config, value) =>
                {
                    config.Professions.TrackPointerScale = value;
                    Globals.Pointer.Value.Scale = value;
                },
                0.2f,
                2f,
                0.2f)
            .AddNumberField(
                () => "Track Pointer Bobbing Rate",
                () => "Changes the speed at which the tracking pointer bounces up and down (higher is faster).",
                config => config.Professions.TrackPointerBobbingRate,
                (config, value) =>
                {
                    config.Professions.TrackPointerBobbingRate = value;
                    Globals.Pointer.Value.BobRate = value;
                },
                0.5f,
                2f,
                0.05f)
            .AddCheckbox(
                () => "Disable Constant Tracking Arrows",
                () => "If enabled, Prospector and Scavenger will only track off-screen objects while ModKey is held.",
                config => config.Professions.DisableAlwaysTrack,
                (config, value) => config.Professions.DisableAlwaysTrack = value)
            .AddNumberField(
                () => "Chance to Start Treasure Hunt",
                () => "The chance that your Scavenger or Prospector hunt senses will start tingling.",
                config => (float)config.Professions.ChanceToStartTreasureHunt,
                (config, value) => config.Professions.ChanceToStartTreasureHunt = value,
                0f,
                1f,
                0.05f)
            .AddCheckbox(
                () => "Allow Scavenger Hunts on Farm",
                () => "Whether a Scavenger Hunt can trigger while entering a farm map.",
                config => config.Professions.AllowScavengerHuntsOnFarm,
                (config, value) => config.Professions.AllowScavengerHuntsOnFarm = value)
            .AddNumberField(
                () => "Scavenger Hunt Handicap",
                () => "Increase this number if you find that Scavenger hunts end too quickly.",
                config => config.Professions.ScavengerHuntHandicap,
                (config, value) => config.Professions.ScavengerHuntHandicap = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                () => "Prospector Hunt Handicap",
                () => "Increase this number if you find that Prospector hunts end too quickly.",
                config => config.Professions.ProspectorHuntHandicap,
                (config, value) => config.Professions.ProspectorHuntHandicap = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                () => "Treasure Detection Distance",
                () => "How close you must be to the treasure tile to reveal it's location, in tiles.",
                config => config.Professions.TreasureDetectionDistance,
                (config, value) => config.Professions.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                () => "Spelunker Speed Cap",
                () => "The maximum speed a Spelunker can reach in the mines.",
                config => (int)config.Professions.SpelunkerSpeedCap,
                (config, value) => config.Professions.SpelunkerSpeedCap = (uint)value,
                1,
                10)
            .AddCheckbox(
                () => "Enable 'Get Excited' buff",
                () => "Toggles the 'Get Excited' buff when a Demolitionist is hit by an explosion.",
                config => config.Professions.EnableGetExcited,
                (config, value) => config.Professions.EnableGetExcited = value)
            .AddNumberField(
                () => "Angler Multiplier Cap",
                () =>
                    "If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.",
                config => config.Professions.AnglerMultiplierCap,
                (config, value) => config.Professions.AnglerMultiplierCap = value,
                0.5f,
                2f)
            .AddNumberField(
                () => "Legendary Pond Population Cap",
                () => "The maximum population of Aquarist Fish Ponds with legendary fish.",
                config => (int)config.Professions.LegendaryPondPopulationCap,
                (config, value) =>
                {
                    config.Professions.LegendaryPondPopulationCap = (uint)value;
                    if (Context.IsWorldReady)
                    {
                        Game1.getFarm().buildings.OfType<FishPond>()
                            .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer ||
                                         ProfessionsModule.Config.LaxOwnershipRequirements) &&
                                        !p.isUnderConstruction()).ForEach(p => p.UpdateMaximumOccupancy());
                    }
                },
                4,
                12)
            .AddNumberField(
                () => "Trash Needed Per Tax Bonus Percent",
                () => "Conservationists must collect this much trash for every 1% tax deduction the following season.",
                config => (int)config.Professions.TrashNeededPerTaxBonusPct,
                (config, value) => config.Professions.TrashNeededPerTaxBonusPct = (uint)value,
                10,
                1000)
            .AddNumberField(
                () => "Trash Needed Per Friendship Point",
                () => "Conservationists must collect this much trash for every 1 friendship point towards villagers.",
                config => (int)config.Professions.TrashNeededPerFriendshipPoint,
                (config, value) => config.Professions.TrashNeededPerFriendshipPoint = (uint)value,
                10,
                1000)
            .AddNumberField(
                () => "Tax Deduction Cap",
                () => "The maximum tax deduction allowed by the Ferngill Revenue Service.",
                config => config.Professions.ConservationistTaxBonusCeiling,
                (config, value) => config.Professions.ConservationistTaxBonusCeiling = value,
                0f,
                1f,
                0.05f)

            // ultimate
            .AddSectionTitle(() => "Special Ability Settings")
            .AddCheckbox(
                () => "Enable Special Abilities",
                () => "Must be enabled to allow activating special abilities.",
                config => config.Professions.EnableSpecials,
                (config, value) => config.Professions.EnableSpecials = value)
            .AddKeyBinding(
                () => "Activation Key",
                () => "The key used to activate the special ability.",
                config => config.Professions.SpecialActivationKey,
                (config, value) => config.Professions.SpecialActivationKey = value)
            .AddCheckbox(
                () => "Hold-To-Activate",
                () => "If enabled, the special ability will be activated only after a short delay.",
                config => config.Professions.HoldKeyToActivateSpecial,
                (config, value) => config.Professions.HoldKeyToActivateSpecial = value)
            .AddNumberField(
                () => "Activation Delay",
                () => "How long the key should be held before the special ability is activated, in seconds.",
                config => config.Professions.SpecialActivationDelay,
                (config, value) => config.Professions.SpecialActivationDelay = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                () => "Gain Factor",
                () =>
                    "Affects the rate at which one builds the Ultimate gauge. Increase this if you feel the gauge raises too slowly.",
                config => (float)config.Professions.SpecialGainFactor,
                (config, value) => config.Professions.SpecialGainFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                () => "Drain Factor",
                () =>
                    "Affects the rate at which the Ultimate gauge depletes during Ultimate. Lower numbers make Ultimate last longer.",
                config => (float)config.Professions.SpecialDrainFactor,
                (config, value) => config.Professions.SpecialDrainFactor = value,
                0.5f,
                2f)

            // prestige
            .AddSectionTitle(() => "Prestige Settings")
            .AddCheckbox(
                () => "Enable Prestige",
                () => "Must be enabled to allow all prestige modifications.",
                config => config.Professions.EnablePrestige,
                (config, value) => config.Professions.EnablePrestige = value)
            .AddNumberField(
                () => "Skill Reset Cost Multiplier",
                () =>
                    "Multiplies the base cost reseting a skill at the Statue of Prestige. Set to 0 to reset for free.",
                config => config.Professions.SkillResetCostMultiplier,
                (config, value) => config.Professions.SkillResetCostMultiplier = value,
                0f,
                2f)
            .AddCheckbox(
                () => "Forget Recipes on Skill Reset",
                () => "Disable this to keep all skill recipes upon skill reseting.",
                config => config.Professions.ForgetRecipes,
                (config, value) => config.Professions.ForgetRecipes = value)
            .AddCheckbox(
                () => "Allow Multiple Prestiges Per Day",
                () => "Whether the player can use the Statue of Prestige more than once in a day.",
                config => config.Professions.AllowMultiplePrestige,
                (config, value) => config.Professions.AllowMultiplePrestige = value)
            .AddNumberField(
                () => "Bonus Skill Experience After Reset",
                () => "Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.",
                config => config.Professions.PrestigeExpMultiplier,
                (config, value) => config.Professions.PrestigeExpMultiplier = value,
                0f,
                2f)
            .AddNumberField(
                () => "Required Experience Per Extended Level",
                () => "How much skill experience is required for each level-up beyond level 10.",
                config => (int)config.Professions.RequiredExpPerExtendedLevel,
                (config, value) => config.Professions.RequiredExpPerExtendedLevel = (uint)value,
                1000,
                10000,
                500)
            .AddNumberField(
                () => "Cost of Prestige Respec",
                () =>
                    "Monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.",
                config => (int)config.Professions.PrestigeRespecCost,
                (config, value) => config.Professions.PrestigeRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddNumberField(
                () => "Cost of Changing Ultimate",
                () => "Monetary cost of changing the combat Ultimate. Set to 0 to change for free.",
                config => (int)config.Professions.ChangeUltCost,
                (config, value) => config.Professions.ChangeUltCost = (uint)value,
                0,
                100000,
                10000)

            // difficulty settings
            .AddSectionTitle(() => "Difficulty Settings")
            .AddNumberField(
                () => "Base Farming Experience Multiplier",
                () => "Multiplies all skill experience gained for Farming from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[0],
                (config, value) => config.Professions.BaseSkillExpMultipliers[0] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Fishing Experience Multiplier",
                () => "Multiplies all skill experience gained for Fishing from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[1],
                (config, value) => config.Professions.BaseSkillExpMultipliers[1] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Foraging Experience Multiplier",
                () => "Multiplies all skill experience gained for Foraging from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[2],
                (config, value) => config.Professions.BaseSkillExpMultipliers[2] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Mining Experience Multiplier",
                () => "Multiplies all skill experience gained for Mining the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[3],
                (config, value) => config.Professions.BaseSkillExpMultipliers[3] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Combat Experience Multiplier",
                () => "Multiplies all skill experience gained for Combat from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[4],
                (config, value) => config.Professions.BaseSkillExpMultipliers[4] = value,
                0.2f,
                2f);

        foreach (var (skillId, _) in ProfessionsModule.Config.CustomSkillExpMultipliers)
        {
            if (!SCSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            var skill = SCSkill.Loaded[skillId];
            this._configMenu
                .AddNumberField(
                    () => $"Base {skill.DisplayName} Experience Multiplier",
                    () => $"Multiplies all skill experience gained for {skill.StringId} from the start of the game.",
                    config => config.Professions.CustomSkillExpMultipliers[skillId],
                    (config, value) => config.Professions.CustomSkillExpMultipliers[skillId] = value,
                    0.2f,
                    2f);
        }
    }
}
