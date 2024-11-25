namespace DaLion.Professions.Commands;

#region using directives

using System;
using System.Text;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
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
        if (args.Length < 2)
        {
            this.Handler.Log.W("You must specify a data key and value.");
            return false;
        }

        var key = args[0].ToLower();
        var value = args[1];
        if (this._dataKeys.Contains(key))
        {
            this.SetModData(key, value);
            return true;
        }

        int level;
        if (Skill.TryFromName(key, true, out var vanillaSkill))
        {
            if (int.TryParse(value, out level))
            {
                vanillaSkill.SetLevel(level);
                return true;
            }

            switch (value)
            {
                case "mastered":
                    if (vanillaSkill.CanGainPrestigeLevels())
                    {
                        return true;
                    }

                    Game1.player.stats.Set(StatKeys.Mastery(vanillaSkill), 1);
                    Game1.player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() + 1));
                    this.Handler.Log.I($"Mastered the {vanillaSkill} skill.");
                    return true;
                case "unmastered":
                case "brainfart":
                    if (!vanillaSkill.CanGainPrestigeLevels())
                    {
                        return true;
                    }

                    Game1.player.stats.Set(StatKeys.Mastery(vanillaSkill), 0);
                    Game1.player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() - 1));
                    this.Handler.Log.I($"Unmastered the {vanillaSkill} skill.");
                    return true;
            }

            return false;
        }

        var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
            s.StringId.ToLower().Contains(key.ToLowerInvariant()) ||
            s.DisplayName.ToLower().Contains(key.ToLowerInvariant()));
        if (customSkill is not null)
        {
            if (int.TryParse(value, out level))
            {
                customSkill.SetLevel(level);
                return true;
            }

            return false;
        }

        switch (key)
        {
            case "limit":
                this.SetLimitBreak(value);
                break;

            case "fishingdex":
            case "fishdex":
                if (args.Length > 2 && args.Any(arg => arg is "-t" or "--trap"))
                {
                    this.SetFishPokedex(value, true);
                }
                else
                {
                    this.SetFishPokedex(value);
                }

                break;

            case "rodmemory":
            case "rodmemo":
            case "rodmem":
                this.SetFishingRodMemory(value);
                break;

            case "animals":
            case "anim":
                this.SetAnimalDispositions(value);
                break;
        }

        return true;
    }

    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <key> <value>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t<key> - A skill name to set the level of, or data key to set the value of.");
        sb.Append("\n\t<value> - The desired new level or value.");
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
        sb.Append(this.GetAvailableKeys());
        return sb.ToString();
    }

    private void SetModData(string key, string value)
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
                this.SetEcologistVarietiesForaged(value);
                break;

            case "minerals":
            case "mineralscollected":
            case "mineralsstudied":
            case "gemologist":
            case "gemologistmineralscollected":
                this.SetGemologistMineralsStudied(value);
                break;

            case "shunt":
            case "scavengerhunt":
            case "scavenger":
            case "scavengerhuntstreak":
                this.SetScavengerHuntStreak(value);
                break;

            case "phunt":
            case "prospectorhunt":
            case "prospector":
            case "prospectorhuntstreak":
                this.SetProspectorHuntStreak(value);
                break;

            case "trash":
            case "trashcollected":
            case "conservationist":
            case "conservationisttrashcollectedthisseason":
                this.SetConservationistTrashCollectedThisSeason(value);
                break;
        }
    }

    private void SetLimitBreak(string value)
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

        if (!Game1.player.HasProfession(limit.ParentProfession))
        {
            this.Handler.Log.W(
                "You don't have the required profession. Use the \"add\" command first if you would like to set this Limit Break.");
            return;
        }

        State.LimitBreak = limit;
    }

    private void SetFishPokedex(string value, bool trap = false)
    {
        var caughtOnly = string.Equals(value, "caught", StringComparison.InvariantCultureIgnoreCase);
        var fishCaught = Game1.player.fishCaught;
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
                caught[1] = int.Parse(split[6]) + 1;
                fishCaught[qid] = caught;
            }
            else if (!fishCaught.TryAdd(qid, [1, int.Parse(split[4]) + 1, 1]))
            {
                var caught = fishCaught[qid];
                caught[1] = int.Parse(split[4]) + 1;
                fishCaught[qid] = caught;
            }

            Game1.stats.checkForFishingAchievements();
        }

        this.Handler.Log.I($"{Game1.player.Name}'s FishingDex has been updated.");
    }

    private void SetAnimalDispositions(string value)
    {
        var both = string.Equals(value, "both", StringComparison.InvariantCultureIgnoreCase);
        var count = 0;
        var animals = Game1.getFarm().getAllFarmAnimals();
        foreach (var animal in animals)
        {
            if (!animal.IsOwnedBy(Game1.player))
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
                    case "friendship" or "friendly":
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
            this.Handler.Log.I("You don't own any animals.");
            return;
        }

        if (both)
        {
            this.Handler.Log.I($"The friendship and happiness of {count} animals has been set to max.");
            return;
        }

        switch (value)
        {
            case "friendship" or "friendly":
                this.Handler.Log.I($"The friendship of {count} animals has been set to max.");
                break;
            case "happiness" or "happy" or "mood":
                this.Handler.Log.I($"The happiness of {count} animals has been set to max.");
                break;
        }
    }

    private void SetEcologistVarietiesForaged(string value)
    {
        if (!Game1.player.HasProfession(Profession.Ecologist))
        {
            this.Handler.Log.W("You must have the Ecologist profession.");
            return;
        }

        var parsed = 0;
        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out parsed))
        {
            this.Handler.Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(
            Game1.player,
            DataKeys.EcologistVarietiesForaged,
            string.IsNullOrEmpty(value) ? value : string.Join(',', Enumerable.Range(0, parsed)));
        this.Handler.Log.I($"Varieties foraged as Ecologist was set to {value}.");
    }

    private void SetGemologistMineralsStudied(string value)
    {
        if (!Game1.player.HasProfession(Profession.Gemologist))
        {
            this.Handler.Log.W("You must have the Gemologist profession.");
            return;
        }

        var parsed = 0;
        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out parsed))
        {
            this.Handler.Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(
            Game1.player,
            DataKeys.GemologistMineralsStudied,
            string.IsNullOrEmpty(value) ? value : string.Join(',', Enumerable.Range(0, parsed)));
        this.Handler.Log.I($"Minerals collected as Gemologist was set to {value}.");
    }

    private void SetProspectorHuntStreak(string value)
    {
        if (!Game1.player.HasProfession(Profession.Prospector))
        {
            this.Handler.Log.W("You must have the Prospector profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            this.Handler.Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(Game1.player, DataKeys.ProspectorHuntStreak, value);
        this.Handler.Log.I($"Prospector Hunt was streak set to {value}.");
    }

    private void SetScavengerHuntStreak(string value)
    {
        if (!Game1.player.HasProfession(Profession.Scavenger))
        {
            this.Handler.Log.W("You must have the Scavenger profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            this.Handler.Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(Game1.player, DataKeys.ScavengerHuntStreak, value);
        this.Handler.Log.I($"Scavenger Hunt streak was set to {value}.");
    }

    private void SetConservationistTrashCollectedThisSeason(string value)
    {
        if (!Game1.player.HasProfession(Profession.Conservationist))
        {
            this.Handler.Log.W("You must have the Conservationist profession.");
            return;
        }

        if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out _))
        {
            this.Handler.Log.W($"{value} is not a valid integer value.");
            return;
        }

        Data.Write(Game1.player, DataKeys.ConservationistTrashCollectedThisSeason, value);
        this.Handler.Log.I(
            $"Conservationist trash collected in the current season ({Game1.CurrentSeasonDisplayName}) was set to {value}.");
    }

    private void SetFishingRodMemory(string value)
    {
        if (Game1.player.CurrentTool is not FishingRod { UpgradeLevel: > 2 } rod)
        {
            this.Handler.Log.W("You must equip an Iridium Rod to use this command.");
            return;
        }

        if (value is not ("686" or "687" or "691" or "692" or "693" or "694" or "695" or "856" or "877" or "SonarBobber"))
        {
            this.Handler.Log.W($"{value} is not a valid tackle ID.");
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
