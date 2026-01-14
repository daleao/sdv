namespace DaLion.Professions.Commands;

#region using directives

using System;
using System.Text;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Constants;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SetCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class SetCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    private readonly HashSet<string> _dataKeys =
    [
        "forage",
        "itemsforaged",
        "varietiesforaged",
        "ecologist",
        "ecologistitemsforaged",
        "minerals",
        "mineralscollected",
        "mineralsstudied",
        "gemologist",
        "gemologistmineralscollected",
        "shunt",
        "scavengerhunt",
        "scavenger",
        "scavengerhuntstreak",
        "phunt",
        "prospectorhunt",
        "prospector",
        "prospectorhuntstreak",
        "trash",
        "trashcollected",
        "conservationist",
        "conservationisttrashcollectedthisseason",
    ];

    /// <inheritdoc />
    public override string[] Triggers { get; } = ["set", "write"];

    /// <inheritdoc />
    public override string Documentation => "Sets the specified data key or skill level to the specified value.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var tokens = args.ToList();
        var farmerIndex = 1;
        var farmerArgs = tokens.Where(a => a.ToLower() is "--farmer" or "-f").ToList();
        if (farmerArgs.Any())
        {
            var fIndex = tokens.IndexOf(farmerArgs.First());
            if (fIndex != -1 && tokens.Count > fIndex + 1 && int.TryParse(tokens[fIndex + 1], out var parsed))
            {
                farmerIndex = parsed;
            }
            else
            {
                Log.W("The `--farmer` flag is missing an accompanying farmer index. Please specify \"1\" for player 1 or \"2\" for player 2.");
                return false;
            }

            tokens.RemoveAt(fIndex + 1);
            tokens.RemoveAt(fIndex);
        }

        var player = Game1.player;
        if (farmerIndex > 1)
        {
            if (!Context.IsSplitScreen)
            {
                Log.W("Can't assign professions to co-op players in a non-splitscreen session.");
                return false;
            }

            var screenId = farmerIndex - 1;
            var onlinePlayers = ModHelper.Multiplayer.GetConnectedPlayers().ToList();
            if (screenId > onlinePlayers.Count)
            {
                Log.W($"Insufficient online players for setting specified player \"{farmerIndex}\".");
                return false;
            }

            var multiplayerId = onlinePlayers.Find(peer => peer.ScreenID == screenId)?.PlayerID;
            if (multiplayerId is null)
            {
                Log.W($"Couldn't find online player with the desired player screen ID \"{screenId}\".");
                return false;
            }

            player = Game1.GetPlayer(multiplayerId.Value, onlyOnline: true);
            if (player is null)
            {
                Log.W($"Couldn't find online player with specified player screen ID \"{screenId}\".");
                return false;
            }
        }

        if (args.Length < 2)
        {
            Log.W("You must specify a data key and value.");
            return false;
        }

        var key = tokens[0].ToLower();
        var value = tokens[1];
        if (this._dataKeys.Contains(key))
        {
            this.SetModData(key, value, player);
            return true;
        }

        int level;
        if (Skill.TryFromName(key, true, out var vanillaSkill))
        {
            if (int.TryParse(value, out level))
            {
                if (level > 10 && player.stats.Get(StatKeys.Mastery(vanillaSkill.Value)) == 0)
                {
                    Log.W($"The {vanillaSkill.Name} skill must be mastered before setting the level to {level}.");
                    return false;
                }

                var currentLevel = player.GetUnmodifiedSkillLevel(vanillaSkill.Value);
                for (var l = currentLevel + 1; l <= level; l++)
                {
                    var point = new Point(vanillaSkill.Value, l);
                    if (!player.newLevels.Contains(point))
                    {
                        player.newLevels.Add(point);
                    }
                }

                vanillaSkill
                    .When(Skill.Farming).Then(() => player.farmingLevel.Value = level)
                    .When(Skill.Fishing).Then(() => player.fishingLevel.Value = level)
                    .When(Skill.Foraging).Then(() => player.foragingLevel.Value = level)
                    .When(Skill.Mining).Then(() => player.miningLevel.Value = level)
                    .When(Skill.Combat).Then(() => player.combatLevel.Value = level);
                player.experiencePoints[vanillaSkill.Value] =
                    Math.Max(player.experiencePoints[vanillaSkill.Value], ISkill.ExperienceCurve[level]);
                return true;
            }

            switch (value)
            {
                case "master":
                case "mastered":
                    if (player.stats.Get(StatKeys.Mastery(vanillaSkill.Value)) != 0)
                    {
                        Log.I($"The {vanillaSkill.Name} skill is already mastered.");
                        return true;
                    }

                    player.stats.Set(StatKeys.Mastery(vanillaSkill), 1);
                    player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() + 1));
                    player.stats.Set(StatKeys.MasteryLevelsSpent, Game1.player.stats.Get(StatKeys.MasteryLevelsSpent) + 1);
                    Log.I($"Mastered the {vanillaSkill} skill.");
                    return true;
                case "unmaster":
                case "unmastered":
                case "brainfart":
                    if (player.stats.Get(StatKeys.Mastery(vanillaSkill.Value)) == 0)
                    {
                        Log.I($"The {vanillaSkill.Name} skill is already not mastered.");
                        return true;
                    }

                    player.stats.Set(StatKeys.Mastery(vanillaSkill), 0);
                    player.stats.Set(
                        StatKeys.MasteryExp,
                        Math.Max(MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() - 1), 0));
                    player.stats.Set(StatKeys.MasteryLevelsSpent, Math.Max(Game1.player.stats.Get(StatKeys.MasteryLevelsSpent) - 1, 0));
                    Log.I($"Unmastered the {vanillaSkill} skill.");
                    return true;
            }

            return false;
        }

        var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
            s.StringId.ToLower().Contains(key.ToLowerInvariant()) ||
            s.DisplayName.ToLower().Contains(key.ToLowerInvariant()));
        if (customSkill is not null)
        {
            if (!int.TryParse(value, out level))
            {
                Log.W($"Invalid parameter value {value}.");
                return false;
            }

            if (level > 10)
            {
                Log.W($"The {customSkill.StringId} cannot be set above level 10.");
                return false;
            }

            var diff = ISkill.ExperienceCurve[level] - SCSkills.GetExperienceFor(player, customSkill.StringId);
            SCSkills.AddExperience(player, customSkill.StringId, diff);
            return true;
        }

        switch (key)
        {
            case "limit":
                this.SetLimitBreak(value, player);
                break;

            case "fishingdex":
            case "fishdex":
                var trap = tokens.Any(arg => arg is "--trap" or "-t");
                var all = tokens.Any(arg => arg is "--all" or "-a");
                if (trap)
                {
                    this.SetFishPokedex(player, !all, trap);
                }
                else
                {
                    this.SetFishPokedex(player, !all);
                }

                break;

            case "rodmemory":
            case "rodmemo":
            case "rodmem":
                this.SetFishingRodMemory(value, player);
                break;
            case "maxtackleuses" when int.TryParse(value, out var maxTackleUses):
                FishingRod.maxTackleUses = maxTackleUses;
                break;
            case "animals":
            case "animal":
            case "anim":
                this.SetAnimalDispositions(value, player);
                break;
        }

        return true;
    }

    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <key> <value> [--<flag>]");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t<key> - A skill name to set the level of, or data key to set the value of.");
        sb.Append("\n\t<value> - The desired new level or value.");
        sb.Append("\n\t<flag> - Optional flags that can be passed onto specific commands, such as 'farmer' setting to a specific farmer in a local co-op session.");
        sb.Append("\n\nExamples:");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} farming 10 => sets the player's Farming skill level to 10");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} limit brute => sets the player's Limit Break to Brute's Frenzy");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} ecologist 30 => sets EcologistVarietiesForaged to the value 30");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} conservationist 100 => sets ConservationistTrashCollectedThisSeason to 100");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} fishdex caught => sets the record size of fish caught so far to the maximum value (for testing Angler profession)");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} fishdex all => sets the record size of all to the maximum value, even if not yet caught (for testing Angler profession)");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} rodmem 856 => sets the tackle memory of the currently held fishing rod to the value 856 (Curiosity Lure, for testing Angler profession)");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} anim friendship => sets the friendship of all owned animals to the maximum value (for testing Breeder profession)");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} anim mood => sets the mood of all owned animals to the maximum value (for testing Producer profession)");
        sb.Append(
            $"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} fishing 15 --farmer 2 => sets player 2's Fishing skill level to 15");
        sb.Append(this.GetAvailableKeys());
        return sb.ToString();
    }

    private void SetModData(string key, string value, Farmer who)
    {
        if (string.Equals(value, "clear", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(value, "reset", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(value, "null", StringComparison.InvariantCultureIgnoreCase))
        {
            value = string.Empty;
        }

        switch (key)
        {
            case "forage":
            case "itemsforaged":
            case "varietiesforaged":
            case "ecologist":
            case "ecologistitemsforaged":
                this.SetEcologistVarietiesForaged(value, who);
                break;

            case "minerals":
            case "mineralscollected":
            case "mineralsstudied":
            case "gemologist":
            case "gemologistmineralscollected":
                this.SetGemologistMineralsStudied(value, who);
                break;

            case "shunt":
            case "scavengerhunt":
            case "scavenger":
            case "scavengerhuntstreak":
                this.SetScavengerHuntStreak(value, who);
                break;

            case "phunt":
            case "prospectorhunt":
            case "prospector":
            case "prospectorhuntstreak":
                this.SetProspectorHuntStreak(value, who);
                break;

            case "trash":
            case "trashcollected":
            case "conservationist":
            case "conservationisttrashcollectedthisseason":
                this.SetConservationistTrashCollectedThisSeason(value, who);
                break;
        }
    }

    private void SetLimitBreak(string value, Farmer who)
    {
        if (string.Equals(value, "clear", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(value, "reset", StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(value, "null", StringComparison.InvariantCultureIgnoreCase))
        {
            State.LimitBreak = null;
            return;
        }

        LimitBreak limit;
        switch (value.ToLower())
        {
            case "brute":
            case "frenzy":
                limit = new BruteFrenzy();
                break;

            case "poacher":
            case "bushwhacker":
            case "bushwhack":
            case "bushwacker":
            case "bushwack":
            case "ambush":
            case "ambuscade":
                limit = new PoacherAmbush();
                break;

            case "desperado":
            case "blossom":
            case "deathblossom":
            case "deadlyblossom":
                limit = new DesperadoBlossom();
                break;

            case "piper":
            case "slimed":
            case "slime":
            case "concerto":
            case "pheronomes":
                limit = new PiperConcerto();
                break;

            default:
                Log.W($"{value} is not a valid Limit Break or combat profession.");
                return;
        }

        if (!who.HasProfession(limit.ParentProfession))
        {
            Log.W(
                "You don't have the required profession. Use the \"add\" command first if you would like to set this Limit Break.");
            return;
        }

        if (!who.IsLocalPlayer)
        {
            var screenId = who.GetScreenId(ModHelper.Multiplayer)!;
            PerScreenState.GetValueForScreen(screenId.Value).LimitBreak = limit;
        }
        else
        {
            State.LimitBreak = limit;
        }
    }

    private void SetFishPokedex(Farmer who, bool caughtOnly, bool trap = false)
    {
        var fishCaught = who.fishCaught;
        foreach (var (key, values) in DataLoader.Fish(Game1.content))
        {
            if (key.IsTrashId() || key.IsAlgaeId() || (values.Contains("trap") && !trap) ||
                (!values.Contains("trap") && trap) || (caughtOnly && !fishCaught.ContainsKey(key)))
            {
                continue;
            }

            var qid = "(O)" + key;
            var split = values.SplitWithoutAllocation('/');
            if (values.Contains("trap") && !fishCaught.TryAdd(qid, [1, int.Parse(split[6]) + 1, 1]))
            {
                var caught = fishCaught[qid];
                caught[1] = int.Parse(split[6]);
                fishCaught[qid] = caught;
            }
            else if (!values.Contains("trap") && !fishCaught.TryAdd(qid, [1, int.Parse(split[4]) + 1, 1]))
            {
                var caught = fishCaught[qid];
                caught[1] = int.Parse(split[4]) + 1;
                fishCaught[qid] = caught;
            }

            Game1.stats.checkForFishingAchievements();
        }

        Log.I($"{who.Name}'s FishingDex has been updated.");
    }

    private void SetAnimalDispositions(string value, Farmer who)
    {
        var both = string.Equals(value, "both", StringComparison.InvariantCultureIgnoreCase) || string.Equals(value, "all", StringComparison.InvariantCultureIgnoreCase);
        var count = 0;
        var animals = Game1.getFarm().getAllFarmAnimals();
        foreach (var animal in animals)
        {
            if (!animal.IsOwnedBy(who))
            {
                continue;
            }

            if (both)
            {
                animal.friendshipTowardFarmer.Value = 1000;
                animal.happiness.Value = 255;
            }
            else
            {
                switch (value)
                {
                    case "friendship" or "friendly" or "friend":
                        animal.friendshipTowardFarmer.Value = 1000;
                        break;
                    case "happiness" or "happy" or "mood":
                        animal.happiness.Value = byte.MaxValue;
                        break;
                }
            }

            count++;
        }

        if (count == 0)
        {
            Log.I("You don't own any animals.");
            return;
        }

        if (both)
        {
            Log.I($"The friendship and happiness of {count} animals has been set to max.");
            return;
        }

        switch (value)
        {
            case "friendship" or "friendly" or "friend":
                Log.I($"The friendship of {count} animals has been set to max.");
                break;
            case "happiness" or "happy" or "mood":
                Log.I($"The happiness of {count} animals has been set to max.");
                break;
        }
    }

    private void SetEcologistVarietiesForaged(string value, Farmer who)
    {
        if (!who.HasProfession(Profession.Ecologist))
        {
            Log.W($"The player {who.Name} must have the Ecologist profession.");
            return;
        }

        var parsed = 0;
        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out parsed))
        {
            Log.W($"{value} is not a valid integer value.");
            return;
        }

        for (var i = 0; i < parsed; i++)
        {
            Data.AppendToEcologistItemsForaged(i.ToString(), who);
        }

        Log.I($"Added {value} varieties foraged as Ecologist.");
    }

    private void SetGemologistMineralsStudied(string value, Farmer who)
    {
        if (!who.HasProfession(Profession.Gemologist))
        {
            Log.W($"The player {who.Name} must have the Gemologist profession.");
            return;
        }

        var parsed = 0;
        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out parsed))
        {
            Log.W($"{value} is not a valid integer value.");
            return;
        }

        for (var i = 0; i < parsed; i++)
        {
            Data.AppendToGemologistMineralsCollected(i.ToString(), who);
        }

        Log.I($"Added {value} minerals collected as Gemologist.");
    }

    private void SetProspectorHuntStreak(string value, Farmer who)
    {
        if (!who.HasProfession(Profession.Prospector))
        {
            Log.W($"The player {who.Name} must have the Prospector profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(who, DataKeys.LongestProspectorHuntStreak, value);
        Log.I($"Prospector Hunt was streak set to {value}.");
    }

    private void SetScavengerHuntStreak(string value, Farmer who)
    {
        if (!who.HasProfession(Profession.Scavenger))
        {
            Log.W($"The player {who.Name} must have the Scavenger profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(who, DataKeys.LongestScavengerHuntStreak, value);
        Log.I($"Scavenger Hunt streak was set to {value}.");
    }

    private void SetConservationistTrashCollectedThisSeason(string value, Farmer who)
    {
        if (!who.HasProfession(Profession.Conservationist))
        {
            Log.W($"The player {who.Name} must have the Conservationist profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(who, DataKeys.ConservationistTrashCollectedThisSeason, value);
        Log.I(
            $"Conservationist trash collected in the current season ({Game1.CurrentSeasonDisplayName}) was set to {value}.");
    }

    private void SetFishingRodMemory(string value, Farmer who)
    {
        if (who.CurrentTool is not FishingRod { UpgradeLevel: > 2 } rod)
        {
            Log.W("You must equip an Iridium Rod to use this command.");
            return;
        }

        if (value is not ("686" or "687" or "691" or "692" or "693" or "694" or "695" or "856" or "877" or "SonarBobber"))
        {
            Log.W($"{value} is not a valid tackle ID.");
            return;
        }

        if (!string.IsNullOrEmpty(Data.Read(rod, DataKeys.FirstMemorizedTackle)))
        {
            if (rod.AttachmentSlotsCount > 2)
            {
                Data.Write(rod, DataKeys.SecondMemorizedTackle, value);
                Data.Write(rod, DataKeys.SecondMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
            }
            else
            {
                Data.Write(rod, DataKeys.FirstMemorizedTackle, value);
                Data.Write(rod, DataKeys.FirstMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
            }
        }
        else
        {
            Data.Write(rod, DataKeys.FirstMemorizedTackle, value);
            Data.Write(rod, DataKeys.FirstMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
        }
    }

    private string GetAvailableKeys()
    {
        var sb = new StringBuilder("\n\nAvailable data fields:");
        sb.Append("\n\t- EcologistVarietiesForaged (shortcuts: 'forages', 'ecologist')");
        sb.Append("\n\t- GemologistMineralsStudied (shortcuts: 'minerals', 'gemologist')");
        sb.Append("\n\t- ProspectorHuntStreak (shortcuts: 'prospector', 'phunt')");
        sb.Append("\n\t- ScavengerHuntStreak (shortcuts: 'scavenger', 'shunt')");
        sb.Append("\n\t- ConservationistTrashCollectedThisSeason (shortcuts: 'conservationist', 'trash')");
        sb.Append("\n\t- FishingDex (shortcuts: 'fishdex')");
        sb.Append("\n\t- RodMemory (shortcuts: 'rodmem')");
        sb.Append("\n\t- Animals (shortcuts: 'anim')");
        return sb.ToString();
    }
}
