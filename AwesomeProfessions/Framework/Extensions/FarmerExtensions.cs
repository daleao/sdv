// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using Common.Extensions;
using Events.GameLoop;
using SuperMode;
using Utility;

using Professions = Utility.Localization;
using SObject = StardewValley.Object;

#endregion using directives

public static class FarmerExtensions
{
    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="professionName">The name of the profession.</param>
    public static bool HasProfession(this Farmer farmer, string professionName)
    {
        return Enum.TryParse<Profession>(professionName, out var profession) &&
               farmer.professions.Contains((int) profession);
    }

    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="professionIndex">The index of the profession.</param>
    public static bool HasProfession(this Farmer farmer, int professionIndex)
    {
        return Enum.TryParse<Profession>(professionIndex.ToString(), out _) &&
               farmer.professions.Contains(professionIndex);
    }

    /// <summary>Whether the farmer has prestiged a particular profession.</summary>
    /// <param name="professionName">The name of the profession.</param>
    public static bool HasPrestigedProfession(this Farmer farmer, string professionName)
    {
        return Enum.TryParse<Profession>(professionName, out var profession) &&
               farmer.professions.Contains((int) profession + 100);
    }

    /// <summary>Whether the farmer has any of the specified professions.</summary>
    /// <param name="professionNames">Sequence of profession names.</param>
    public static bool HasAnyOfProfessions(this Farmer farmer, params string[] professionNames)
    {
        return professionNames.Any(name =>
            Enum.TryParse<Profession>(name, out var profession) &&
            farmer.professions.Contains((int) profession));
    }

    /// <summary>Whether the farmer has any of the specified professions.</summary>
    /// <param name="professionNames">Sequence of profession names.</param>
    /// <param name="firstMatch">The first profession acquired by the player among the specified profession names.</param>
    public static bool HasAnyOfProfessions(this Farmer farmer, string[] professionNames, out string firstMatch)
    {
        firstMatch = professionNames.FirstOrDefault(name =>
            Enum.TryParse<Profession>(name, out var profession) &&
            farmer.professions.Contains((int) profession));
        return firstMatch is not null;
    }

    /// <summary>Whether the farmer has acquired any other professions branching from the specified profession.</summary>
    /// <param name="professionIndex">A profession index (0 to 29).</param>
    /// <param name="otherProfessions">
    ///     An array of acquired professions in the same branch as
    ///     <paramref name="professionIndex">, excluding itself.
    /// </param>
    public static bool HasOtherProfessionsInBranch(this Farmer farmer, int professionIndex,
        out int[] otherProfessions)
    {
        var otherProfessionsInBranch = (professionIndex % 6) switch
        {
            0 => new[] {professionIndex + 1},
            1 => new[] {professionIndex - 1},
            2 => new[] {professionIndex + 1, professionIndex + 2, professionIndex + 3},
            3 => new[] {professionIndex - 1, professionIndex + 1, professionIndex + 2},
            4 => new[] {professionIndex - 2, professionIndex - 1, professionIndex + 1},
            5 => new[] {professionIndex - 3, professionIndex - 2, professionIndex - 1},
            _ => Array.Empty<int>()
        };

        otherProfessions = farmer.professions.Intersect(otherProfessionsInBranch).ToArray();
        return otherProfessions.Any();
    }

    /// <summary>Whether the farmer has acquired all professions branching from the specified profession.</summary>
    /// <param name="professionIndex">A profession index (0 to 29).</param>
    public static bool HasAllProfessionsInBranch(this Farmer farmer, int professionIndex)
    {
        return professionIndex % 6 == 0 && farmer.professions.Contains(professionIndex + 2) &&
               farmer.professions.Contains(professionIndex + 3) ||
               professionIndex % 6 == 1 && farmer.professions.Contains(professionIndex + 3) &&
               farmer.professions.Contains(professionIndex + 4) ||
               professionIndex % 6 > 1;
    }

    /// <summary>Whether the farmer has all six professions in the specified skill.</summary>
    public static bool HasAllProfessionsInSkill(this Farmer farmer, int which)
    {
        return farmer.NumberOfProfessionsInSkill(which) == 6;
    }

    /// <summary>Whether the farmer has all 30 vanilla professions.</summary>
    public static bool HasAllProfessions(this Farmer farmer)
    {
        var allProfessions = Enumerable.Range(0, 30).ToList();
        return farmer.professions.Intersect(allProfessions).Count() == allProfessions.Count();
    }

