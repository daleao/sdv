using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions;

internal static class ConsoleCommands
{
    internal static void Register()
    {
        ModEntry.ModHelper.ConsoleCommands.Add("player_skills", "List the player's current skill levels.",
            PrintLocalPlayerSkillLevels);
        ModEntry.ModHelper.ConsoleCommands.Add("player_resetskills", "Reset all player's skills.",
            ResetLocalPlayerSkills);
        ModEntry.ModHelper.ConsoleCommands.Add("player_professions", "List the player's current professions.",
            PrintLocalPlayerProfessions);
        ModEntry.ModHelper.ConsoleCommands.Add("player_addprofessions",
            "Add the specified professions to the local player, without affecting skill levels." +
            GetUsageForAddProfessions(),
            AddProfessionsToLocalPlayer);
        ModEntry.ModHelper.ConsoleCommands.Add("player_resetprofessions",
            "Reset all skills and professions for the local player.",
            ResetLocalPlayerProfessions);
        ModEntry.ModHelper.ConsoleCommands.Add("player_setultvalue",
            "Set the Super Mode meter to the desired value.",
            SetSuperModeGaugeValue);
        ModEntry.ModHelper.ConsoleCommands.Add("player_readyult", "Max-out the Super Mode meter.",
            MaxSuperModeGaugeValue);
        ModEntry.ModHelper.ConsoleCommands.Add("player_chooseult",
            "Change the currently registered Super Mode profession.",
            SetSuperModeIndex);
        ModEntry.ModHelper.ConsoleCommands.Add("player_whichult",
            "Check the currently registered Super Mode profession.",
            PrintSuperModeIndex);
        ModEntry.ModHelper.ConsoleCommands.Add("player_maxanimalfriendship",
            "Max-out the friendship of all owned animals.",
            MaxAnimalFriendship);
        ModEntry.ModHelper.ConsoleCommands.Add("player_maxanimalmood", "Max-out the mood of all owned animals.",
            MaxAnimalMood);
        ModEntry.ModHelper.ConsoleCommands.Add("player_fishingprogress",
            "Check your fishing progress as Angler.",
            PrintFishCaughtAudit);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_data",
            "Check the current value of all mod data fields." + GetUsageForSetModData(),
            PrintModData);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_setdata", "Set a new value for a mod data field.",
            SetModData);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_events", "List currently subscribed mod events.",
            PrintSubscribedEvents);
    }

    #region command handlers

    /// <summary>List the current skill levels of the local player.</summary>
    internal static void PrintLocalPlayerSkillLevels(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        ModEntry.Log($"Farming level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Farming)}", LogLevel.Info);
        ModEntry.Log($"Fishing level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Fishing)}", LogLevel.Info);
        ModEntry.Log($"Foraging level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Foraging)}", LogLevel.Info);
        ModEntry.Log($"Mining level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Mining)}", LogLevel.Info);
        ModEntry.Log($"Combat level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Combat)}", LogLevel.Info);
    }

    /// <summary>Reset all skills for the local player.</summary>
    internal static void ResetLocalPlayerSkills(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        Game1.player.FarmingLevel = 0;
        Game1.player.FishingLevel = 0;
        Game1.player.ForagingLevel = 0;
        Game1.player.MiningLevel = 0;
        Game1.player.CombatLevel = 0;
        for (var i = 0; i < 5; ++i) Game1.player.experiencePoints[i] = 0;

        Game1.player.craftingRecipes.Clear();
        Game1.player.cookingRecipes.Clear();
        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    /// <summary>List the current professions of the local player.</summary>
    internal static void PrintLocalPlayerProfessions(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        foreach (var professionsIndex in Game1.player.professions)
            try
            {
                ModEntry.Log(
                    professionsIndex < 100
                        ? $"{Framework.Utility.Professions.NameOf(professionsIndex)}"
                        : $"{Framework.Utility.Professions.NameOf(professionsIndex - 100)} (P)", LogLevel.Info);
            }
            catch (IndexOutOfRangeException)
            {
                ModEntry.Log($"Unknown profession index {professionsIndex}", LogLevel.Info);
            }
    }

    /// <summary>Add specified professions to the local player.</summary>
    internal static void AddProfessionsToLocalPlayer(string command, string[] args)
    {
        if (!args.Any())
        {
            ModEntry.Log("You must specify at least one profession." + GetUsageForAddProfessions(), LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        var prestige = args[0] is "-p" or "--prestiged";
        if (prestige) args = args.Skip(1).ToArray();

        List<int> professionsToAdd = new();
        foreach (var arg in args)
        {
            if (arg == "all")
            {
                var range = Enumerable.Range(0, 30).ToHashSet();
                if (prestige) range = range.Concat(Enumerable.Range(100, 30)).ToHashSet();

                professionsToAdd.AddRange(range);
                ModEntry.Log($"Added all {(prestige ? "prestiged " : "")}professions to farmer {Game1.player.Name}.", LogLevel.Info);
                break;
            }

            var professionName = arg.FirstCharToUpper();
            if (Framework.Utility.Professions.IndexByName.Forward.TryGetValue(professionName, out var professionIndex))
            {
                if (!prestige && Game1.player.HasProfession(professionName) || prestige && Game1.player.HasPrestigedProfession(professionName))
                {
                    ModEntry.Log("You already have this profession.", LogLevel.Warn);
                    continue;
                }

                professionsToAdd.Add(professionIndex);
                if (prestige) professionsToAdd.Add(100 + professionIndex);
                ModEntry.Log(
                    $"Added {Framework.Utility.Professions.NameOf(professionIndex)}{(prestige ? " (P)" : "")} profession to farmer {Game1.player.Name}.",
                    LogLevel.Info);
            }
            else
            {
                ModEntry.Log($"Ignoring unknown profession {arg}.", LogLevel.Warn);
            }
        }

        LevelUpMenu levelUpMenu = new();
        foreach (var professionIndex in professionsToAdd.Distinct().Except(Game1.player.professions))
        {
            Game1.player.professions.Add(professionIndex);
            levelUpMenu.getImmediateProfessionPerk(professionIndex);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    /// <summary>Remove all professions from the local player.</summary>
    internal static void ResetLocalPlayerProfessions(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        ModState.SuperModeIndex = -1;
        var professionsToRemove = Game1.player.professions.ToList();
        foreach (var professionIndex in professionsToRemove)
        {
            Game1.player.professions.Remove(professionIndex);
            LevelUpMenu.removeImmediateProfessionPerk(professionIndex);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    /// <summary>Set <see cref="ModState.SuperModeGaugeValue" /> to the max value.</summary>
    internal static void SetSuperModeGaugeValue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (ModState.SuperModeIndex < 0)
        {
            ModEntry.Log("You must have a level 10 combat profession.", LogLevel.Warn);
            return;
        }

        if (!args.Any() || args.Length > 1)
        {
            ModEntry.Log("You must specify a single value.", LogLevel.Warn);
            return;
        }

        if (int.TryParse(args[0], out var value))
            ModState.SuperModeGaugeValue = Math.Min(value, ModState.SuperModeGaugeMaxValue);
        else
            ModEntry.Log("You must specify an integer value.", LogLevel.Warn);
    }

    /// <summary>Set <see cref="ModState.SuperModeGaugeValue" /> to the desired value.</summary>
    internal static void MaxSuperModeGaugeValue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (ModState.SuperModeIndex < 0)
        {
            ModEntry.Log("You must have a level 10 combat profession.", LogLevel.Warn);
            return;
        }

        // first raise above zero, then fill, otherwise fill event won't trigger
        ModState.SuperModeGaugeValue = 1;
        ModState.SuperModeGaugeValue = ModState.SuperModeGaugeMaxValue;
    }

    /// <summary>Set <see cref="ModState.SuperModeIndex" /> to a different combat profession, in case you have more than one.</summary>
    internal static void SetSuperModeIndex(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (!args.Any() || args.Length > 1)
        {
            ModEntry.Log("You must specify a single value.", LogLevel.Warn);
            return;
        }

        if (!args[0].IsAnyOf("brute", "poacher", "desperado", "piper"))
        {
            ModEntry.Log("You must specify a valid level 10 combat profession.", LogLevel.Warn);
            return;
        }

        if (!Game1.player.HasProfession(args[0].FirstCharToUpper()))
        {
            ModEntry.Log("You don't have this profession.", LogLevel.Warn);
            return;
        }

        ModState.SuperModeIndex = Framework.Utility.Professions.IndexOf(args[0].FirstCharToUpper());
    }

    /// <summary>Print the currently registered Super Mode profession.</summary>
    internal static void PrintSuperModeIndex(string command, string[] args)
    {
        if (ModState.SuperModeIndex < 0)
        {
            ModEntry.Log("Not registered to any Super Mode.", LogLevel.Info);
            return;
        }

        var key = Framework.Utility.Professions.NameOf(ModState.SuperModeIndex).ToLower();
        var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
        var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
        ModEntry.Log($"Registered to {professionDisplayName}'s {buffName}.", LogLevel.Info);
    }

    /// <summary>Set all farm animals owned by the local player to the max friendship value.</summary>
    internal static void MaxAnimalFriendship(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;
        if (count <= 0)
        {
            ModEntry.Log("You don't own any animals.", LogLevel.Warn);
            return;
        }

        foreach (var animal in animals) animal.friendshipTowardFarmer.Value = 1000;
        ModEntry.Log($"Maxed the friendship of {count} animals", LogLevel.Info);
    }

    /// <summary>Set all farm animals owned by the local player to the max mood value.</summary>
    internal static void MaxAnimalMood(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;

        if (count <= 0)
        {
            ModEntry.Log("You don't own any animals.", LogLevel.Warn);
            return;
        }

        foreach (var animal in animals) animal.happiness.Value = 255;
        ModEntry.Log($"Maxed the mood of {count} animals", LogLevel.Info);
    }

    /// <summary>Check current fishing progress.</summary>
    internal static void PrintFishCaughtAudit(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (!Game1.player.fishCaught.Pairs.Any())
        {
            ModEntry.Log("You haven't caught any fish.", LogLevel.Warn);
            return;
        }

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("ModEntry.Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 152, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);
        int numLegendariesCaught = 0, numMaxSizedCaught = 0;
        var caughtFishNames = new List<string>();
        var nonMaxSizedCaught = new Dictionary<string, Tuple<int, int>>();
        var result = string.Empty;
        foreach (var p in Game1.player.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(p.Key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (Objects.LegendaryFishNames.Contains(dataFields[0]))
            {
                ++numLegendariesCaught;
            }
            else
            {
                if (p.Value[1] >= Convert.ToInt32(dataFields[4]))
                    ++numMaxSizedCaught;
                else
                    nonMaxSizedCaught.Add(dataFields[0],
                        new(p.Value[1], Convert.ToInt32(dataFields[4])));
            }

            caughtFishNames.Add(dataFields[0]);
        }

        var priceMultiplier = Game1.player.HasProfession("Angler")
            ? (numMaxSizedCaught + numMaxSizedCaught * 5).ToString() + '%'
            : "Zero. You're not an Angler.";
        result +=
            $"Species caught: {Game1.player.fishCaught.Count()}/{fishData.Count}\nMax-sized: {numMaxSizedCaught}/{Game1.player.fishCaught.Count()}\nLegendaries: {numLegendariesCaught}/10\nTotal Angler price bonus: {priceMultiplier}\n\nThe following caught fish are not max-sized:";
        result = nonMaxSizedCaught.Keys.Aggregate(result,
            (current, fish) =>
                current +
                $"\n- {fish} (current: {nonMaxSizedCaught[fish].Item1}, max: {nonMaxSizedCaught[fish].Item2})");

        var seasonFish = from specificFishData in fishData.Values
            where specificFishData.Split('/')[6].Contains(Game1.currentSeason)
            select specificFishData.Split('/')[0];

        result += "\n\nThe following fish can be caught this season:";
        result = seasonFish.Except(caughtFishNames).Aggregate(result, (current, fish) => current + $"\n- {fish}");

        ModEntry.Log(result, LogLevel.Info);
    }

    /// <summary>Print the current value of every mod data field to the console.</summary>
    internal static void PrintModData(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        var fields = new[]
        {
            "ItemsForaged", "MineralsCollected", "ProspectorStreak", "ScavengerStreak",
            "WaterTrashCollectedThisSeason", "ActiveTaxBonusPercent"
        };
        foreach (var field in fields)
        {
            var value = ModEntry.Data.Read($"{field}");
            if (field == "ActiveTaxBonusPercent" && float.TryParse(value, out var pct))
                value = (pct * 100).ToString(CultureInfo.InvariantCulture) + '%';

            ModEntry.Log(
                !string.IsNullOrEmpty(value)
                    ? $"{field}: {value}"
                    : $"Mod data does not contain an entry for {field}.", LogLevel.Info);
        }
    }

    /// <summary>Set a new value to the specified mod data field.</summary>
    internal static void SetModData(string command, string[] args)
    {
        if (!args.Any() || args.Length != 2)
        {
            ModEntry.Log("You must specify a data field and value." + GetUsageForSetModData(), LogLevel.Warn);
            return;
        }

        if (!int.TryParse(args[1], out var value) || value < 0)
        {
            ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
            return;
        }

        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        switch (args[0])
        {
            case "itemsforaged":
                SetItemsForaged(value);
                break;

            case "mineralscollected":
                SetMineralsCollected(value);
                break;

            case "scavengerstreak":
                SetScavengerStreak(value);
                break;

            case "prospectorstreak":
                SetProspectorStreak(value);
                break;

            case "watertrashcollectedthisseason":
                SetWaterTrashCollectedThisSeason(value);
                break;

            default:
                ModEntry.Log($"'{args[0]}' is not a settable data field.", LogLevel.Warn);
                break;
        }
    }

    /// <summary>Print the currently subscribed mod events to the console.</summary>
    internal static void PrintSubscribedEvents(string command, string[] args)
    {
        ModEntry.Log("Currently subscribed events:", LogLevel.Info);
        foreach (var eventName in ModEntry.Subscriber.Select(e => e.GetType().Name))
            ModEntry.Log($"{eventName}", LogLevel.Info);
    }

    #endregion command handlers

    #region private methods

    /// <summary>Tell the dummies how to use the console command.</summary>
    private static string GetUsageForAddProfessions()
    {
        var result = "\n\nUsage: player_addprofessions <argument1> <argument2> ... <argumentN>";
        result += "\nAvailable arguments:";
        result +=
            "\n\t'<profession>' - get the specified profession.";
        result += "\n\t'all' - get all professions.";
        result += "\n\nExample:";
        result += "\n\tplayer_addprofessions artisan brute";
        return result;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private static string GetUsageForSetModData()
    {
        var result = "\n\nUsage: wol_setdata <name> <value>";
        result += "\n\nExample:";
        result += "\n\twol_setdata itemsforaged 100";
        return result;
    }

    /// <summary>Set a new value to the ItemsForaged data field.</summary>
    internal static void SetItemsForaged(int value)
    {
        if (!Game1.player.HasProfession("Ecologist"))
        {
            ModEntry.Log("You must have the Ecologist profession.", LogLevel.Warn);
            return;
        }

        ModEntry.Data.Write("ItemsForaged", value.ToString());
        ModEntry.Log($"ItemsForaged set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the MineralsCollected data field.</summary>
    internal static void SetMineralsCollected(int value)
    {
        if (!Game1.player.HasProfession("Gemologist"))
        {
            ModEntry.Log("You must have the Gemologist profession.", LogLevel.Warn);
            return;
        }

        ModEntry.Data.Write("MineralsCollected", value.ToString());
        ModEntry.Log($"MineralsCollected set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the ProspectorStreak data field.</summary>
    internal static void SetProspectorStreak(int value)
    {
        if (!Game1.player.HasProfession("Prospector"))
        {
            ModEntry.Log("You must have the Prospector profession.", LogLevel.Warn);
            return;
        }

        ModEntry.Data.Write("ProspectorStreak", value.ToString());
        ModEntry.Log($"ProspectorStreak set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the ScavengerStreak data field.</summary>
    internal static void SetScavengerStreak(int value)
    {
        if (!Game1.player.HasProfession("Scavenger"))
        {
            ModEntry.Log("You must have the Scavenger profession.", LogLevel.Warn);
            return;
        }

        ModEntry.Data.Write("ScavengerStreak", value.ToString());
        ModEntry.Log($"ScavengerStreak set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the WaterTrashCollectedThisSeason data field.</summary>
    internal static void SetWaterTrashCollectedThisSeason(int value)
    {
        if (!Game1.player.HasProfession("Conservationist"))
        {
            ModEntry.Log("You must have the Conservationist profession.", LogLevel.Warn);
            return;
        }

        ModEntry.Data.Write("WaterTrashCollectedThisSeason", value.ToString());
        ModEntry.Log($"WaterTrashCollectedThisSeason set to {value}.", LogLevel.Info);
    }

    #endregion private methods
}