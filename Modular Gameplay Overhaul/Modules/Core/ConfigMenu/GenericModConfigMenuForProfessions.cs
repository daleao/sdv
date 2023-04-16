namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Professions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.UI;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu if available.</summary>
    private void RegisterProfessions()
    {
        this
            .AddPage(OverhaulModule.Professions.Namespace, () => "Profession Settings")

            // controls and ui settings
            .AddSectionTitle(() => "Controls and UI Settings")
            .AddKeyBinding(
                () => "Mod Key",
                () => "The key used by Prospector, Scavenger and Rascal professions to enable active effects.",
                config => config.Professions.ModKey,
                (config, value) => config.Professions.ModKey = value)
            .AddCheckbox(
                () => "Show 'MAX' Icon in Fish Collection",
                () => "Toggles whether or not to display the 'Max' icon below fish caught at max size.",
                config => config.Professions.ShowFishCollectionMaxIcon,
                (config, value) => config.Professions.ShowFishCollectionMaxIcon = value)
            .AddNumberField(
                () => "Tracking Pointer Scale",
                () => "Changes the size of the pointer used to track objects by Prospector and Scavenger professions.",
                config => config.Professions.TrackingPointerScale,
                (config, value) =>
                {
                    config.Professions.TrackingPointerScale = value;
                    if (HudPointer.Instance.IsValueCreated)
                    {
                        HudPointer.Instance.Value.Scale = value;
                    }
                },
                0.2f,
                2f,
                0.2f)
            .AddNumberField(
                () => "Track Pointer Bobbing Rate",
                () => "Changes the speed at which the tracking pointer bounces up and down (higher is faster).",
                config => config.Professions.TrackingPointerBobbingRate,
                (config, value) =>
                {
                    config.Professions.TrackingPointerBobbingRate = value;
                    if (HudPointer.Instance.IsValueCreated)
                    {
                        HudPointer.Instance.Value.BobRate = value;
                    }
                },
                0.5f,
                2f,
                0.05f)
            .AddCheckbox(
                () => "Disable Constant Tracking Arrows",
                () => "If enabled, Prospector and Scavenger will only track off-screen objects while ModKey is held.",
                config => config.Professions.DisableAlwaysTrack,
                (config, value) => config.Professions.DisableAlwaysTrack = value)
            .AddHorizontalRule()

            // professions
            .AddSectionTitle(() => "Profession Settings")
            .AddCheckbox(
                () => "Should Junimos Inherit Professions",
                () => "Whether Junimo harvesters should apply Harvester and Agriculturist perks.",
                config => config.Professions.ShouldJunimosInheritProfessions,
                (config, value) => config.Professions.ShouldJunimosInheritProfessions = value)
            .AddCheckbox(
                () => "Artisan Goods Always Input Quality",
                () => "Enable this if you preferred the old broken Artisan perk without randomization.",
                config => config.Professions.ArtisanGoodsAlwaysInputQuality,
                (config, value) => config.Professions.ArtisanGoodsAlwaysInputQuality = value)
            .AddCheckbox(
                () => "Bees Are Animals",
                () => "Whether Bee House products should be affected by Producer bonuses.",
                config => config.Professions.BeesAreAnimals,
                (config, value) => config.Professions.BeesAreAnimals = value)
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
            this.AddCheckbox(
                () => "Lax Ownership Requirements",
                () =>
                    "If enabled, machine and building ownerhsip will be ignored when determining whether to apply certain profession bonuses.",
                config => config.Professions.LaxOwnershipRequirements,
                (config, value) => config.Professions.LaxOwnershipRequirements = value);
        }

        this
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
                3f,
                0.2f)
            .AddNumberField(
                () => "Prospector Hunt Handicap",
                () => "Increase this number if you find that Prospector hunts end too quickly.",
                config => config.Professions.ProspectorHuntHandicap,
                (config, value) => config.Professions.ProspectorHuntHandicap = value,
                1f,
                3f,
                0.2f)
            .AddNumberField(
                () => "Treasure Detection Distance",
                () => "How close you must be to the treasure tile to reveal it's location, in tiles.",
                config => config.Professions.TreasureDetectionDistance,
                (config, value) => config.Professions.TreasureDetectionDistance = value,
                1f,
                10f,
                0.5f)
            .AddNumberField(
                () => "Spelunker Speed Ceiling",
                () => "The maximum speed a Spelunker can reach in the mines.",
                config => (int)config.Professions.SpelunkerSpeedCeiling,
                (config, value) => config.Professions.SpelunkerSpeedCeiling = (uint)value,
                1,
                10)
            .AddCheckbox(
                () => "Enable 'Get Excited' buff",
                () => "Toggles the 'Get Excited' buff when a Demolitionist is hit by an explosion.",
                config => config.Professions.EnableGetExcited,
                (config, value) => config.Professions.EnableGetExcited = value)
            .AddCheckbox(
                () => "Crystalariums Upgrade With Gemologist",
                () => "Whether or not to increase the quality of active Crystalarium held minerals when the owner Gemologist receives a quality boost.",
                config => config.Professions.CrystalariumsUpgradeWithGemologist,
                (config, value) => config.Professions.CrystalariumsUpgradeWithGemologist = value)
            .AddNumberField(
                () => "Angler Price Bonus Ceiling",
                () =>
                    "If multiple new fish mods are installed, you may want to adjust this to a sensible value. Limits the price multiplier for fish sold by Angler.",
                config => config.Professions.AnglerPriceBonusCeiling,
                (config, value) => config.Professions.AnglerPriceBonusCeiling = value,
                0.5f,
                2f)
            .AddNumberField(
                () => "Aquarist Fish Pond Ceiling",
                () =>
                    "If you like raising dozen of fish species, you may use this limit how easy the fishing minigame will become.",
                config => config.Professions.AquaristFishPondCeiling,
                (config, value) => config.Professions.AquaristFishPondCeiling = value,
                0.5f,
                2f)
            .AddNumberField(
                () => "Legendary Pond Population Ceiling",
                () => "The maximum population of Aquarist Fish Ponds with legendary fish.",
                config => (int)config.Professions.LegendaryPondPopulationCeiling,
                (config, value) =>
                {
                    config.Professions.LegendaryPondPopulationCeiling = (uint)value;
                    if (!Context.IsWorldReady)
                    {
                        return;
                    }

                    var buildings = Game1.getFarm().buildings;
                    for (var i = 0; i < buildings.Count; i++)
                    {
                        var building = buildings[i];
                        if (building is FishPond pond &&
                            (pond.IsOwnedBy(Game1.player) || config.Professions.LaxOwnershipRequirements) &&
                            !pond.isUnderConstruction())
                        {
                            pond.UpdateMaximumOccupancy();
                        }
                    }
                },
                4,
                12)
            .AddNumberField(
                () => "Trash Per Tax Deduction %",
                () => "Conservationists must collect this much trash for every 1% tax deduction the following season.",
                config => (int)config.Professions.TrashNeededPerTaxDeductionPct,
                (config, value) => config.Professions.TrashNeededPerTaxDeductionPct = (uint)value,
                10,
                1000)
            .AddNumberField(
                () => "Trash Per Friendship Point",
                () => "Conservationists must collect this much trash for every 1 friendship point towards villagers.",
                config => (int)config.Professions.TrashNeededPerFriendshipPoint,
                (config, value) => config.Professions.TrashNeededPerFriendshipPoint = (uint)value,
                10,
                1000)
            .AddNumberField(
                () => "Tax Deduction Ceiling",
                () => "The maximum tax deduction allowed by the Ferngill Revenue Service.",
                config => config.Professions.ConservationistTaxBonusCeiling,
                (config, value) => config.Professions.ConservationistTaxBonusCeiling = value,
                0f,
                1f,
                0.05f)
            .AddNumberField(
                () => "Piper Buff Ceiling",
                () => "The maximum stack that can be gained for each buff stat.",
                config => (int)config.Professions.PiperBuffCeiling,
                (config, value) => config.Professions.PiperBuffCeiling = (uint)value,
                10,
                1000)
            .AddHorizontalRule()

            // ultimates
            .AddSectionTitle(() => "Limit Break Settings")
            .AddCheckbox(
                () => "Enable Limit Breaks",
                () => "Must be enabled to allow using Limit Breaks.",
                config => config.Professions.EnableLimitBreaks,
                (config, value) => config.Professions.EnableLimitBreaks = value)
            .AddKeyBinding(
                () => "Activation Key",
                () => "The key used to activate the Limit Break.",
                config => config.Professions.LimitBreakKey,
                (config, value) => config.Professions.LimitBreakKey = value)
            .AddCheckbox(
                () => "Hold-To-Activate",
                () => "If enabled, the Limit Break will be activated only by holding the key for a short interval.",
                config => config.Professions.HoldKeyToLimitBreak,
                (config, value) => config.Professions.HoldKeyToLimitBreak = value)
            .AddNumberField(
                () => "Activation Delay",
                () => "How long the key should be held before the Limit Break is activated, in seconds.",
                config => config.Professions.LimitBreakHoldDelaySeconds,
                (config, value) => config.Professions.LimitBreakHoldDelaySeconds = value,
                0f,
                3f,
                0.2f)
            .AddNumberField(
                () => "Gain Factor",
                () =>
                    "Affects the rate at which one builds the Limit Break gauge. Increase this if you feel the gauge raises too slowly.",
                config => (float)config.Professions.LimitGainFactor,
                (config, value) => config.Professions.LimitGainFactor = value,
                0.1f,
                2f)
            .AddNumberField(
                () => "Drain Factor",
                () =>
                    "Affects the rate at which the Limit Break gauge depletes during Ultimate. Lower numbers make Ultimate last longer.",
                config => (float)config.Professions.LimitDrainFactor,
                (config, value) => config.Professions.LimitDrainFactor = value,
                0.5f,
                2f)
            .AddNumberField(
                () => "Cost of Limit Break Respec",
                () => "Monetary cost of changing the chosen Limit Break. Set to 0 to change for free.",
                config => (int)config.Professions.LimitRespecCost,
                (config, value) => config.Professions.LimitRespecCost = (uint)value,
                0,
                100000,
                10000)
            .AddHorizontalRule()

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
                () => "Bonus Skill Exp After Reset",
                () => "Cumulative bonus that multiplies a skill's experience gain after each respective skill reset.",
                config => config.Professions.PrestigeExpFactor,
                (config, value) => config.Professions.PrestigeExpFactor = value,
                -0.5f,
                2f)
            .AddNumberField(
                () => "Required Exp Per Prestige Level",
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
            .AddDropdown(
                () => "Progression Style",
                () => "Determines the style of the sprite that appears next to skill bars, and indicates the skill reset progression.",
                config => config.Professions.ProgressionStyle.ToString(),
                (config, value) =>
                {
                    config.Professions.ProgressionStyle = Enum.Parse<Config.PrestigeProgressionStyle>(value);
                    ModHelper.GameContent.InvalidateCache(
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
            .AddHorizontalRule()

            // experience settings
            .AddSectionTitle(() => "Experience Settings")
            .AddNumberField(
                () => "Base Farming Exp Multiplier",
                () => "Multiplies all skill experience gained for Farming from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[0],
                (config, value) => config.Professions.BaseSkillExpMultipliers[0] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Fishing Exp Multiplier",
                () => "Multiplies all skill experience gained for Fishing from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[1],
                (config, value) => config.Professions.BaseSkillExpMultipliers[1] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Foraging Exp Multiplier",
                () => "Multiplies all skill experience gained for Foraging from the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[2],
                (config, value) => config.Professions.BaseSkillExpMultipliers[2] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Mining Exp Multiplier",
                () => "Multiplies all skill experience gained for Mining the start of the game.",
                config => config.Professions.BaseSkillExpMultipliers[3],
                (config, value) => config.Professions.BaseSkillExpMultipliers[3] = value,
                0.2f,
                2f)
            .AddNumberField(
                () => "Base Combat Exp Multiplier",
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
            this
                .AddNumberField(
                    () => $"Base {skill.DisplayName} Exp Multiplier",
                    () => $"Multiplies all skill experience gained for {skill.StringId} from the start of the game.",
                    config => config.Professions.CustomSkillExpMultipliers[skillId],
                    (config, value) => config.Professions.CustomSkillExpMultipliers[skillId] = value,
                    0.2f,
                    2f);
        }
    }
}
