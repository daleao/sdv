namespace DaLion.Stardew.Professions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

using Common.Extensions;
using Framework;
using Framework.Extensions;
using Framework.SuperMode;
using Framework.Utility;

#endregion using directives

internal static class ConsoleCommands
{
    /// <summary>Register all internally defined console commands.</summary>
    /// <param name="helper">The console command API.</param>
    internal static void Register(ICommandHelper helper)
    {
        helper.Add("player_skills", "List the player's current skill levels.",
            PrintLocalPlayerSkillLevels);
        helper.Add("player_resetskills", "Reset all player's skills.",
            ResetLocalPlayerSkills);
        helper.Add("player_professions", "List the player's current professions.",
            PrintLocalPlayerProfessions);
        helper.Add("player_addprofessions",
            "Add the specified professions to the local player, without affecting skill levels." +
            GetUsageForAddProfessions(),
            AddProfessionsToLocalPlayer);
        helper.Add("player_resetprofessions",
            "Reset all skills and professions for the local player.",
            ResetLocalPlayerProfessions);
        helper.Add("player_readyult", "Max-out the Super Mode meter, or set it to the specified percentage.",
            SetSuperModeGaugeValue);
        helper.Add("player_changeult",
            "Change the currently registered Super Mode profession.",
            SetSuperModeIndex);
        helper.Add("player_whichult",
            "Check the currently registered Super Mode profession.",
            PrintSuperModeIndex);
        helper.Add("player_maxanimalfriendship",
            "Max-out the friendship of all owned animals.",
            MaxAnimalFriendship);
        helper.Add("player_maxanimalmood", "Max-out the mood of all owned animals.",
            MaxAnimalMood);
        helper.Add("player_fishingprogress",
            "Check your fishing progress as Angler.",
            PrintFishingAudit);
        helper.Add("wol_data",
            "Check the current value of all mod data fields." + GetUsageForSetModData(),
            PrintModData);
        helper.Add("wol_setdata", "Set a new value for a mod data field.",
            SetModData);
        helper.Add("wol_events", "List currently subscribed mod events.",
            PrintSubscribedEvents);
        helper.Add("wol_resetthehunt",
            "Forcefully reset the current Treasure Hunt with a new target treasure tile.",
            RerollTreasureTile);
    }

    #region command handlers

    /// <summary>List the current skill levels of the local player.</summary>
    internal static void PrintLocalPlayerSkillLevels(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        Log.I($"Farming level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Farming)}");
        Log.I($"Fishing level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Fishing)}");
        Log.I($"Foraging level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Foraging)}");
        Log.I($"Mining level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Mining)}");
        Log.I($"Combat level: {Game1.player.GetUnmodifiedSkillLevel((int) SkillType.Combat)}");
    }

    /// <summary>Reset all skills for the local player.</summary>
    internal static void ResetLocalPlayerSkills(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
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
            Log.W("You must load a save first.");
            return;
        }

        var message = $"Farmer {Game1.player.Name}'s professions:";
        foreach (var professionsIndex in Game1.player.professions)
            try
            {
                message += "\n\t- " +
                    (professionsIndex < 100
                        ? $"{professionsIndex.ToProfessionName()}"
                        : $"{(professionsIndex - 100).ToProfessionName()} (P)");
            }
            catch (IndexOutOfRangeException)
            {
                Log.I($"Unknown profession index {professionsIndex}");
            }

