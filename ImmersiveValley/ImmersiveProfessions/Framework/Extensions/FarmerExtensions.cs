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
using StardewValley.Monsters;

using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Stardew;
using Events.GameLoop;
using Ultimate;
using Utility;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="profession">The index of the profession.</param>
    public static bool HasProfession(this Farmer farmer, Profession profession, bool prestiged = false)
    {
        return farmer.professions.Contains((int) profession + (prestiged ? 100 : 0));
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
            ? farmer.professions.Count(p => p >= 0 && p / 6 == which && p % 6 > 1)
            : farmer.professions.Count(p => p >= 0 && p / 6 == which);
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
                farmer.farmingLevel.Value = 0;
                break;
            case SkillType.Fishing:
                farmer.fishingLevel.Value = 0;
                break;
            case SkillType.Foraging:
                farmer.foragingLevel.Value = 0;
                break;
            case SkillType.Mining:
                farmer.miningLevel.Value = 0;
                break;
            case SkillType.Combat:
                farmer.combatLevel.Value = 0;
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
            farmer.ForgetRecipesForSkill(skillType, true);

        // revalidate health
        if (skillType == SkillType.Combat) LevelUpMenu.RevalidateHealth(farmer);
    }

    /// <summary>Set the level of a specific skill for this farmer.</summary>
    /// <param name="skillType">The desired skill.</param>
    /// <param name="newLevel">The new level.</param>
    public static void SetSkillLevel(this Farmer farmer, SkillType skillType, int newLevel)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (skillType)
        {
            case SkillType.Farming:
                farmer.farmingLevel.Value = newLevel;
                break;
            case SkillType.Fishing:
                farmer.fishingLevel.Value = newLevel;
                break;
            case SkillType.Foraging:
                farmer.foragingLevel.Value = newLevel;
                break;
            case SkillType.Mining:
                farmer.miningLevel.Value = newLevel;
                break;
            case SkillType.Combat:
                farmer.combatLevel.Value = newLevel;
                break;
            case SkillType.Luck:
                farmer.luckLevel.Value = newLevel;
                break;
            default:
                return;
        }
    }

    /// <summary>Remove all recipes associated with the specified skill from the farmer.</summary>
    /// <param name="skillType">The desired skill.</param>
    /// <param name="addToRecoveryDict">Whether to store crafted quantities for later recovery.</param>
    public static void ForgetRecipesForSkill(this Farmer farmer, SkillType skillType, bool addToRecoveryDict)
    {
        var forgottenRecipesDict = farmer.ReadData(DataField.ForgottenRecipesDict).ParseDictionary<string, int>();

        // remove associated crafting recipes
        var craftingRecipes =
            farmer.craftingRecipes.Keys.ToDictionary(key => key,
                key => farmer.craftingRecipes[key]);
        foreach (var (key, value) in CraftingRecipe.craftingRecipes)
        {
            if (!value.Split('/')[4].Contains(skillType.ToString()) || !craftingRecipes.ContainsKey(key)) continue;

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
            if (!value.Split('/')[3].Contains(skillType.ToString()) || !cookingRecipes.ContainsKey(key)) continue;

            if (addToRecoveryDict)
            {
                if (!forgottenRecipesDict.TryAdd(key, cookingRecipes[key]))
                    forgottenRecipesDict[key] += cookingRecipes[key];
            }

            farmer.cookingRecipes.Remove(key);
        }

        if (addToRecoveryDict)
            farmer.WriteData(DataField.ForgottenRecipesDict, forgottenRecipesDict.Stringify());
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
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
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

        return Math.Min(bonus, ModEntry.Config.AnglerMultiplierCeiling);
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
        return 1f + farmer.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
    }

    /// <summary>The quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = farmer.ReadDataAs<uint>(DataField.EcologistItemsForaged);
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>The quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = farmer.ReadDataAs<uint>(DataField.GemologistMineralsCollected);
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
    public static string ReadData(this Farmer farmer, DataField field, string defaultValue = "")
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
    public static T ReadDataAs<T>(this Farmer farmer, DataField field, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read from a field, external to this mod, in the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataExtAs<T>(this Farmer farmer, string field, string modId, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Farmer farmer, DataField field, string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Write($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteDataExt(this Farmer farmer, string field, string modId, string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Write($"{modId}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Farmer farmer, DataField field, string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID },
                new[] { Game1.MasterPlayer.UniqueMultiplayerID }); // request the main player
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Write to a field, external to this mod, in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="modId">The unique id of the external mod.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataExtIfNotExists(this Farmer farmer, DataField field, string modId, string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{modId}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { modId },
                new[] { Game1.MasterPlayer.UniqueMultiplayerID }); // request the main player
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Append a string to an existing string field in the <see cref="ModDataDictionary"/>, or initialize to the given value.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Farmer farmer, DataField field, string value, string separator = ",")
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Append/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
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
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Append/{field}",
                new[] { modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        var current = Game1.player.ReadData(field);
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
    public static void IncrementData<T>(this Farmer farmer, DataField field, T amount)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(amount, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
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
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(amount, $"RequestUpdateData/Increment/{field}",
                new[] { modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Farmer farmer, DataField field)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(1, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
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
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(1, $"RequestUpdateData/Increment/{field}",
                new[] { modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{modId}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }
}