    /// <summary>Get the last 1st-tier profession acquired by the farmer in the specified skill.</summary>
    /// <param name="skill">The skill index.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetCurrentBranchForSkill(this Farmer farmer, int skill)
    {
        var lastIndex = farmer.professions.ToList().FindLastIndex(p => p == skill * 6 || p == skill * 6 + 1);
        return lastIndex >= 0
            ? farmer.professions[lastIndex]
            : lastIndex;
    }

    /// <summary>Get the last level 2nd-tier profession acquired by the farmer in the specified skill branch.</summary>
    /// <param name="branch">The branch (level 5 profession) index.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetCurrentProfessionForBranch(this Farmer farmer, int branch)
    {
        var lastIndex = farmer.professions.ToList().FindLastIndex(p => branch % 6 == 0
            ? p == branch + 2 || p == branch + 3
            : p == branch + 3 || p == branch + 4);
        return lastIndex >= 0
            ? farmer.professions[lastIndex]
            : lastIndex;
    }

    /// <summary>Get all the farmer's professions associated with a specific skill.</summary>
    /// <param name="which">The skill index.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    public static IEnumerable<int> GetAllProfessionsForSkill(this Farmer farmer, int which,
        bool excludeTierOneProfessions = false)
    {
        return farmer.professions.Intersect(excludeTierOneProfessions
            ? Enumerable.Range(which * 6 + 2, 4)
            : Enumerable.Range(which * 6, 6));
    }

    /// <summary>Count the number of professions acquired by the player in the specified skill.</summary>
    /// <param name="which">The skill index.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    public static int NumberOfProfessionsInSkill(this Farmer farmer, int which,
        bool excludeTierOneProfessions = false)
    {
        return excludeTierOneProfessions
            ? farmer.professions.Count(p => p / 6 == which && p % 6 > 1)
            : farmer.professions.Count(p => p / 6 == which);
    }

    /// <summary>Whether the farmer can reset the specified skill for prestige.</summary>
    /// <param name="skillType">A skill index (0 to 4).</param>
    public static bool CanResetSkill(this Farmer farmer, SkillType skillType)
    {
        var isSkillLevelTen = farmer.GetUnmodifiedSkillLevel((int) skillType) == 10;
        var justLeveledUp = farmer.newLevels.Contains(new((int) skillType, 10));
        var hasAtLeastOneButNotAllProfessionsInSkill =
            farmer.NumberOfProfessionsInSkill((int) skillType, true) is > 0 and < 4;
        var alreadyResetThisSkill =
            EventManager.TryGet<PrestigeDayEndingEvent>(out var prestigeDayEnding) &&
            prestigeDayEnding.SkillsToReset.Value.Contains(skillType);

        return isSkillLevelTen && !justLeveledUp && hasAtLeastOneButNotAllProfessionsInSkill &&
               !alreadyResetThisSkill;
    }

    /// <summary>Whether the farmer can reset any skill for prestige.</summary>
    public static bool CanResetAnySkill(this Farmer farmer)
    {
        return Enum.GetValues<SkillType>().Any(farmer.CanResetSkill);
    }

    /// <summary>Get the cost of resetting the specified skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static int GetResetCost(this Farmer farmer, SkillType skillType)
    {
        var multiplier = ModEntry.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f) return 0;

        var count = farmer.NumberOfProfessionsInSkill((int) skillType, true);
#pragma warning disable 8509
        var baseCost = count switch
