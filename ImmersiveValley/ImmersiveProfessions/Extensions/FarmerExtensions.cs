// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Collections;
using Common.ModData;
using Framework;
using Framework.Ultimates;
using Framework.Utility;
using Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    public static bool HasProfession(this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        if (prestiged && !profession.Id.IsIn(Profession.GetRange())) return false;
        return farmer.professions.Contains(profession.Id + (prestiged ? 100 : 0));
    }

    /// <summary>Whether the farmer has acquired both level ten professions branching from the specified level five profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    public static bool HasAllProfessionsBranchingFrom(this Farmer farmer, IProfession profession) =>
        profession.BranchingProfessions.All(farmer.professions.Contains);

    /// <summary>Whether the farmer has all six professions in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    public static bool HasAllProfessionsInSkill(this Farmer farmer, ISkill skill) =>
        skill.ProfessionIds.All(farmer.professions.Contains);

    /// <summary>Whether the farmer has all available professions (vanilla + modded).</summary>
    public static bool HasAllProfessions(this Farmer farmer) =>
        Enumerable.Range(0, 30).Concat(ModEntry.CustomProfessions.Values.Select(p => p.Id))
            .All(farmer.professions.Contains);

    /// <summary>Get the last 1st-tier profession acquired by the farmer in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentBranchForSkill(this Farmer farmer, ISkill skill) =>
        farmer.professions.Where(pid => pid.IsIn(skill.TierOneProfessionIds)).DefaultIfEmpty(-1).Last();

    /// <summary>Get the last level 2nd-tier profession acquired by the farmer in the specified skill branch.</summary>
    /// <param name="branch">The branch (level 5 <see cref="IProfession"/>) to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    public static int GetCurrentProfessionForBranch(this Farmer farmer, IProfession branch) =>
        farmer.professions.Where(pid => pid.IsIn(branch.BranchingProfessions)).DefaultIfEmpty(-1).Last();

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

    /// <summary>Get the professions which the player is missing in the specified skill.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    public static IEnumerable<IProfession> GetMissingProfessionsInSkill(this Farmer farmer, ISkill skill,
        bool excludeTierOneProfessions = false) =>
        excludeTierOneProfessions
            ? skill.Professions.Where(p => p.Level == 10 && !farmer.professions.Contains(p.Id))
            : skill.Professions.Where(p => !farmer.professions.Contains(p.Id));

    /// <summary>Get the last acquired profession by the farmer in the specified subset, or simply the last acquired profession if no subset is specified.</summary>
    /// <param name="subset">An array of profession ids.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetMostRecentProfession(this Farmer farmer, IEnumerable<int>? subset = null) =>
        subset is null
            ? farmer.professions[^1]
            : farmer.professions.Where(p => p.IsIn(subset)).DefaultIfEmpty(-1).Last();

    /// <summary>Whether the farmer can reset the specified skill for prestige.</summary>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    public static bool CanResetSkill(this Farmer farmer, ISkill skill)
    {
        if (skill is Skill vanillaSkill && vanillaSkill == Farmer.luckSkill && ModEntry.LuckSkillApi is null) return false;

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
            farmer.GetProfessionsForSkill(skill, true).Count() is > 0 and < 4;
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

    /// <summary>Whether the farmer can reset any skill for prestige.</summary>
    public static bool CanResetAnySkill(this Farmer farmer) =>
        Skill.List.Any(farmer.CanResetSkill) || ModEntry.CustomSkills.Values.Any(farmer.CanResetSkill);

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

        return (int)(baseCost * multiplier);
    }

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
        if (skill == Farmer.combatSkill) LevelUpMenu.RevalidateHealth(farmer);

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
    public static void SetSkillLevel(this Farmer farmer, Skill skill, int newLevel)
    {
        switch (skill)
        {
            case Farmer.farmingSkill:
                farmer.farmingLevel.Value = newLevel;
                break;
            case Farmer.fishingSkill:
                farmer.fishingLevel.Value = newLevel;
                break;
            case Farmer.foragingSkill:
                farmer.foragingLevel.Value = newLevel;
                break;
            case Farmer.miningSkill:
                farmer.miningLevel.Value = newLevel;
                break;
            case Farmer.combatSkill:
                farmer.combatLevel.Value = newLevel;
                break;
            case Farmer.luckSkill:
                farmer.luckLevel.Value = newLevel;
                break;
        }
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
            if (skill == Farmer.luckSkill && skill.CurrentExp > 0 && ModEntry.LuckSkillApi is null)
            {
                Log.W(
                    $"Local player {Game1.player.Name} has gained Luck experience, but Luck Skill mod is not installed. The Luck skill will be reset.");
                Game1.player.ResetSkill(skill);
                continue;
            }

            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && farmer.HasAllProfessionsInSkill(skill) && skill != Farmer.luckSkill;
            switch (skill.CurrentLevel)
            {
                case >= 10 when !canGainPrestigeLevels:
                    {
                        if (skill.CurrentLevel > 10) Game1.player.SetSkillLevel(skill, 10);
                        if (skill.CurrentExp > Experience.VANILLA_CAP_I)
                            Game1.player.experiencePoints[skill] = Experience.VANILLA_CAP_I;
                        break;
                    }
                case >= 20 when canGainPrestigeLevels:
                    {
                        if (skill.CurrentLevel > 20) Game1.player.SetSkillLevel(skill, 20);
                        if (skill.CurrentExp > Experience.PrestigeCap)
                            Game1.player.experiencePoints[skill] = Experience.PrestigeCap;
                        break;
                    }
                default:
                    {
                        var expectedLevel = 0;
                        var level = 1;
                        while (level <= 10 && skill.CurrentExp >= Experience.ExperienceByLevel[level++]) ++expectedLevel;

                        if (canGainPrestigeLevels && skill.CurrentExp - Experience.VANILLA_CAP_I > 0)
                            while (level <= 20 && skill.CurrentExp >= Experience.ExperienceByLevel[level++])
                                ++expectedLevel;

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
        var forgottenRecipesDict = ModDataIO.Read(farmer, "ForgottenRecipesDict")
            .ParseDictionary<string, int>();

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
            ModDataIO.Write(farmer, "ForgottenRecipesDict", forgottenRecipesDict.Stringify());
    }

    /// <summary>Remove all recipes associated with the specified skill from the farmer.</summary>
    /// <param name="skillType">The desired skill.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForLoveOfCookingSkill(this Farmer farmer, bool addToRecoveryDict = false)
    {
        var forgottenRecipesDict = ModDataIO.Read(farmer, "ForgottenRecipesDict")
            .ParseDictionary<string, int>();

        // remove associated cooking recipes
        var cookingRecipes = ModEntry.CookingSkillApi!
            .GetAllLevelUpRecipes().Values
            .SelectMany(r => r)
            .Select(r => "blueberry.cac." + r)
            .ToList();
        var knownCookingRecipes = farmer.cookingRecipes.Keys.Where(key => key.IsIn(cookingRecipes)).ToDictionary(
            key => key,
            key => farmer.cookingRecipes[key]);
        foreach (var (key, value) in knownCookingRecipes)
        {
            if (addToRecoveryDict && !forgottenRecipesDict.TryAdd(key, value))
                forgottenRecipesDict[key] += value;

            farmer.cookingRecipes.Remove(key);
        }


        if (addToRecoveryDict)
            ModDataIO.Write(farmer, "ForgottenRecipesDict", forgottenRecipesDict.Stringify());
    }

    /// <summary>Get all available Ultimate's not currently registered.</summary>
    public static IEnumerable<UltimateIndex> GetUnchosenUltimates(this Farmer farmer) =>
        farmer.professions.Where(p => Enum.IsDefined(typeof(UltimateIndex), p)).Cast<UltimateIndex>()
            .Except(new[] { Game1.player.get_Ultimate()!.Index, UltimateIndex.None });

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
    public static float GetProducerPriceBonus(this Farmer farmer) =>
        Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse)b.indoors.Value).isFull()).Sum(_ => 0.05f);

    /// <summary>The bonus catching bar speed for prestiged Fisher.</summary>
    /// <remarks>UNUSED.</remarks>
    public static float GetFisherBonusCatchingBarSpeed(this Farmer farmer, int whichFish) =>
        farmer.fishCaught.TryGetValue(whichFish, out var caughtData)
            ? caughtData[0] >= ModEntry.Config.FishNeededForInstantCatch
                ? 1f
                : Math.Max(caughtData[0] * (0.1f / ModEntry.Config.FishNeededForInstantCatch) * 0.0002f, 0.002f)
            : 0.002f;

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    public static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAlgaeIndex() && !p.Value.Contains("trap"))
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
            .Where(pond => (pond.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                           pond.fishType.Value > 0)
            .Select(pond => pond.fishType.Value);

        return Math.Min(fishTypes.Distinct().Count() * 0.000165f, 0.002f);
    }

    /// <summary>The price bonus applied to all items sold by Conservationist.</summary>
    public static float GetConservationistPriceMultiplier(this Farmer farmer) =>
        1f + ModDataIO.Read<float>(farmer, "ConservationistActiveTaxBonusPct");

    /// <summary>The quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = ModDataIO.Read<uint>(farmer, "EcologistItemsForaged");
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>The quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = ModDataIO.Read<uint>(farmer, "GemologistMineralsCollected");
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Enumerate the Slimes currently inhabiting owned Slimes Hutches.</summary>
    public static IEnumerable<GreenSlime> GetRaisedSlimes(this Farmer farmer) =>
        Game1.getFarm().buildings
            .Where(b => (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        b.indoors.Value is SlimeHutch && !b.isUnderConstruction())
            .SelectMany(b => b.indoors.Value.characters.OfType<GreenSlime>());

    /// <summary>Whether this farmer is currently using the Poacher Ultimate.</summary>
    public static bool IsInAmbush(this Farmer farmer) =>
        farmer.get_UltimateIndex() == (int)UltimateIndex.PoacherAmbush && farmer.get_IsUltimateActive().Value;
}