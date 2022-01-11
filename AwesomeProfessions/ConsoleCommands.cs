using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.SuperMode;
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
            PrintFishingAudit);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_data",
            "Check the current value of all mod data fields." + GetUsageForSetModData(),
            PrintModData);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_setdata", "Set a new value for a mod data field.",
            SetModData);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_events", "List currently subscribed mod events.",
            PrintSubscribedEvents);
        ModEntry.ModHelper.ConsoleCommands.Add("wol_resetthehunt",
            "Forcefully reset the current Treasure Hunt with a new target treasure tile.",
            RerollTreasureTile);
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
        ModEntry.Log($"Foraging level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Foraging)}",
            LogLevel.Info);
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

        var message = $"Farmer {Game1.player.Name}'s professions:";
        foreach (var professionsIndex in Game1.player.professions)
            try
            {
                message += "\n\t- " +
                    (professionsIndex < 100
                        ? $"{Framework.Utility.Professions.NameOf(professionsIndex)}"
                        : $"{Framework.Utility.Professions.NameOf(professionsIndex - 100)} (P)");
            }
            catch (IndexOutOfRangeException)
            {
                ModEntry.Log($"Unknown profession index {professionsIndex}", LogLevel.Info);
            }

        ModEntry.Log(message, LogLevel.Info);
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
        foreach (var arg in args.Select(a => a.ToLower()))
        {
            if (arg == "all")
            {
                var range = Enumerable.Range(0, 30).ToHashSet();
                if (prestige) range = range.Concat(Enumerable.Range(100, 30)).ToHashSet();

                professionsToAdd.AddRange(range);
                ModEntry.Log($"Added all {(prestige ? "prestiged " : "")}professions to farmer {Game1.player.Name}.",
                    LogLevel.Info);
                break;
            }

            var professionName = arg.FirstCharToUpper();
            if (Framework.Utility.Professions.IndexByName.Forward.TryGetValue(professionName, out var professionIndex))
            {
                if (!prestige && Game1.player.HasProfession(professionName) ||
                    prestige && Game1.player.HasPrestigedProfession(professionName))
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

        ModEntry.State.Value.SuperMode = null;
        ModData.Write(DataField.SuperModeIndex, null);
        for (var i = Game1.player.professions.Count - 1; i >= 0; --i)
        {
            var professionIndex = Game1.player.professions[i];
            Game1.player.professions.RemoveAt(i);
            LevelUpMenu.removeImmediateProfessionPerk(professionIndex);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    /// <summary>Set <see cref="ModEntry.State.Value.SuperModeGaugeValue" /> to the max value.</summary>
    internal static void SetSuperModeGaugeValue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (ModEntry.State.Value.SuperMode is null)
        {
            ModEntry.Log("Not registered to any Super Mode.", LogLevel.Warn);
            return;
        }

        if (!args.Any() || args.Length > 1)
        {
            ModEntry.Log("You must specify a single value.", LogLevel.Warn);
            return;
        }

        if (double.TryParse(args[0], out var value))
            ModEntry.State.Value.SuperMode.Gauge.CurrentValue = Math.Min(value, SuperModeGauge.MaxValue);
        else
            ModEntry.Log("You must specify an integer value.", LogLevel.Warn);
    }

    /// <summary>Set <see cref="ModEntry.State.Value.SuperModeGaugeValue" /> to the desired value.</summary>
    internal static void MaxSuperModeGaugeValue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (ModEntry.State.Value.SuperMode is null)
        {
            ModEntry.Log("Not registered to any Super Mode.", LogLevel.Warn);
            return;
        }

        ModEntry.State.Value.SuperMode.Gauge.CurrentValue = SuperModeGauge.MaxValue;
    }

    /// <summary>
    ///     Reset the Super Mode instance to a different combat profession's, in case you have more
    ///     than one.
    /// </summary>
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

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            ModEntry.Log("You don't have any 2nd-tier combat professions.", LogLevel.Warn);
            return;
        }

        args[0] = args[0].ToLower().FirstCharToUpper();
        if (!Enum.TryParse<SuperModeIndex>(args[0], out var index))
        {
            ModEntry.Log("You must enter a valid 2nd-tier combat profession.", LogLevel.Warn);
            return;
        }

        if (!Game1.player.HasProfession(args[0]))
        {
            ModEntry.Log("You don't have this profession.", LogLevel.Warn);
            return;
        }

        ModEntry.State.Value.SuperMode = new(index);
        ModData.Write(DataField.SuperModeIndex, index.ToString());
    }

    /// <summary>Print the currently registered Super Mode profession.</summary>
    internal static void PrintSuperModeIndex(string command, string[] args)
    {
        if (ModEntry.State.Value.SuperMode is null)
        {
            ModEntry.Log("Not registered to any Super Mode.", LogLevel.Info);
            return;
        }

        var key = ModEntry.State.Value.SuperMode.Index;
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
    internal static void PrintFishingAudit(string command, string[] args)
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
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
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
                $"\n\t- {fish} (current: {nonMaxSizedCaught[fish].Item1}, max: {nonMaxSizedCaught[fish].Item2})");

        var seasonFish = from specificFishData in fishData.Values
            where specificFishData.Split('/')[6].Contains(Game1.currentSeason)
            select specificFishData.Split('/')[0];

        result += "\n\nThe following fish can be caught this season:";
        result = seasonFish.Except(caughtFishNames).Aggregate(result, (current, fish) => current + $"\n\t- {fish}");

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

        var message = $"Farmer {Game1.player.Name}'s mod data:";
        var value = ModData.Read(DataField.EcologistItemsForaged);
        message += "\n\t- " +
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.EcologistItemsForaged}: {value} ({ModEntry.Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                : $"Mod data does not contain an entry for {DataField.EcologistItemsForaged}.");

        value = ModData.Read(DataField.GemologistMineralsCollected);
        message += "\n\t- " +
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.GemologistMineralsCollected}: {value} ({ModEntry.Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                : $"Mod data does not contain an entry for {DataField.GemologistMineralsCollected}.");

        value = ModData.Read(DataField.ProspectorHuntStreak);
        message += "\n\t- " +
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.ProspectorHuntStreak}: {value} (affects treasure quality)"
                : $"Mod data does not contain an entry for {DataField.ProspectorHuntStreak}.");

        value = ModData.Read(DataField.ScavengerHuntStreak);
        message += "\n\t- " +
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.ScavengerHuntStreak}: {value} (affects treasure quality)"
                : $"Mod data does not contain an entry for {DataField.ScavengerHuntStreak}.");

        value = ModData.Read(DataField.ConservationistTrashCollectedThisSeason);
        message += "\n\t- " +
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.ConservationistTrashCollectedThisSeason}: {value} (expect a {int.Parse(value) / ModEntry.Config.TrashNeededPerTaxLevel}% tax deduction next season)"
                : $"Mod data does not contain an entry for {DataField.ConservationistTrashCollectedThisSeason}.");

        value = ModData.Read(DataField.ConservationistActiveTaxBonusPct);
        message += "\n\t- " + 
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.ConservationistActiveTaxBonusPct}: {float.Parse(value) * 100}%"
                : $"Mod data does not contain an entry for {DataField.ConservationistActiveTaxBonusPct}.");

        ModEntry.Log(message, LogLevel.Info);
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

        switch (args[0].ToLower())
        {
            case "forages":
            case "ecologistitemsforaged":
                SetEcologistItemsForaged(value);
                break;

            case "minerals":
            case "gemologistmineralscollected":
                SetGemologistMineralsCollected(value);
                break;

            case "shunt":
            case "scavengerhuntstreak":
                SetScavengerHuntStreak(value);
                break;

            case "phunt":
            case "prospectorhuntstreak":
                SetProspectorHuntStreak(value);
                break;

            case "trash":
            case "conservationisttrashcollectedthisseason":
                SetConservationistTrashCollectedThisSeason(value);
                break;

            default:
                var message = $"'{args[0]}' is not a settable data field.\n" + GetAvailableDataFields();
                ModEntry.Log(message, LogLevel.Warn);
                break;
        }
    }

    /// <summary>Print the currently subscribed mod events to the console.</summary>
    internal static void PrintSubscribedEvents(string command, string[] args)
    {
        var message = "Enabled events:";
        message = ModEntry.EventManager.GetAllEnabled()
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        ModEntry.Log(message, LogLevel.Info);
    }

    /// <summary>Force a new treasure tile to be selected for the currently active Treasure Hunt.</summary>
    internal static void RerollTreasureTile(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.Log("You must load a save first.", LogLevel.Warn);
            return;
        }

        if (!ModEntry.State.Value.ScavengerHunt.IsActive && !ModEntry.State.Value.ProspectorHunt.IsActive)
        {
            ModEntry.Log("There is no Treasure Hunt currently active.", LogLevel.Warn);
            return;
        }

        if (ModEntry.State.Value.ScavengerHunt.IsActive)
        {
            var v = ModEntry.State.Value.ScavengerHunt.ChooseTreasureTile(Game1.currentLocation);
            if (v is null)
            {
                ModEntry.Log("Couldn't find a valid treasure tile after 10 tries.", LogLevel.Warn);
                return;
            }

            Game1.currentLocation.MakeTileDiggable(v.Value);
            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.Value.ScavengerHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<uint>(ModEntry.State.Value.ScavengerHunt, "Elapsed").SetValue(0);

            ModEntry.Log("The Scavenger Hunt was reset.", LogLevel.Info);
        }
        else if (ModEntry.State.Value.ProspectorHunt.IsActive)
        {
            var v = ModEntry.State.Value.ProspectorHunt.ChooseTreasureTile(Game1.currentLocation);
            if (v is null)
            {
                ModEntry.Log("Couldn't find a valid treasure tile after 10 tries.", LogLevel.Warn);
                return;
            }

            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.Value.ProspectorHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<int>(ModEntry.State.Value.ProspectorHunt, "Elapsed").SetValue(0);

            ModEntry.Log("The Prospector Hunt was reset.", LogLevel.Info);
        }
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
        var result = "\n\nUsage: wol_setdata <field> <value>";
        result += "\n\nExamples:";
        result += "\n\twol_setdata EcologistItemsForaged 100";
        result += "\n\twol_setdata trash 500";
        result += "\n\n" + GetAvailableDataFields();
        return result;
    }

    /// <summary>Tell the dummies which fields they can set.</summary>
    private static string GetAvailableDataFields()
    {
        var result = "Available data fields:";
        result += $"\n\t- {DataField.EcologistItemsForaged} (shortcut 'forages')";
        result += $"\n\t- {DataField.GemologistMineralsCollected} (shortcut 'minerals')";
        result += $"\n\t- {DataField.ProspectorHuntStreak} (shortcut 'phunt')";
        result += $"\n\t- {DataField.ScavengerHuntStreak} (shortcut 'shunt')";
        result += $"\n\t- {DataField.ConservationistTrashCollectedThisSeason} (shortcut 'trash')";
        return result;
    }

    /// <summary>Set a new value to the EcologistItemsForaged data field.</summary>
    internal static void SetEcologistItemsForaged(int value)
    {
        if (!Game1.player.HasProfession("Ecologist"))
        {
            ModEntry.Log("You must have the Ecologist profession.", LogLevel.Warn);
            return;
        }

        ModData.Write(DataField.EcologistItemsForaged, value.ToString());
        ModEntry.Log($"Items foraged as Ecologist was set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the GemologistMineralsCollected data field.</summary>
    internal static void SetGemologistMineralsCollected(int value)
    {
        if (!Game1.player.HasProfession("Gemologist"))
        {
            ModEntry.Log("You must have the Gemologist profession.", LogLevel.Warn);
            return;
        }

        ModData.Write(DataField.GemologistMineralsCollected, value.ToString());
        ModEntry.Log($"Minerals collected as Gemologist was set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the ProspectorHuntStreak data field.</summary>
    internal static void SetProspectorHuntStreak(int value)
    {
        if (!Game1.player.HasProfession("Prospector"))
        {
            ModEntry.Log("You must have the Prospector profession.", LogLevel.Warn);
            return;
        }

        ModData.Write(DataField.ProspectorHuntStreak, value.ToString());
        ModEntry.Log($"Prospector Hunt was streak set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the ScavengerHuntStreak data field.</summary>
    internal static void SetScavengerHuntStreak(int value)
    {
        if (!Game1.player.HasProfession("Scavenger"))
        {
            ModEntry.Log("You must have the Scavenger profession.", LogLevel.Warn);
            return;
        }

        ModData.Write(DataField.ScavengerHuntStreak, value.ToString());
        ModEntry.Log($"Scavenger Hunt streak was set to {value}.", LogLevel.Info);
    }

    /// <summary>Set a new value to the ConservationistTrashCollectedThisSeason data field.</summary>
    internal static void SetConservationistTrashCollectedThisSeason(int value)
    {
        if (!Game1.player.HasProfession("Conservationist"))
        {
            ModEntry.Log("You must have the Conservationist profession.", LogLevel.Warn);
            return;
        }

        ModData.Write(DataField.ConservationistTrashCollectedThisSeason, value.ToString());
        ModEntry.Log($"Conservationist trash collected in the current season was set to {value}.", LogLevel.Info);
    }

    #endregion private methods
}