#pragma warning restore 8509
        {
            1 => 10000,
            2 => 50000,
            3 => 100000
        };

        return (int) (baseCost * multiplier);
    }

    /// <summary>Resets a specific skill level, removing all associated recipes and bonuses but maintaining profession perks.</summary>
    /// <param name="skillType">The skill to reset.</param>
    public static void ResetSkill(this Farmer farmer, SkillType skillType)
    {
        // reset skill level
        switch (skillType)
        {
            case SkillType.Farming:
                farmer.FarmingLevel = 0;
                break;

            case SkillType.Fishing:
                farmer.FishingLevel = 0;
                break;

            case SkillType.Foraging:
                farmer.ForagingLevel = 0;
                break;

            case SkillType.Mining:
                farmer.MiningLevel = 0;
                break;

            case SkillType.Combat:
                farmer.CombatLevel = 0;
                break;

            case SkillType.Luck:
            default:
                return;
        }

        var toRemove = farmer.newLevels.Where(p => p.X == (int) skillType);
        foreach (var item in toRemove) farmer.newLevels.Remove(item);

        // reset skill experience
        farmer.experiencePoints[(int) skillType] = 0;

        if (ModEntry.Config.ForgetRecipesOnSkillReset)
        {
            var forgottenRecipesDict = ModData.Read(DataField.ForgottenRecipesDict).ToDictionary<string, int>(",", ";");

            // remove associated crafting recipes
            foreach (var recipe in farmer.GetCraftingRecipesForSkill(skillType))
            {
                forgottenRecipesDict.Add(recipe, farmer.craftingRecipes[recipe]);
                farmer.craftingRecipes.Remove(recipe);
            }

            // remove associated cooking recipes
            foreach (var recipe in farmer.GetCookingRecipesForSkill(skillType))
            {
                forgottenRecipesDict.Add(recipe, farmer.cookingRecipes[recipe]);
                farmer.cookingRecipes.Remove(recipe);
            }

            ModData.Write(DataField.ForgottenRecipesDict, forgottenRecipesDict.ToString(",", ";"));
        }

        // revalidate health
        if (skillType == SkillType.Combat) LevelUpMenu.RevalidateHealth(farmer);
    }

    /// <summary>Get all the farmer's crafting recipes associated with a specific skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static IEnumerable<string> GetCraftingRecipesForSkill(this Farmer farmer, SkillType skillType)
    {
        return CraftingRecipe.craftingRecipes.Where(r =>
                r.Value.Split('/')[4].Contains(skillType.ToString()) && farmer.craftingRecipes.ContainsKey(r.Key))
            .Select(recipe => recipe.Key);
    }

    /// <summary>Get all the farmer's cooking recipes associated with a specific skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static IEnumerable<string> GetCookingRecipesForSkill(this Farmer farmer, SkillType skillType)
    {
        return CraftingRecipe.cookingRecipes.Where(r =>
                r.Value.Split('/')[3].Contains(skillType.ToString()) && farmer.cookingRecipes.ContainsKey(r.Key))
            .Select(recipe => recipe.Key);
    }

    /// <summary>Get all available Super Mode's not currently registered.</summary>
    public static IEnumerable<SuperModeIndex> GetUnchosenSuperModes(this Farmer farmer)
    {
        return farmer.professions.Where(p => Enum.IsDefined(typeof(SuperModeIndex), p)).Cast<SuperModeIndex>();
    }

    /// <summary>Whether the farmer has caught the specified fish at max size.</summary>
    /// <param name="index">The fish's index.</param>
    public static bool HasCaughtMaxSized(this Farmer farmer, int index)
    {
        if (!farmer.fishCaught.ContainsKey(index) || farmer.fishCaught[index][1] <= 0) return false;

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        if (!fishData.TryGetValue(index, out var specificFishData)) return false;

        var dataFields = specificFishData.Split('/');
        return farmer.fishCaught[index][1] >= Convert.ToInt32(dataFields[4]);
    }

    /// <summary>Affects the price of produce sold by Producer.</summary>
    /// <param name="who">The player.</param>
    public static float GetProducerPriceBonus(this Farmer farmer)
    {
        return Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse)b.indoors.Value).isFull()).Sum(_ => 0.05f);
    }

    /// <summary>Affects the price of fish sold by Angler.</summary>
    /// <param name="who">The player.</param>
    public static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        var multiplier = 0f;
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (ObjectLookups.LegendaryFishNames.Contains(dataFields[0]))
                multiplier += 0.05f;
            else if (value[1] >= Convert.ToInt32(dataFields[4]))
                multiplier += 0.01f;
        }

        return multiplier;
    }

    /// <summary>Compensates the decay of the "catching" bar for Aquarist.</summary>
    public static float GetAquaristBonusCatchingBarSpeed(this Farmer farmer)
    {
        var fishTypes = Game1.getFarm().buildings
            .Where(b => (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond pond &&
                        pond.fishType.Value > 0)
            .Cast<FishPond>()
            .Select(pond => pond.fishType.Value);

        return Math.Min(fishTypes.Distinct().Count() * 0.000165f, 0.002f);
    }

    /// <summary>Affects the price all items sold by Conservationist.</summary>
    public static float GetConservationistPriceMultiplier(this Farmer farmer)
    {
        return 1f + ModData.ReadAs<float>(DataField.ConservationistActiveTaxBonusPct, farmer);
    }

    /// <summary>Affects the quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = ModData.ReadAs<uint>(DataField.EcologistItemsForaged, farmer);
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Affects the quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = ModData.ReadAs<uint>(DataField.GemologistMineralsCollected, farmer);
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Affects that chance that a ladder or shaft will spawn for Spelunker.</summary>
    public static double GetSpelunkerBonusLadderDownChance(this Farmer farmer)
    {
        if (!farmer.IsLocalPlayer) return 0.0;
        return ModEntry.State.Value.SpelunkerLadderStreak * 0.005;
    }

    /// <summary>Affects the raw damage dealt by Brute.</summary>
    /// <param name="who">The player.</param>
    public static float GetBruteBonusDamageMultiplier(this Farmer farmer)
    {
        var multiplier = 1.15f;
        if (!farmer.IsLocalPlayer || ModEntry.State.Value.SuperMode is not { Index: SuperModeIndex.Brute } superMode)
            return multiplier;

        multiplier += superMode.IsActive
            ? (farmer.HasPrestigedProfession("Fighter") ? 0.2f : 0.1f) + 0.15f + farmer.attackIncreaseModifier + // double fighter, brute and ring bonuses
              (farmer.CurrentTool is not null
                  ? farmer.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f // double enchants
                  : 0f)
              + SuperModeGauge.MaxValue / 10 * 0.005f // apply the maximum fury bonus
            : (int) superMode.Gauge.CurrentValue / 10 * 0.005f; // apply current fury bonus

        return multiplier;
    }

    /// <summary>Affects the cooldown of special moves performed by prestiged Brute.</summary>
    /// <param name="who">The player.</param>
    public static float GetPrestigedBruteCooldownReduction(this Farmer farmer)
    {
        return 1f - farmer.attackIncreaseModifier + (farmer.CurrentTool is not null
            ? farmer.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>Affects the power of critical strikes performed by Poacher.</summary>
    public static float GetPoacherCritDamageMultiplier(this Farmer farmer)
    {
        if (!farmer.IsLocalPlayer) return 1f;

        return ModEntry.State.Value.SuperMode.IsActive
            ? 1f + SuperModeGauge.MaxValue / 10 * 0.04f // apply the maximum cold blood bonus
            : 1f + (int) ModEntry.State.Value.SuperMode.Gauge.CurrentValue / 10 * 0.04f; // apply current cold blood bonus
    }

    /// <summary>Affects the cooldown special moves performed by prestiged Poacher.</summary>
    /// <param name="who">The player.</param>
    public static float GetPrestigedPoacherCooldownReduction(this Farmer farmer)
    {
        return 1f - farmer.critChanceModifier + farmer.critPowerModifier + (farmer.CurrentTool is not null
            ? farmer.CurrentTool.GetEnchantmentLevel<AquamarineEnchantment>() +
              farmer.CurrentTool.GetEnchantmentLevel<JadeEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>Affects the maximum number of bonus Slimes that can be attracted by Piper.</summary>
    public static int GetPiperSlimeSpawnAttempts(this Farmer farmer)
    {
        if (!farmer.IsLocalPlayer) return 0;

        return ModEntry.State.Value.SuperMode.IsActive
            ? SuperModeGauge.MaxValue / 50 + 1 // apply the maximum eubstance bonus
            : (int) ModEntry.State.Value.SuperMode.Gauge.CurrentValue / 50 + 1; // apply current eubstance bonus
    }

    /// <summary>Affects the chance to shoot twice consecutively for Desperado.</summary>
    /// <param name="who">The player.</param>
    public static float GetDesperadoDoubleStrafeChance(this Farmer farmer)
    {
        var healthPercent = (double) farmer.health / farmer.maxHealth;
        return (float) Math.Min(2 / (healthPercent + 1.5) - 0.75, 0.5f);
    }

    /// <summary>Affects projectile velocity, knockback, hitbox size and pierce chance for Desperado.</summary>
    public static float GetDesperadoBulletPower(this Farmer farmer)
    {
        if (!farmer.IsLocalPlayer || ModEntry.State.Value.SuperMode.IsActive) return 1f;
        return 1f + (int)ModEntry.State.Value.SuperMode.Gauge.CurrentValue / 10 * 0.01f;
    }
}