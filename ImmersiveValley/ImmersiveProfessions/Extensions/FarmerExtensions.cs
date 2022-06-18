// ReSharper disable PossibleLossOfFraction
#nullable enable
namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Monsters;

using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Stardew;
using Common.Integrations;
using Framework;
using Framework.Ultimate;
using Framework.Utility;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the regular or prestiged variant.</param>
    public static bool HasProfession(this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        if (prestiged && !profession.Id.IsIn(Profession.GetRange())) return false;
        return farmer.professions.Contains(profession.Id + (prestiged ? 100 : 0));
    }

    ///// <summary>Whether the farmer has a particular profession.</summary>
    ///// <param name="index">A valid profession index.</param>
    ///// <param name="prestiged">Whether to check for the regular or prestiged variant.</param>
    //public static bool HasProfession(this Farmer farmer, int index, bool prestiged = false)
    //{
    //    if (Profession.TryFromValue(index, out _))
    //        return farmer.professions.Contains(index + (prestiged ? 100 : 0));

    //    if (!prestiged && ModEntry.CustomProfessions.ContainsKey(index))
    //        return farmer.professions.Contains(index);

    //    return false;
    //}

    /// <summary>Whether the farmer has acquired both level ten professions branching from the specified level five profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    public static bool HasAllProfessionsBranchingFrom(this Farmer farmer, IProfession profession)
    {
        return profession.BranchingProfessions.All(farmer.professions.Contains);
    }

    ///// <summary>Whether the farmer has acquired both level ten professions branching from the specified level five profession.</summary>
    ///// <param name="index">A valid profession index.</param>
    //public static bool HasAllProfessionsBranchingFrom(this Farmer farmer, int index)
    //{
    //    if (Profession.TryFromValue(index, out var profession))
    //        return farmer.HasAllProfessionsBranchingFrom((IProfession) profession);
        
    //    if (ModEntry.CustomProfessions.TryGetValue(index, out var customProfession))
    //        return farmer.HasAllProfessionsBranchingFrom(customProfession);

    //    return false;
    //}

    /// <summary>Whether the farmer has all six professions in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    public static bool HasAllProfessionsInSkill(this Farmer farmer, ISkill skill)
    {
        return skill.ProfessionIds.All(farmer.professions.Contains);
    }

    ///// <summary>Whether the farmer has all six professions in the specified skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    //public static bool HasAllProfessionsInSkill(this Farmer farmer, int index)
    //{
    //    return Skill.TryFromValue(index, out var skill) && skill.ProfessionIds.All(farmer.professions.Contains);
    //}

    ///// <summary>Whether the farmer has all six professions in the specified custom skill.</summary>
    ///// <param name="skillId">A valid custom skill id.</param>
    //public static bool HasAllProfessionsInSkill(this Farmer farmer, string skillId)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill) &&
    //           skill.ProfessionIds.All(farmer.professions.Contains);
    //}

    /// <summary>Whether the farmer has all available professions (vanilla + modded).</summary>
    public static bool HasAllProfessions(this Farmer farmer)
    {
        return Enumerable.Range(0, 30).Concat(ModEntry.CustomProfessions.Values.Select(p => p.Id))
            .All(farmer.professions.Contains);
    }

    /// <summary>Get the last 1st-tier profession acquired by the farmer in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentBranchForSkill(this Farmer farmer, ISkill skill)
    {
        return farmer.professions.Where(pid => pid.IsIn(skill.TierOneProfessionIds)).DefaultIfEmpty(-1).Last();
    }

    ///// <summary>Get the last 1st-tier profession acquired by the farmer in the specified skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    ///// <returns>The last acquired profession index, or -1 if none was found.</returns>
    //public static int GetCurrentBranchForSkill(this Farmer farmer, int index)
    //{
    //    return !Skill.TryFromValue(index, out var skill) ? farmer.GetCurrentBranchForSkill((ISkill) skill) : -1;
    //}

    ///// <summary>Get the last 1st-tier profession acquired by the farmer in the specified custom skill.</summary>
    ///// <param name="skillId">A valid custom skill id.</param>
    ///// <returns>The last acquired profession index, or -1 if none was found.</returns>
    //public static int GetCurrentBranchForSkill(this Farmer farmer, string skillId)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill) ? farmer.GetCurrentBranchForSkill(skill) : -1;
    //}

    /// <summary>Get the last level 2nd-tier profession acquired by the farmer in the specified skill branch.</summary>
    /// <param name="branch">The branch (level 5 <see cref="IProfession"/>) to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentProfessionForBranch(this Farmer farmer, IProfession branch)
    {
        return farmer.professions.Where(pid => pid.IsIn(branch.BranchingProfessions)).DefaultIfEmpty(-1).Last();
    }

    ///// <summary>Get the last level 2nd-tier profession acquired by the farmer branching from the specified 1st-tier branch.</summary>
    ///// <param name="index">A valid branch (level 5 profession) index.</param>
    ///// <returns>The last acquired profession index, or -1 if none was found.</returns>
    //public static int GetCurrentProfessionForBranch(this Farmer farmer, int index)
    //{
    //    if (Profession.TryFromValue(index, out var profession) && profession.Level == 5)
    //        return farmer.GetCurrentProfessionForBranch((IProfession) profession);

    //    if (ModEntry.CustomProfessions.TryGetValue(index, out var customProfession) && customProfession.Level == 5)
    //        return farmer.GetCurrentProfessionForBranch(customProfession);

    //    return -1;
    //}

    /// <summary>Get all the farmer's professions associated with a specific skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    public static IEnumerable<IProfession> GetProfessionsForSkill(this Farmer farmer, ISkill skill,
        bool excludeTierOneProfessions = false)
    {
        var ids = farmer.professions.Intersect(
            excludeTierOneProfessions
            ? skill.TierTwoProfessionIds
            : skill.ProfessionIds
        );

        return ModEntry.CustomSkills.ContainsKey(skill.StringId)
            ? ids.Select(id => ModEntry.CustomProfessions[id])
            : ids.Select(Profession.FromValue);
    }

    ///// <summary>Get all the farmer's professions associated with a specific skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    ///// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    //public static IEnumerable<IProfession> GetProfessionsInSkill(this Farmer farmer, int index,
    //    bool excludeTierOneProfessions = false)
    //{
    //    return Skill.TryFromValue(index, out var skill)
    //        ? farmer.GetProfessionsInSkill((ISkill) skill, excludeTierOneProfessions)
    //        : Enumerable.Empty<IProfession>();
    //}

    ///// <summary>Get all the farmer's professions associated with a specific custom skill.</summary>
    ///// <param name="skillId">A valid custom skill id.</param>
    ///// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    //public static IEnumerable<IProfession> GetProfessionsInSkill(this Farmer farmer, string skillId,
    //    bool excludeTierOneProfessions = false)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill)
    //        ? farmer.GetProfessionsInSkill(skill, excludeTierOneProfessions)
    //        : Enumerable.Empty<IProfession>();
    //}

    /// <summary>Get the professions which the player is missing in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    public static IEnumerable<IProfession> GetMissingProfessionsInSkill(this Farmer farmer, ISkill skill,
        bool excludeTierOneProfessions = false)
    {
        return excludeTierOneProfessions
            ? skill.Professions.Where(p => p.Level == 10 && !farmer.professions.Contains(p.Id))
            : skill.Professions.Where(p => !farmer.professions.Contains(p.Id));
    }

    ///// <summary>Get the professions which the player is missing in the specified skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    ///// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    //public static IEnumerable<int> GetMissingProfessionsInSkill(this Farmer farmer, int index,
    //    bool excludeTierOneProfessions = false)
    //{
    //    return Skill.TryFromValue(index, out var skill)
    //        ? farmer.GetMissingProfessionsInSkill((ISkill) skill, excludeTierOneProfessions)
    //        : Enumerable.Empty<int>();
    //}

    ///// <summary>Get the professions which the player is missing in the specified custom skill.</summary>
    ///// <param name="skillId">A valid custom skill index.</param>
    ///// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    //public static IEnumerable<int> GetMissingProfessionsInSkill(this Farmer farmer, string skillId,
    //    bool excludeTierOneProfessions = false)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill)
    //        ? farmer.GetMissingProfessionsInSkill(skill, excludeTierOneProfessions)
    //        : Enumerable.Empty<int>();
    //}

    /// <summary>Get the last acquired profession by the farmer in the specified subset, or simply the last acquired profession if no subset is specified.</summary>
    /// <param name="subset">An array of profession ids.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetMostRecentProfession(this Farmer farmer, IEnumerable<int>? subset = null)
    {
        return subset is null
            ? farmer.professions[^1]
            : farmer.professions.Where(p => p.IsIn(subset)).DefaultIfEmpty(-1).Last();
    }

    /// <summary>Whether the farmer can reset the specified skill for prestige.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    public static bool CanResetSkill(this Farmer farmer, ISkill skill)
    {
        if (skill is Skill vanillaSkill && vanillaSkill == Skill.Luck && ModEntry.LuckSkillApi is null) return false;

        var isSkillLevelTen = skill.CurrentLevel == 10;
        if (!isSkillLevelTen)
        {
            Log.D($"{skill.StringId} skill cannot be reset because it's level is not 10.");
            return false;
        }

        var justLeveledUp = skill.NewLevels.Contains(10);
        if (justLeveledUp)
        {
            Log.D($"{skill.StringId} cannot be reset because {farmer.Name} has not seen the level-up menu.");
            return false;
        }

        var hasAtLeastOneButNotAllProfessionsInSkill =
            farmer.GetProfessionsForSkill(skill, true).Count() is > 0 and < 4;
        if (!hasAtLeastOneButNotAllProfessionsInSkill)
        {
            Log.D(
                $"{skill.StringId} cannot be reset because {farmer.Name} either already has all professions in the skill, or none at all.");
            return false;
        }

        var alreadyResetThisSkill = ModEntry.PlayerState.SkillsToReset.Contains(skill);
        if (alreadyResetThisSkill)
        {
            Log.D($"{skill.StringId} has already been marked for reset tonight.");
            return false;
        }

        return true;
    }

    ///// <summary>Whether the farmer can reset the specified skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    //public static bool CanResetSkill(this Farmer farmer, int index)
    //{
    //    return Skill.TryFromValue(index, out var skill) && farmer.CanResetSkill((ISkill) skill);
    //}

    ///// <summary>Whether the farmer can reset the specified custom skill.</summary>
    ///// <param name="skillId">A valid custom skill id.</param>
    //public static bool CanResetSkill(this Farmer farmer, string skillId)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill) && farmer.CanResetSkill(skill);
    //}

    /// <summary>Whether the farmer can reset any skill for prestige.</summary>
    public static bool CanResetAnySkill(this Farmer farmer)
    {
        return Skill.List.Any(farmer.CanResetSkill) || ModEntry.CustomSkills.Values.Any(farmer.CanResetSkill);
    }

    /// <summary>Get the cost of resetting the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    public static int GetResetCost(this Farmer farmer, ISkill skill)
    {
        var multiplier = ModEntry.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f) return 0;

        var count = farmer.GetProfessionsForSkill(skill, true).Count();
        var baseCost = count switch
        {
            1 => 10000,
            2 => 50000,
            3 => 100000,
            _ => 0
        };

        return (int) (baseCost * multiplier);
    }

    ///// <summary>Get the cost of resetting the specified skill.</summary>
    ///// <param name="index">A valid skill index.</param>
    //public static int GetResetCost(this Farmer farmer, int index)
    //{
    //    return Skill.TryFromValue(index, out var skill) ? farmer.GetResetCost((ISkill) skill) : int.MaxValue;
    //}

    ///// <summary>Get the cost of resetting the specified skill.</summary>
    ///// <param name="skillId">A valid custom skill index.</param>
    //public static int GetResetCost(this Farmer farmer, string skillId)
    //{
    //    return ModEntry.CustomSkills.TryGetValue(skillId, out var skill) ? farmer.GetResetCost(skill) : int.MaxValue;
    //}

    /// <summary>Reset the skill's level, optionally removing associated recipes, but maintaining acquired profession.</summary>
    /// <param name="skill">The <see cref="Skill"/> to reset.</param>
    public static void ResetSkill(this Farmer farmer, Skill skill)
    {
        // reset skill level
        switch (skill)
        {
            case Farmer.farmingSkill:
                farmer.farmingLevel.Value = 0;
                break;
            case Farmer.fishingSkill:
                farmer.fishingLevel.Value = 0;
                break;
            case Farmer.foragingSkill:
                farmer.foragingLevel.Value = 0;
                break;
            case Farmer.miningSkill:
                farmer.miningLevel.Value = 0;
                break;
            case Farmer.combatSkill:
                farmer.combatLevel.Value = 0;
                break;
            case Farmer.luckSkill:
                farmer.luckLevel.Value = 0;
                break;
            default:
                return;
        }

        var toRemove = farmer.newLevels.Where(p => p.X == skill);
        foreach (var item in toRemove) farmer.newLevels.Remove(item);

        // reset skill experience
        farmer.experiencePoints[skill] = 0;

        if (ModEntry.Config.ForgetRecipesOnSkillReset && skill < Skill.Luck)
            farmer.ForgetRecipesForSkill(skill, true);

        // revalidate health
        if (skill == Skill.Combat) LevelUpMenu.RevalidateHealth(farmer);

        Log.D($"Farmer {farmer.Name}'s {skill.DisplayName} skill has been reset.");
    }

    /// <summary>Resets a specific skill level, removing all associated recipes and bonuses but maintaining profession perks.</summary>
    /// <param name="skill">The <see cref="CustomSkill"/> to reset.</param>
    public static void ResetCustomSkill(this Farmer farmer, CustomSkill skill)
    {
        ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(farmer, skill.StringId, -skill.CurrentExp);
        if (ModEntry.Config.ForgetRecipesOnSkillReset && skill.StringId == "blueberry.LoveOfCooking.CookingSkill")
            farmer.ForgetRecipesForLoveOfCookingSkill(true);

        Log.D($"Farmer {farmer.Name}'s {skill.DisplayName} skill has been reset.");
    }

    /// <summary>Set the level of the specified skill for this farmer.</summary>
    /// <param name="skill">The <see cref="Skill"/> whose level should be set.</param>
    /// <param name="newLevel">The new level.</param>
    /// <param name="setExperience">Whether to set the skill's experience to the corresponding value.</param>
    /// <remarks>Will not change professions or recipes.</remarks>
    public static void SetSkillLevel(this Farmer farmer, Skill skill, int newLevel, bool setExperience = false)
    {
        var oldLevel = 0;
        switch (skill)
        {
            case Farmer.farmingSkill:
                oldLevel = farmer.farmingLevel.Value;
                farmer.farmingLevel.Value = newLevel;
                break;
            case Farmer.fishingSkill:
                oldLevel = farmer.fishingLevel.Value;
                farmer.fishingLevel.Value = newLevel;
                break;
            case Farmer.foragingSkill:
                oldLevel = farmer.foragingLevel.Value;
                farmer.foragingLevel.Value = newLevel;
                break;
            case Farmer.miningSkill:
                oldLevel = farmer.miningLevel.Value;
                farmer.miningLevel.Value = newLevel;
                break;
            case Farmer.combatSkill:
                oldLevel = farmer.combatLevel.Value;
                farmer.combatLevel.Value = newLevel;
                break;
            case Farmer.luckSkill:
                oldLevel = farmer.luckLevel.Value;
                farmer.luckLevel.Value = newLevel;
                break;
        }

        while (oldLevel <= newLevel)
            farmer.newLevels.Add(new(skill, oldLevel++));

        if (setExperience)
            farmer.experiencePoints[skill] = Experience.ExperienceByLevel[newLevel];

        if (skill == Skill.Combat) LevelUpMenu.RevalidateHealth(farmer);
    }

    /// <summary>Set the level of the specified custom skill for this farmer.</summary>
    /// <param name="skill">The <see cref="CustomSkill"/> whose level should be set.</param>
    /// <param name="newLevel">The new level.</param>
    /// <remarks>Will not change professions or recipes.</remarks>
    public static void SetCustomSkillLevel(this Farmer farmer, CustomSkill skill, int newLevel)
    {
        newLevel = Math.Min(newLevel, 10);
        var diff = Experience.ExperienceByLevel[newLevel] - skill.CurrentExp;
        ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(farmer, skill.StringId, diff);
    }

    /// <summary>Check if the farmer's skill levels match what is expected from their respective experience points, and if not fix the current level.</summary>
    public static void RevalidateLevels(this Farmer farmer)
    {
        foreach (var skill in Skill.List)
        {
            if (skill == Skill.Luck && ModEntry.LuckSkillApi is null)
            {
                Log.W(
                    $"Local player {Game1.player.Name} has gained Luck experience, but Luck Skill mod is not installed. The Luck skill will be reset.");
                Game1.player.ResetSkill(skill);
                continue;
            }

            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && farmer.HasAllProfessionsInSkill(skill);
            switch (skill.CurrentLevel)
            {
                case >= 10 when !canGainPrestigeLevels:
                    {
                        if (skill.CurrentLevel > 10) Game1.player.SetSkillLevel(skill, 10, true);
                        else if (skill.CurrentExp > Experience.VANILLA_CAP_I)
                            Game1.player.experiencePoints[skill] = Experience.VANILLA_CAP_I;
                        break;
                    }
                case >= 20 when canGainPrestigeLevels:
                    {
                        if (skill.CurrentLevel > 20) Game1.player.SetSkillLevel(skill, 20, true);
                        else if (skill.CurrentExp > Experience.PrestigeCap)
                            Game1.player.experiencePoints[skill] = Experience.PrestigeCap;
                        break;
                    }
                default:
                    {
                        var expectedLevel = 0;
                        var level = 1;
                        while (level <= 10 && skill.CurrentExp >= Experience.ExperienceByLevel[level++]) ++expectedLevel;

                        if (canGainPrestigeLevels && skill.CurrentExp - Experience.VANILLA_CAP_I > 0)
                            while (level <= 20 && skill.CurrentExp >= Experience.ExperienceByLevel[level++]) ++expectedLevel;

                        if (skill.CurrentLevel != expectedLevel)
                        {
                            if (skill.CurrentLevel < expectedLevel)
                                for (var levelup = skill.CurrentLevel + 1; levelup <= expectedLevel; ++levelup)
                                {
                                    var point = new Point(skill, levelup);
                                    if (!Game1.player.newLevels.Contains(point))
                                        Game1.player.newLevels.Add(point);
                                }

                            farmer.SetSkillLevel(skill, expectedLevel);
                        }

                        farmer.experiencePoints[skill] = skill.CurrentLevel switch
                        {
                            >= 10 when !canGainPrestigeLevels => Experience.VANILLA_CAP_I,
                            >= 20 when canGainPrestigeLevels => Experience.PrestigeCap,
                            _ => Game1.player.experiencePoints[skill]
                        };

                        break;
                    }
            }
        }
    }

    /// <summary>Remove all recipes associated with the specified skill from the farmer.</summary>
    /// <param name="skillType">The desired skill.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForSkill(this Farmer farmer, Skill skill, bool addToRecoveryDict = false)
    {
        var forgottenRecipesDict = farmer.ReadData(ModData.ForgottenRecipesDict).ParseDictionary<string, int>();

        // remove associated crafting recipes
        var craftingRecipes =
            farmer.craftingRecipes.Keys.ToDictionary(key => key,
                key => farmer.craftingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.craftingRecipes)
        {
            if (!value.Split('/')[4].Contains(skill.StringId) || !craftingRecipes.ContainsKey(key)) continue;

            if (addToRecoveryDict)
                if (!forgottenRecipesDict.TryAdd(key, craftingRecipes[key]))
                    forgottenRecipesDict[key] += craftingRecipes[key];
            
            farmer.craftingRecipes.Remove(key);
        }

        // remove associated cooking recipes
        var cookingRecipes =
            farmer.cookingRecipes.Keys.ToDictionary(key => key,
                key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.cookingRecipes)
        {
            if (!value.Split('/')[3].Contains(skill.StringId) || !cookingRecipes.ContainsKey(key)) continue;

            if (addToRecoveryDict)
            {
                if (!forgottenRecipesDict.TryAdd(key, cookingRecipes[key]))
                    forgottenRecipesDict[key] += cookingRecipes[key];
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (addToRecoveryDict)
            farmer.WriteData(ModData.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
    }

    /// <summary>Remove all recipes associated with the specified skill from the farmer.</summary>
    /// <param name="skillType">The desired skill.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForLoveOfCookingSkill(this Farmer farmer, bool addToRecoveryDict = false)
    {
        if (ModEntry.CookingSkillApi is null) return;

        var forgottenRecipesDict = farmer.ReadData(ModData.ForgottenRecipesDict).ParseDictionary<string, int>();

        // remove associated cooking recipes
        var cookingRecipes = ModEntry.CookingSkillApi.GetAllLevelUpRecipes().Values.SelectMany(r => r).ToList();
        var knownCookingRecipes = farmer.cookingRecipes.Keys.Where(key => key.IsIn(cookingRecipes)).ToDictionary(key => key,
                key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in knownCookingRecipes)
        {
            if (addToRecoveryDict && !forgottenRecipesDict.TryAdd(key, value))
                forgottenRecipesDict[key] += value;

            farmer.cookingRecipes.Remove(key);
        }

        if (addToRecoveryDict)
            farmer.WriteData(ModData.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
    }

    /// <summary>Get all available Ultimate's not currently registered.</summary>
    public static IEnumerable<UltimateIndex> GetUnchosenUltimates(this Farmer farmer)
    {
        return farmer.professions.Where(p => Enum.IsDefined(typeof(UltimateIndex), p)).Cast<UltimateIndex>()
            .Except(new[] {ModEntry.PlayerState.RegisteredUltimate.Index, UltimateIndex.None});
    }

    /// <summary>Whether the farmer has caught the specified fish at max size.</summary>
    /// <param name="index">The fish's index.</param>
    public static bool HasCaughtMaxSized(this Farmer farmer, int index)
    {
        if (!farmer.fishCaught.ContainsKey(index) || farmer.fishCaught[index][1] <= 0) return false;

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsIn(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        if (!fishData.TryGetValue(index, out var specificFishData)) return false;

        var dataFields = specificFishData.Split('/');
        return farmer.fishCaught[index][1] >= Convert.ToInt32(dataFields[4]);
    }

    /// <summary>The price bonus applied to animal produce sold by Producer.</summary>
    public static float GetProducerPriceBonus(this Farmer farmer)
    {
        return Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse) b.indoors.Value).isFull()).Sum(_ => 0.05f);
    }

    /// <summary>The bonus catching bar speed for prestiged Fisher.</summary>
    /// <remarks>UNUSED.</remarks>
    public static float GetFisherBonusCatchingBarSpeed(this Farmer farmer, int whichFish)
    {
        return farmer.fishCaught.TryGetValue(whichFish, out var caughtData)
            ? caughtData[0] >= ModEntry.Config.FishNeededForInstantCatch
                ? 1f
                : Math.Max(caughtData[0] * (0.1f / ModEntry.Config.FishNeededForInstantCatch) * 0.0002f, 0.002f)
            : 0.002f;
    }

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    public static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAlgae() && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        var bonus = 0f;
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (ObjectLookups.LegendaryFishNames.Contains(dataFields[0]))
                bonus += 0.05f;
            else if (value[1] >= Convert.ToInt32(dataFields[4]))
                bonus += 0.01f;
        }

        return Math.Min(bonus, ModEntry.Config.AnglerMultiplierCap);
    }

    /// <summary>The amount of "catching" bar to compensate for Aquarist.</summary>
    public static float GetAquaristCatchingBarCompensation(this Farmer farmer)
    {
        var fishTypes = Game1.getFarm().buildings
            .OfType<FishPond>()
            .Where(pond => (pond.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) && pond.fishType.Value > 0)
            .Select(pond => pond.fishType.Value);

        return Math.Min(fishTypes.Distinct().Count() * 0.000165f, 0.002f);
    }

    /// <summary>The price bonus applied to all items sold by Conservationist.</summary>
    public static float GetConservationistPriceMultiplier(this Farmer farmer)
    {
        return 1f + farmer.ReadDataAs<float>(ModData.ConservationistActiveTaxBonusPct);
    }

    /// <summary>The quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = farmer.ReadDataAs<uint>(ModData.EcologistItemsForaged);
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>The quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = farmer.ReadDataAs<uint>(ModData.GemologistMineralsCollected);
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Enumerate the Slimes currently inhabiting owned Slimes Hutches.</summary>
    public static IEnumerable<GreenSlime> GetRaisedSlimes(this Farmer farmer)
    {
        return Game1.getFarm().buildings
            .Where(b => (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        b.indoors.Value is SlimeHutch && !b.isUnderConstruction())
            .SelectMany(b => b.indoors.Value.characters.OfType<GreenSlime>());
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Farmer farmer, ModData field, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field, external to this mod, in the <see cref="ModDataDictionary" /> as <see cref="string"/>.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadDataExt(this Farmer farmer, string field, string modId, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T? ReadDataAs<T>(this Farmer farmer, ModData field, T? defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field, external to this mod, in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T? ReadDataExtAs<T>(this Farmer farmer, string field, string modId, T? defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Farmer farmer, ModData field, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}");
            return;
        }

        Game1.player.modData.Write($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {farmer.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteDataExt(this Farmer farmer, string field, string modId, string? value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}", modId);
            return;
        }

        Game1.player.modData.Write($"{modId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D(string.IsNullOrEmpty(value)
            ? $"[ModData]: Cleared {farmer.Name}'s {field}."
            : $"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Farmer farmer, ModData field, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}");
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataExtIfNotExists(this Farmer farmer, ModData field, string modId, string? value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{modId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Write/{field}", modId);
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Farmer farmer, ModData field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}");
            return;
        }

        var current = Game1.player.ReadData(field);
        if (current.Contains(value))
        {
            Log.D($"[ModData]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            Game1.player.WriteData(field, string.IsNullOrEmpty(current) ? value : current + separator + value);
            Log.D($"[ModData]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Append a string to an existing string field, external to this mod, in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void AppendDataExt(this Farmer farmer, string field, string value, string modId, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(value, $"RequestUpdateData/Append/{field}", modId);
            return;
        }

        var current = Game1.player.ReadDataExt(field, modId);
        if (current.Contains(value))
        {
            Log.D($"[ModData]: {farmer.Name}'s {field} already contained {value}.");
        }
        else
        {
            Game1.player.WriteDataExt(field, string.IsNullOrEmpty(current) ? value : current + separator + value, modId);
            Log.D($"[ModData]: Appended {farmer.Name}'s {field} with {value}");
        }
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Farmer farmer, ModData field, T amount)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(amount.ToString(), $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(this Farmer farmer, string field, T amount, string modId)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost(amount.ToString(), $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Farmer farmer, ModData field)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}");
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }

    /// <summary>Increment the value of a numeric field, external to this mod, in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    public static void IncrementDataExt<T>(this Farmer farmer, string field, string modId)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            ModEntry.Broadcaster.MessageHost("1", $"RequestUpdateData/Increment/{field}", modId);
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }
}