        Log.I(message);
    }

    /// <summary>Add specified professions to the local player.</summary>
    internal static void AddProfessionsToLocalPlayer(string command, string[] args)
    {
        if (!args.Any())
        {
            Log.W("You must specify at least one profession." + GetUsageForAddProfessions());
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
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
                Log.I($"Added all {(prestige ? "prestiged " : "")}professions to farmer {Game1.player.Name}.");
                break;
            }

            var professionName = arg.FirstCharToUpper();
            if (Enum.TryParse<Profession>(professionName, out var profession))
            {
                if (!prestige && Game1.player.HasProfession(profession) ||
                    prestige && Game1.player.HasProfession(profession, true))
                {
                    Log.W("You already have this profession.");
                    continue;
                }

                professionsToAdd.Add((int) profession);
                if (prestige) professionsToAdd.Add((int) profession + 100);
                Log.I(
                    $"Added {profession.ToString()}{(prestige ? " (P)" : "")} profession to farmer {Game1.player.Name}.");
            }
            else
            {
                Log.W($"Ignoring unknown profession {arg}.");
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
            Log.W("You must load a save first.");
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

    /// <summary>Set <see cref="ModEntry.State.Value.SuperModeGaugeValue" /> to the desired value, or max it out if no value is specified.</summary>
    internal static void SetSuperModeGaugeValue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (ModEntry.State.Value.SuperMode is null)
        {
            Log.W("Not registered to any Super Mode.");
            return;
        }

        if (!args.Any())
        {
            ModEntry.State.Value.SuperMode.Gauge.CurrentValue = SuperModeGauge.MaxValue;
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Too many arguments. Specify a single value between 0 and 100.");
            return;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            Log.W("Bad arguments. Specify an integer value between 0 and 100.");
            return;
        }

        ModEntry.State.Value.SuperMode.Gauge.CurrentValue = SuperModeGauge.MaxValue * (double)value / 100;
    }

    /// <summary>
    ///     Reset the Super Mode instance to a different combat profession's, in case you have more
    ///     than one.
    /// </summary>
    internal static void SetSuperModeIndex(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (!args.Any() || args.Length > 1)
        {
            Log.W("You must specify a single value.");
            return;
        }

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        args[0] = args[0].ToLower().FirstCharToUpper();
        if (!Enum.TryParse<SuperModeIndex>(args[0], out var index))
        {
            Log.W("You must enter a valid 2nd-tier combat profession.");
            return;
        }

        if (!Game1.player.HasProfession((Profession) index))
        {
            Log.W("You don't have this profession.");
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
            Log.I("Not registered to any Super Mode.");
            return;
        }

        var key = ModEntry.State.Value.SuperMode.Index;
        var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
        var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
        Log.I($"Registered to {professionDisplayName}'s {buffName}.");
    }

    /// <summary>Set all farm animals owned by the local player to the max friendship value.</summary>
    internal static void MaxAnimalFriendship(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;
        if (count <= 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals) animal.friendshipTowardFarmer.Value = 1000;
        Log.I($"Maxed the friendship of {count} animals");
    }

    /// <summary>Set all farm animals owned by the local player to the max mood value.</summary>
    internal static void MaxAnimalMood(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;

        if (count <= 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals) animal.happiness.Value = 255;
        Log.I($"Maxed the mood of {count} animals");
    }

    /// <summary>Check current fishing progress.</summary>
    internal static void PrintFishingAudit(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (!Game1.player.fishCaught.Pairs.Any())
        {
            Log.W("You haven't caught any fish.");
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
        foreach (var (key, value) in Game1.player.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (ObjectLookups.LegendaryFishNames.Contains(dataFields[0]))
            {
                ++numLegendariesCaught;
            }
            else
            {
                if (value[1] >= Convert.ToInt32(dataFields[4]))
                    ++numMaxSizedCaught;
                else
                    nonMaxSizedCaught.Add(dataFields[0],
                        new(value[1], Convert.ToInt32(dataFields[4])));
            }

            caughtFishNames.Add(dataFields[0]);
        }

        var priceMultiplier = Game1.player.HasProfession(Profession.Angler)
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

        Log.I(result);
    }

    /// <summary>Print the current value of every mod data field to the console.</summary>
    internal static void PrintModData(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
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
                ? $"{DataField.ConservationistTrashCollectedThisSeason}: {value} (expect a {Math.Min(int.Parse(value) / ModEntry.Config.TrashNeededPerTaxLevel, (int) (ModEntry.Config.TaxDeductionCeiling * 100))}% tax deduction next season)"
                : $"Mod data does not contain an entry for {DataField.ConservationistTrashCollectedThisSeason}.");

        value = ModData.Read(DataField.ConservationistActiveTaxBonusPct);
        message += "\n\t- " + 
            (!string.IsNullOrEmpty(value)
                ? $"{DataField.ConservationistActiveTaxBonusPct}: {float.Parse(value) * 100}%"
                : $"Mod data does not contain an entry for {DataField.ConservationistActiveTaxBonusPct}.");

        Log.I(message);
    }

    /// <summary>Set a new value to the specified mod data field.</summary>
    internal static void SetModData(string command, string[] args)
    {
        if (!args.Any() || args.Length != 2)
        {
            Log.W("You must specify a data field and value." + GetUsageForSetModData());
            return;
        }

        if (!int.TryParse(args[1], out var value) || value < 0)
        {
            Log.W("You must specify a positive integer value.");
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
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
                Log.W(message);
                break;
        }
    }

    /// <summary>Print the currently subscribed mod events to the console.</summary>
    internal static void PrintSubscribedEvents(string command, string[] args)
    {
        var message = "Enabled events:";
        message = EventManager.GetAllEnabled()
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }

    /// <summary>Force a new treasure tile to be selected for the currently active Treasure Hunt.</summary>
    internal static void RerollTreasureTile(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (!ModEntry.State.Value.ScavengerHunt.IsActive && !ModEntry.State.Value.ProspectorHunt.IsActive)
        {
            Log.W("There is no Treasure Hunt currently active.");
            return;
        }

        if (ModEntry.State.Value.ScavengerHunt.IsActive)
        {
            var v = ModEntry.State.Value.ScavengerHunt.ChooseTreasureTile(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            Game1.currentLocation.MakeTileDiggable(v.Value);
            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.Value.ScavengerHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<uint>(ModEntry.State.Value.ScavengerHunt, "elapsed").SetValue(0);

            Log.I("The Scavenger Hunt was reset.");
        }
        else if (ModEntry.State.Value.ProspectorHunt.IsActive)
        {
            var v = ModEntry.State.Value.ProspectorHunt.ChooseTreasureTile(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.Value.ProspectorHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<int>(ModEntry.State.Value.ProspectorHunt, "Elapsed").SetValue(0);

            Log.I("The Prospector Hunt was reset.");
        }
    }

    #endregion command handlers

    #region private methods

    /// <summary>Tell the dummies how to use the console command.</summary>
    private static string GetUsageForAddProfessions()
    {
        var result = "\n\nUsage: player_addprofessions [--prestige] <profession1> <profession2> ... <professionN>";
        result += "\n\nParameters:";
        result += "\n\t<profession>\t- one of the modded profession names, or 'all'";
        result += "\n\nOptional flags:";
        result +=
            "\n\t--prestige, -p\t- add the prestiged versions of the specified professions (will automatically add the base versions as well)";
        result += "\n\nExamples:";
        result += "\n\tplayer_addprofessions artisan brute";
        result += "\n\tplayer_addprofessions -p all";
        return result;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private static string GetUsageForSetModData()
    {
        var result = "\n\nUsage: wol_setdata <field> <value>";
        result += "\n\nParameters:";
        result += "\n\t<field>\t- the name of the field";
        result += "\\n\t<value>\t- the desired new value";
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
    private static void SetEcologistItemsForaged(int value)
    {
        if (!Game1.player.HasProfession(Profession.Ecologist))
        {
            Log.W("You must have the Ecologist profession.");
            return;
        }

        ModData.Write(DataField.EcologistItemsForaged, value.ToString());
        Log.I($"Items foraged as Ecologist was set to {value}.");
    }

    /// <summary>Set a new value to the GemologistMineralsCollected data field.</summary>
    private static void SetGemologistMineralsCollected(int value)
    {
        if (!Game1.player.HasProfession(Profession.Gemologist))
        {
            Log.W("You must have the Gemologist profession.");
            return;
        }

        ModData.Write(DataField.GemologistMineralsCollected, value.ToString());
        Log.I($"Minerals collected as Gemologist was set to {value}.");
    }

    /// <summary>Set a new value to the ProspectorHuntStreak data field.</summary>
    private static void SetProspectorHuntStreak(int value)
    {
        if (!Game1.player.HasProfession(Profession.Prospector))
        {
            Log.W("You must have the Prospector profession.");
            return;
        }

        ModData.Write(DataField.ProspectorHuntStreak, value.ToString());
        Log.I($"Prospector Hunt was streak set to {value}.");
    }

    /// <summary>Set a new value to the ScavengerHuntStreak data field.</summary>
    private static void SetScavengerHuntStreak(int value)
    {
        if (!Game1.player.HasProfession(Profession.Scavenger))
        {
            Log.W("You must have the Scavenger profession.");
            return;
        }

        ModData.Write(DataField.ScavengerHuntStreak, value.ToString());
        Log.I($"Scavenger Hunt streak was set to {value}.");
    }

    /// <summary>Set a new value to the ConservationistTrashCollectedThisSeason data field.</summary>
    private static void SetConservationistTrashCollectedThisSeason(int value)
    {
        if (!Game1.player.HasProfession(Profession.Conservationist))
        {
            Log.W("You must have the Conservationist profession.");
            return;
        }

        ModData.Write(DataField.ConservationistTrashCollectedThisSeason, value.ToString());
        Log.I($"Conservationist trash collected in the current season was set to {value}.");
    }

    #endregion private methods
}