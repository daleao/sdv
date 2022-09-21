// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Collections;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.Utility;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Determines whether the <paramref name="farmer"/> has a particular <paramref name="profession"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasProfession(this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        if (prestiged && !profession.Id.IsAnyOf(Profession.GetRange()))
        {
            return false;
        }

        return farmer.professions.Contains(profession.Id + (prestiged ? 100 : 0));
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has acquired both level ten professions branching from the
    ///     specified level five <paramref name="profession"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has both tier-two professions which branch from <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAllProfessionsBranchingFrom(this Farmer farmer, IProfession profession)
    {
        return profession.BranchingProfessions.All(farmer.professions.Contains);
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has all six professions in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has all four professions belonging to the <paramref name="skill"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAllProfessionsInSkill(this Farmer farmer, ISkill skill)
    {
        return skill.ProfessionIds.All(farmer.professions.Contains);
    }

    /// <summary>Determines whether the <paramref name="farmer"/> has all available professions (vanilla + modded).</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="includeCustom">Whether to include <see cref="CustomProfession"/>s in the count.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has all 30 vanilla professions, otherwise <see langword="false"/>.</returns>
    public static bool HasAllProfessions(this Farmer farmer, bool includeCustom = false)
    {
        var allProfessions = Enumerable.Range(0, 30);
        if (includeCustom)
        {
            allProfessions = allProfessions.Concat(CustomProfession.LoadedProfessions.Values.Select(p => p.Id));
        }

        return allProfessions.All(farmer.professions.Contains);
    }

    /// <summary>
    ///     Gets the most recent tier-one profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentBranchForSkill(this Farmer farmer, ISkill skill)
    {
        return farmer.professions.Where(pid => pid.IsAnyOf(skill.TierOneProfessionIds)).DefaultIfEmpty(-1).Last();
    }

    /// <summary>
    ///     Gets the most recent tier-two profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="branch"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="branch">The branch (level 5 <see cref="IProfession"/>) to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentProfessionForBranch(this Farmer farmer, IProfession branch)
    {
        return farmer.professions.Where(pid => pid.IsAnyOf(branch.BranchingProfessions)).DefaultIfEmpty(-1).Last();
    }

    /// <summary>Gets all the <paramref name="farmer"/>'s professions associated with a specific <paramref name="skill"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="IProfession"/>s in <paramref name="skill"/>.</returns>
    public static IProfession[] GetProfessionsForSkill(
        this Farmer farmer, ISkill skill, bool excludeTierOneProfessions = false)
    {
        return farmer.professions
            .Intersect(excludeTierOneProfessions ? skill.TierTwoProfessionIds : skill.ProfessionIds)
            .Select<int, IProfession>(id =>
                CustomSkill.Loaded.ContainsKey(skill.StringId)
                    ? CustomProfession.LoadedProfessions[id]
                    : Profession.FromValue(id)).ToArray();
    }

    /// <summary>
    ///     Gets the professions which the <paramref name="farmer"/> is missing in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of un-obtained <see cref="IProfession"/>s.</returns>
    public static IProfession[] GetMissingProfessionsInSkill(
        this Farmer farmer, ISkill skill, bool excludeTierOneProfessions = false)
    {
        return excludeTierOneProfessions
            ? skill.Professions.Where(p => p.Level == 10 && !farmer.professions.Contains(p.Id)).ToArray()
            : skill.Professions.Where(p => !farmer.professions.Contains(p.Id)).ToArray();
    }

    /// <summary>
    ///     Gets the last acquired profession by the <paramref name="farmer"/> in the specified subset, or simply the
    ///     last acquired profession if no subset is specified.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="subset">An array of profession ids.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetMostRecentProfession(this Farmer farmer, IEnumerable<int>? subset = null)
    {
        return subset is null
            ? farmer.professions[^1]
            : farmer.professions.Where(p => p.IsAnyOf(subset)).DefaultIfEmpty(-1).Last();
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> can reset the specified <paramref name="skill"/> for
    ///     prestige.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="skill"/> can be reset, otherwise <see langword="false"/>.</returns>
    public static bool CanResetSkill(this Farmer farmer, ISkill skill)
    {
        if (skill is Skill vanillaSkill && vanillaSkill == Farmer.luckSkill && ModEntry.LuckSkillApi is null)
        {
            return false;
        }

        var isSkillLevelTen = skill.CurrentLevel >= 10;
        if (!isSkillLevelTen)
        {
            Log.D($"{skill.StringId} skill cannot be reset because it's level is lower than 10.");
            return false;
        }

        var justLeveledUp = skill.NewLevels.Contains(10);
        if (justLeveledUp)
        {
            Log.D($"{skill.StringId} cannot be reset because {farmer.Name} has not seen the level-up menu.");
            return false;
        }

        var hasProfessionsLeftToAcquire =
            farmer.GetProfessionsForSkill(skill, true).Length is > 0 and < 4;
        if (!hasProfessionsLeftToAcquire)
        {
            Log.D(
                $"{skill.StringId} cannot be reset because {farmer.Name} either already has all professions in the skill, or none at all.");
            return false;
        }

        var alreadyResetThisSkill = ModEntry.State.SkillsToReset.Contains(skill);
        if (alreadyResetThisSkill)
        {
            Log.D($"{skill.StringId} has already been marked for reset tonight.");
            return false;
        }

        return true;
    }

    /// <summary>Determines whether the <paramref name="farmer"/> can reset any <see cref="ISkill"/> for prestige.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if at least one of the <paramref name="farmer"/>'s <see cref="ISkill"/>s can be reset, otherwise <see langword="false"/>.</returns>
    public static bool CanResetAnySkill(this Farmer farmer)
    {
        return Skill.List.Any(farmer.CanResetSkill) || CustomSkill.Loaded.Values.Any(farmer.CanResetSkill);
    }

    /// <summary>Gets the cost of resetting the specified <paramref name="skill"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>A sum of gold to be paid.</returns>
    public static int GetResetCost(this Farmer farmer, ISkill skill)
    {
        var multiplier = ModEntry.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f)
        {
            return 0;
        }

        var count = farmer.GetProfessionsForSkill(skill, true).Length;
        var baseCost = count switch
        {
            1 => 10000,
            2 => 50000,
            3 => 100000,
            _ => 0,
        };

        return (int)(baseCost * multiplier);
    }

    /// <summary>
    ///     Resets the <paramref name="skill"/>'s level, optionally removing associated recipes, but maintaining acquired
    ///     profession.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="Skill"/> to reset.</param>
    public static void ResetSkill(this Farmer farmer, Skill skill)
    {
        // reset skill level
        skill
            .When(Skill.Farming).Then(() => farmer.farmingLevel.Value = 0)
            .When(Skill.Fishing).Then(() => farmer.fishingLevel.Value = 0)
            .When(Skill.Foraging).Then(() => farmer.foragingLevel.Value = 0)
            .When(Skill.Mining).Then(() => farmer.miningLevel.Value = 0)
            .When(Skill.Combat).Then(() => farmer.combatLevel.Value = 0)
            .When(Skill.Luck).Then(() => farmer.luckLevel.Value = 0);

        var toRemove = farmer.newLevels.Where(p => p.X == skill);
        foreach (var item in toRemove)
        {
            farmer.newLevels.Remove(item);
        }

        // reset skill experience
        farmer.experiencePoints[skill] = 0;

        if (ModEntry.Config.ForgetRecipes && skill < Skill.Luck)
        {
            farmer.ForgetRecipesForSkill(skill, true);
        }

        // revalidate health
        if (skill == Farmer.combatSkill)
        {
            LevelUpMenu.RevalidateHealth(farmer);
        }

        Log.D($"Farmer {farmer.Name}'s {skill.DisplayName} skill has been reset.");
    }

    /// <summary>
    ///     Resets the <paramref name="skill"/>'s level, removing all associated recipes and bonuses but maintaining
    ///     profession perks.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="CustomSkill"/> to reset.</param>
    public static void ResetCustomSkill(this Farmer farmer, CustomSkill skill)
    {
        ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(farmer, skill.StringId, -skill.CurrentExp);
        if (ModEntry.Config.ForgetRecipes && skill.StringId == "blueberry.LoveOfCooking.CookingSkill")
        {
            farmer.ForgetRecipesForLoveOfCookingSkill(true);
        }

        Log.D($"Farmer {farmer.Name}'s {skill.DisplayName} skill has been reset.");
    }

    /// <summary>Sets the level of the specified <paramref name="skill"/> for this <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="Skill"/> whose level should be set.</param>
    /// <param name="newLevel">The new level.</param>
    /// <remarks>Will not change professions or recipes.</remarks>
    public static void SetSkillLevel(this Farmer farmer, Skill skill, int newLevel)
    {
        skill
            .When(Skill.Farming).Then(() => farmer.farmingLevel.Value = newLevel)
            .When(Skill.Fishing).Then(() => farmer.fishingLevel.Value = newLevel)
            .When(Skill.Foraging).Then(() => farmer.foragingLevel.Value = newLevel)
            .When(Skill.Mining).Then(() => farmer.miningLevel.Value = newLevel)
            .When(Skill.Combat).Then(() => farmer.combatLevel.Value = newLevel)
            .When(Skill.Luck).Then(() => farmer.luckLevel.Value = newLevel);
    }

    /// <summary>Sets the level of the specified custom <paramref name="skill"/> for this <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="CustomSkill"/> whose level should be set.</param>
    /// <param name="newLevel">The new level.</param>
    /// <remarks>Will not change professions or recipes.</remarks>
    public static void SetCustomSkillLevel(this Farmer farmer, CustomSkill skill, int newLevel)
    {
        newLevel = Math.Min(newLevel, 10);
        var diff = ISkill.ExperienceByLevel[newLevel] - skill.CurrentExp;
        ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(farmer, skill.StringId, diff);
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/>'s skill levels correspond with expectations for their
    ///     respective experience points, and if not fixes those levels.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    public static void RevalidateLevels(this Farmer farmer)
    {
        foreach (var skill in Skill.List)
        {
            if (skill == Skill.Luck && skill.CurrentExp > 0 && ModEntry.LuckSkillApi is null)
            {
                Log.W(
                    $"Farmer {farmer.Name} has gained Luck experience, but Luck Skill mod is not installed. The Luck skill will be reset.");
                farmer.ResetSkill(skill);
                continue;
            }

            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && farmer.HasAllProfessionsInSkill(skill) &&
                                        skill == Skill.Luck;
            switch (skill.CurrentLevel)
            {
                case >= 10 when !canGainPrestigeLevels:
                {
                    if (skill.CurrentLevel > 10)
                    {
                        farmer.SetSkillLevel(skill, 10);
                    }

                    if (skill.CurrentExp > ISkill.VanillaExpCap)
                    {
                        farmer.experiencePoints[skill] = ISkill.VanillaExpCap;
                    }

                    break;
                }

                case >= 20 when canGainPrestigeLevels:
                {
                    if (skill.CurrentLevel > 20)
                    {
                        farmer.SetSkillLevel(skill, 20);
                    }

                    if (skill.CurrentExp > ISkill.ExperienceByLevel[20])
                    {
                        farmer.experiencePoints[skill] = ISkill.ExperienceByLevel[20];
                    }

                    break;
                }

                default:
                {
                    var expectedLevel = 0;
                    var level = 1;
                    while (level <= 10 && skill.CurrentExp >= ISkill.ExperienceByLevel[level++])
                    {
                        ++expectedLevel;
                    }

                    if (canGainPrestigeLevels && skill.CurrentExp - ISkill.VanillaExpCap > 0)
                    {
                        while (level <= 20 && skill.CurrentExp >= ISkill.ExperienceByLevel[level++])
                        {
                            ++expectedLevel;
                        }
                    }

                    if (skill.CurrentLevel != expectedLevel)
                    {
                        if (skill.CurrentLevel < expectedLevel)
                        {
                            for (var levelUp = skill.CurrentLevel + 1; levelUp <= expectedLevel; ++levelUp)
                            {
                                var point = new Point(skill, levelUp);
                                if (!farmer.newLevels.Contains(point))
                                {
                                    farmer.newLevels.Add(point);
                                }
                            }
                        }

                        farmer.SetSkillLevel(skill, expectedLevel);
                    }

                    farmer.experiencePoints[skill] = skill.CurrentLevel switch
                    {
                        >= 10 when !canGainPrestigeLevels => ISkill.VanillaExpCap,
                        >= 20 when canGainPrestigeLevels => ISkill.ExperienceByLevel[20],
                        _ => farmer.experiencePoints[skill],
                    };

                    break;
                }
            }
        }
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/>'s registered <see cref="IUltimate"/> is valid, or whether they
    ///     should be assigned one based on their professions.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    public static void RevalidateUltimate(this Farmer farmer)
    {
        var ultimateIndex = farmer.Read("UltimateIndex", -1);
        switch (ultimateIndex)
        {
            case < 0 when farmer.professions.Any(p => p is >= 26 and < 30):
                Log.W(
                    $"{farmer.Name} is eligible for Ultimate but is not currently registered to any. The registered Ultimate will be set to a default value.");
                ultimateIndex = farmer.professions.First(p => p is >= 26 and < 30);
                break;
            case >= 0 when !farmer.professions.Contains(ultimateIndex):
            {
                Log.W($"{farmer.Name} is registered to Ultimate index {ultimateIndex} but is missing the corresponding profession. The registered Ultimate will be reset.");
                ultimateIndex = farmer.professions.Any(p => p is >= 26 and < 30)
                    ? farmer.professions.First(p => p is >= 26 and < 30)
                    : -1;

                break;
            }
        }

        if (ultimateIndex > 0)
        {
            farmer.Set_Ultimate(Ultimate.FromValue(ultimateIndex));
        }
    }

    /// <summary>
    ///     Removes all recipes associated with the specified <paramref name="skill"/> from the
    ///     <paramref name="farmer"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="Skill"/>.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForSkill(this Farmer farmer, Skill skill, bool addToRecoveryDict = false)
    {
        var forgottenRecipesDict = farmer.Read("ForgottenRecipesDict")
            .ParseDictionary<string, int>();

        // remove associated crafting recipes
        var craftingRecipes =
            farmer.craftingRecipes.Keys.ToDictionary(
                key => key,
                key => farmer.craftingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.craftingRecipes)
        {
            if (!value.Split('/')[4].Contains(skill.StringId) || !craftingRecipes.ContainsKey(key))
            {
                continue;
            }

            if (addToRecoveryDict)
            {
                if (!forgottenRecipesDict.TryAdd(key, craftingRecipes[key]))
                {
                    forgottenRecipesDict[key] += craftingRecipes[key];
                }
            }

            farmer.craftingRecipes.Remove(key);
        }

        // remove associated cooking recipes
        var cookingRecipes =
            farmer.cookingRecipes.Keys.ToDictionary(
                key => key,
                key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.cookingRecipes)
        {
            if (!value.Split('/')[3].Contains(skill.StringId) || !cookingRecipes.ContainsKey(key))
            {
                continue;
            }

            if (addToRecoveryDict)
            {
                if (!forgottenRecipesDict.TryAdd(key, cookingRecipes[key]))
                {
                    forgottenRecipesDict[key] += cookingRecipes[key];
                }
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (addToRecoveryDict)
        {
            farmer.Write("ForgottenRecipesDict", forgottenRecipesDict.Stringify());
        }
    }

    /// <summary>Removes all recipes associated with the Love Of Cooking skill from the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForLoveOfCookingSkill(this Farmer farmer, bool addToRecoveryDict = false)
    {
        var forgottenRecipesDict = farmer.Read("ForgottenRecipesDict")
            .ParseDictionary<string, int>();

        // remove associated cooking recipes
        var cookingRecipes = ModEntry.CookingSkillApi!
            .GetAllLevelUpRecipes().Values
            .SelectMany(r => r)
            .Select(r => "blueberry.cac." + r)
            .ToList();
        var knownCookingRecipes = farmer.cookingRecipes.Keys.Where(key => key.IsAnyOf(cookingRecipes)).ToDictionary(
            key => key,
            key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in knownCookingRecipes)
        {
            if (addToRecoveryDict && !forgottenRecipesDict.TryAdd(key, value))
            {
                forgottenRecipesDict[key] += value;
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (addToRecoveryDict)
        {
            farmer.Write("ForgottenRecipesDict", forgottenRecipesDict.Stringify());
        }
    }

    /// <summary>
    ///     Gets the total experience multiplier based on the <paramref name="farmer"/>'s configs and
    ///     <paramref name="skill"/> reset count.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The corresponding skill.</param>
    /// <returns>A <see cref="float"/> multiplier for the <paramref name="skill"/>'s experience.</returns>
    public static float GetExperienceMultiplier(this Farmer farmer, ISkill skill)
    {
        return skill.BaseExperienceMultiplier * (ModEntry.Config.EnablePrestige
            ? (float)Math.Pow(
                1f + ModEntry.Config.PrestigeExpMultiplier,
                farmer.GetProfessionsForSkill(skill, true).Length)
            : 1f);
    }

    /// <summary>Gets the <see cref="Ultimate"/> options which are not currently registered.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ultimate"/>s, excluding the currently registered.</returns>
    public static IEnumerable<Ultimate> GetUnchosenUltimates(this Farmer farmer)
    {
        var chosen = farmer.Get_Ultimate();
        return farmer.professions.Where(p => p is >= 26 and < 30)
            .Except(chosen is not null ? new[] { chosen.Value } : Enumerable.Empty<int>()).Select(Ultimate.FromValue);
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has caught the fish with specified <paramref name="index"/>
    ///     at max size.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="index">The fish's index.</param>
    /// <returns><see langword="true"/> if the fish with the specified <paramref name="index"/> has been caught with max size, otherwise <see langword="false"/>.</returns>
    public static bool HasCaughtMaxSized(this Farmer farmer, int index)
    {
        if (!farmer.fishCaught.ContainsKey(index) || farmer.fishCaught[index][1] <= 0)
        {
            return false;
        }

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        if (!fishData.TryGetValue(index, out var specificFishData))
        {
            return false;
        }

        var dataFields = specificFishData.Split('/');
        return farmer.fishCaught[index][1] >= Convert.ToInt32(dataFields[4]);
    }

    /// <summary>Gets the price bonus applied to animal produce sold by <see cref="Profession.Producer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    public static float GetProducerPriceBonus(this Farmer farmer)
    {
        return Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse)b.indoors.Value).isFull()).Sum(_ => 0.05f);
    }

    /// <summary>Gets the bonus catching bar speed for prestiged <see cref="Profession.Fisher"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="whichFish">The fish index.</param>
    /// <returns>A <see cref="float"/> catching bar height.</returns>
    /// <remarks>UNUSED.</remarks>
    public static float GetFisherBonusCatchingBarSpeed(this Farmer farmer, int whichFish)
    {
        return farmer.fishCaught.TryGetValue(whichFish, out var caughtData)
            ? caughtData[0] >= ModEntry.Config.FishNeededForInstantCatch
                ? 1f
                : Math.Max(caughtData[0] * (0.1f / ModEntry.Config.FishNeededForInstantCatch) * 0.0002f, 0.002f)
            : 0.002f;
    }

    /// <summary>Gets the price bonus applied to fish sold by <see cref="Profession.Angler"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    public static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAlgaeIndex() && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        var bonus = 0f;
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData))
            {
                continue;
            }

            var dataFields = specificFishData.Split('/');
            if (Lookups.LegendaryFishNames.Contains(dataFields[0]))
            {
                bonus += 0.05f;
            }
            else if (value[1] >= Convert.ToInt32(dataFields[4]))
            {
                bonus += 0.01f;
            }
        }

        return Math.Min(bonus, ModEntry.Config.AnglerMultiplierCap);
    }

    /// <summary>Gets the amount of "catching" bar to compensate for <see cref="Profession.Aquarist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> catching height.</returns>
    public static float GetAquaristCatchingBarCompensation(this Farmer farmer)
    {
        var fishTypes = Game1.getFarm().buildings
            .OfType<FishPond>()
            .Where(pond => (pond.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                           pond.fishType.Value > 0)
            .Select(pond => pond.fishType.Value);

        return Math.Min(fishTypes.Distinct().Count() * 0.000165f, 0.002f);
    }

    /// <summary>Gets the price bonus applied to all items sold by <see cref="Profession.Conservationist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for general items.</returns>
    public static float GetConservationistPriceMultiplier(this Farmer farmer)
    {
        return 1f + farmer.Read<float>("ConservationistActiveTaxBonusPct");
    }

    /// <summary>Gets the quality of items foraged by <see cref="Profession.Ecologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = farmer.Read<uint>("EcologistItemsForaged");
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Gets the quality of minerals collected by <see cref="Profession.Gemologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = farmer.Read<uint>("GemologistMineralsCollected");
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Enumerates the <see cref="GreenSlime"/>s currently inhabiting owned <see cref="SlimeHutch"/>es.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="GreenSlime"/>s currently inhabiting owned <see cref="SlimeHutch"/>es.</returns>
    public static IEnumerable<GreenSlime> GetRaisedSlimes(this Farmer farmer)
    {
        return Game1.getFarm().buildings
            .Where(b => (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        b.indoors.Value is SlimeHutch && !b.isUnderConstruction())
            .SelectMany(b => b.indoors.Value.characters.OfType<GreenSlime>());
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> is currently using the <see cref="Profession.Poacher"/>
    ///     <see cref="Ultimate"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if <see cref="Ultimate.PoacherAmbush"/> is active.</returns>
    public static bool IsInAmbush(this Farmer farmer)
    {
        return farmer.Get_Ultimate() == Ultimate.PoacherAmbush && Ultimate.PoacherAmbush.IsActive;
    }
}
