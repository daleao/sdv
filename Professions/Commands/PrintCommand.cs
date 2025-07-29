namespace DaLion.Professions.Commands;

#region using directives

using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;
using static System.String;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PrintCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class PrintCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["print", "read", "show", "log", "list"];

    /// <inheritdoc />
    public override string Documentation => "Print the specified information.";

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

            player = Game1.getFarmer(multiplayerId.Value);
            if (player is null)
            {
                Log.W($"Couldn't find online player with specified player screen ID \"{screenId}\".");
                return false;
            }
        }

        if (tokens.Count == 0)
        {
            this.PrintProfessionsList([], player);
            return true;
        }

        switch (tokens[0].ToLower())
        {
            case "data":
                this.PrintModData(player);
                break;

            case "limit":
            case "ulti":
                this.PrintLimitBreak(player);
                break;

            case "fishdex":
            case "fish":
                this.PrintFishPokedex(player);
                break;

            default:
                this.PrintProfessionsList(tokens, player);
                break;
        }

        return true;
    }

    /// <inheritdoc />
    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [<key>]");
        sb.Append("\n\nParameters:");
        sb.Append(
            "\n\t<key> - What you would like to print. Accepted values are \"data\", \"limit\", \"fishdex\", any skill name, or \"all\" for all skills.");
        sb.Append("\n\nExamples:");
        return sb.ToString();
    }

    private void PrintProfessionsList(List<string> tokens, Farmer? who = null)
    {
        who ??= Game1.player;
        if (tokens.Count == 0 && who.professions.Count == 0)
        {
            Log.I($"Farmer {who.Name} doesn't have any professions.");
            return;
        }

        var sb = new StringBuilder("Query result:");
        if (tokens.Count > 0)
        {
            var q = new Queue<ISkill>();
            if (tokens[0] == "all")
            {
                Skill.List.ForEach(q.Enqueue);
                CustomSkill.Loaded.Values.ForEach(q.Enqueue);
            }
            else
            {
                foreach (var token in tokens)
                {
                    if (Skill.TryFromName(token, true, out var vanillaSkill))
                    {
                        q.Enqueue(vanillaSkill);
                    }
                    else
                    {
                        var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                            s.StringId.ToLower().Contains(token.ToLowerInvariant()) ||
                            s.DisplayName.ToLower().Contains(token.ToLowerInvariant()));
                        if (customSkill is not null)
                        {
                            q.Enqueue(customSkill);
                        }
                        else
                        {
                            Log.W($"{token} is not a valid skill name.");
                        }
                    }
                }
            }

            while (q.TryDequeue(out var skill))
            {
                var currentLevel = skill is VanillaSkill vSkill1
                    ? who.GetUnmodifiedSkillLevel(vSkill1.Value)
                    : SCSkills.GetSkillLevel(who, skill.StringId);
                var currentExp = skill is VanillaSkill vSkill2
                    ? who.experiencePoints[vSkill2.Value]
                    : SCSkills.GetExperienceFor(who, skill.StringId);
                var maxLevel = skill is VanillaSkill vSkill3
                    ? vSkill3.MaxLevel
                    : 10;
                sb.Append($"\n{skill.StringId} " +
                          $"LV{currentLevel} " +
                          $"(EXP: {currentExp}" +
                          (currentLevel < maxLevel
                              ? $" / {ISkill.ExperienceCurve[skill.CurrentLevel + 1]})"
                              : ')'));
                var professionsInSkill = who.GetProfessionsForSkill(skill);
                Array.Sort(professionsInSkill);
                var list = professionsInSkill.Aggregate(
                    Empty,
                    (current, next) =>
                        current +
                        $"{(next.Level == 5 ? "\n>" : "\n - ")} {next.StringId} {(who.professions.Contains(next.Id + 100) ? "(P) " : string.Empty)}(LV{next.Level} / ID: {next.Id})");
                sb.Append(list);
            }
        }
        else
        {
            List<IProfession> professions = [];
            List<int> unknown = [];
            foreach (var pid in who.professions)
            {
                if (Profession.TryFromValue(pid >= 100 ? pid - 100 : pid, out var vanillaProfession))
                {
                    if (pid < 100)
                    {
                        professions.Add(vanillaProfession);
                    }
                }
                else if (CustomProfession.Loaded.TryGetValue(pid, out var customProfession) ||
                         CustomProfession.Loaded.TryGetValue(pid - 100, out customProfession))
                {
                    if (pid == customProfession.Id)
                    {
                        professions.Add(customProfession);
                    }
                }
                else
                {
                    unknown.Add(pid);
                }
            }

            professions.Sort();
            foreach (var profession in professions)
            {
                sb.Append(
                    $"\n- {profession.StringId} ({profession.ParentSkill.StringId} LV{profession.Level} / ID: {profession.Id})");
                if (who.professions.Contains(profession.Id + 100))
                {
                    sb.Append(
                        $"\n- Prestiged {profession.StringId} ({profession.ParentSkill.StringId} LV{profession.Level + 10} / ID: {profession.Id + 100})");
                }
            }

            unknown.Sort();
            foreach (var pid in unknown)
            {
                sb.Append($"\n- Unknown profession {pid}");
            }
        }

        Log.I(sb.ToString());
    }

    private void PrintModData(Farmer? who = null)
    {
        who ??= Game1.player;
        var sb = new StringBuilder($"Farmer {who.Name}'s mod data:");
        var value = Data.Read(who, DataKeys.EcologistVarietiesForaged);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine();
            var parsed = value.ParseList<string>();
            var which = parsed.Select(p => ItemRegistry.Create(p).Name);
            sb.AppendLine("=== Ecologist Progress ===");
            sb.AppendLine($"Forage varieties collected:          {parsed.Count}");
            sb.AppendLine($"Current expected quality:            {(ObjectQuality)who.GetEcologistForageQuality()}");
            sb.AppendLine($"Varieties left for best quality:     {Config.ForagesNeededForBestQuality - parsed.Count}");
            sb.AppendLine();
            sb.AppendLine("Collected varieties:");
            foreach (var name in which)
            {
                sb.AppendLine($"- {name}");
            }
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for EcologistVarietiesForaged.");
        }

        value = Data.Read(who, DataKeys.GemologistMineralsStudied);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine();
            var parsed = value.ParseList<string>();
            var which = parsed.Select(p => ItemRegistry.Create(p).Name);
            sb.AppendLine("=== Gemologist Progress ===");
            sb.AppendLine($"Mineral varieties studied:          {parsed.Count}");
            sb.AppendLine($"Current expected quality:           {(ObjectQuality)who.GetEcologistForageQuality()}");
            sb.AppendLine($"Varieties left for best quality:    {Config.ForagesNeededForBestQuality - parsed.Count}");
            sb.AppendLine();
            sb.AppendLine("Studied varieties:");
            foreach (var name in which)
            {
                sb.AppendLine($"- {name}");
            }
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for GemologistMineralsStudied.");
        }

        value = Data.Read(who, DataKeys.CurrentProspectorHuntStreak);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine();
            sb.AppendLine("=== Prospector Progress ===");
            sb.AppendLine($"Current win streak:     {value}");
            sb.AppendLine($"Longest win streak:     {Data.Read(who, DataKeys.LongestProspectorHuntStreak)}");
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for ProspectorHuntStreak.");
        }

        value = Data.Read(who, DataKeys.CurrentScavengerHuntStreak);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine();
            sb.AppendLine("=== Scavenger Progress ===");
            sb.AppendLine($"Current win streak:     {value}");
            sb.AppendLine($"Longest win streak:     {Data.Read(who, DataKeys.LongestScavengerHuntStreak)}");
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for ScavengerHuntStreak.");
        }

        value = Data.Read(who, DataKeys.ConservationistTrashCollectedThisSeason);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine();
            sb.AppendLine("=== Conservationist Progress ===");
            sb.AppendLine($"Trash collected in current season:         {value}");
            sb.AppendLine($"Expected tax deduction next season:        {Math.Min((int)float.Parse(value) / Config.ConservationistTrashNeededPerTaxDeduction / 100f, Config.ConservationistTaxDeductionCeiling):0%}");
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for ConservationistTrashCollectedThisSeason.");
        }

        value = Data.Read(who, DataKeys.ConservationistActiveTaxDeduction);
        if (!IsNullOrEmpty(value))
        {
            sb.AppendLine($"Active tax deduction in current season:    {float.Parse(value):0%}");
        }
        else
        {
            sb.AppendLine("- Mod data does not contain an entry for ConservationistActiveTaxDeduction.");
        }

        // if (player.HasProfession(Profession.Angler) && player.CurrentTool is FishingRod rod)
        // {
        //     value = Data.Read(rod, DataKeys.FirstMemorizedTackle);
        //     sb.Append("\n\t- ").Append(
        //         !IsNullOrEmpty(value)
        //             ? CurrentCulture($"Fishing Rod Tackle Memory Slot 1: {value}")
        //             : "Mod data does not contain an entry for FirstMemorizedTackle.");
        //
        //     value = Data.Read(rod, DataKeys.SecondMemorizedTackle);
        //     sb.Append("\n\t- ").Append(
        //         !IsNullOrEmpty(value)
        //             ? CurrentCulture($"Fishing Rod Tackle Memory Slot 2: {value}")
        //             : "Mod data does not contain an entry for SecondMemorizedTackle.");
        //
        //     value = Data.Read(rod, DataKeys.FirstMemorizedTackleUses);
        //     sb.Append("\n\t- ").Append(
        //         !IsNullOrEmpty(value)
        //             ? CurrentCulture($"Fishing Rod Tackle Memory Uses Slot 1: {value}")
        //             : "Mod data does not contain an entry for FirstMemorizedTackleprfsUses.");
        //
        //     value = Data.Read(rod, DataKeys.SecondMemorizedTackleUses);
        //     sb.Append("\n\t- ").Append(
        //         !IsNullOrEmpty(value)
        //             ? CurrentCulture($"Fishing Rod Tackle Memory Uses Slot 2: {value}")
        //             : "Mod data does not contain an entry for SecondMemorizedTackleUses.");
        // }

        Log.I(sb.ToString());
    }

    private void PrintLimitBreak(Farmer? who = null)
    {
        who ??= Game1.player;
        var limit = State.LimitBreak;
        if (limit is null)
        {
            Log.I($"Farmer {who.Name} does not have a Limit Break.");
            return;
        }

        Log.I(
            $"{who.Name} has broken the limits of the {limit.ParentProfession.Title} profession and acquired the {limit.DisplayName} Limit Break.");
    }

    private void PrintFishPokedex(Farmer? who = null)
    {
        who ??= Game1.player;
        if (!who.fishCaught.Pairs.Any())
        {
            Log.W("You haven't caught any fish.");
            return;
        }

        var fishData = DataLoader.Fish(Game1.content);
        int numLegendaryCaught = 0, numMaxSizedCaught = 0, numCaught = 0;
        List<string> caughtFishNames = [];
        Dictionary<string, Tuple<int, int>> nonMaxSizedCaught = [];
        var sb = new StringBuilder();
        foreach (var (key, value) in Game1.player.fishCaught.Pairs)
        {
            if (key.IsTrashId() || !fishData.TryGetValue(key, out var specificFishData) ||
                specificFishData.Contains("trap"))
            {
                continue;
            }

            var dataFields = specificFishData.SplitWithoutAllocation('/');
            var name = dataFields[0].ToString();
            if (key.IsBossFishId())
            {
                numLegendaryCaught++;
            }
            else
            {
                numCaught++;
                var maxSize = int.Parse(dataFields[4]);
                if (value[1] > maxSize)
                {
                    numMaxSizedCaught++;
                }
                else
                {
                    nonMaxSizedCaught.Add(
                        name,
                        new Tuple<int, int>(value[1], maxSize));
                }
            }

            caughtFishNames.Add(name);
        }

        var priceMultiplier = Game1.player.HasProfession(Profession.Angler)
            ? CurrentCulture(
                $"{Math.Min((numMaxSizedCaught * 0.01f) + (numLegendaryCaught * 0.05f), Config.AnglerPriceBonusCeiling):0%}")
            : "zero. You're not an Angler..";
        sb.Append(
            $"You've caught {Game1.player.fishCaught.Count()} out of {fishData.Count} fishes. Of those, {numMaxSizedCaught} are max-sized, and {numLegendaryCaught} are legendary. You're total Angler price bonus is {priceMultiplier}." +
            "\nThe following caught fish are not max-sized:");
        sb.Append(nonMaxSizedCaught.Keys.Aggregate(
            sb,
            (current, fish) =>
                current.Append(
                    $"\n\t- {fish} (current: {nonMaxSizedCaught[fish].Item1}, max: {nonMaxSizedCaught[fish].Item2})")));

        var seasonFish = from specificFishData in fishData.Values
            where specificFishData.SplitWithoutAllocation('/')[6]
                .Contains(Game1.currentSeason, StringComparison.Ordinal)
            select specificFishData.SplitWithoutAllocation('/')[0].ToString();
        sb.Append("\nThe following fish can be caught this season:");
        sb = seasonFish.Except(caughtFishNames)
            .Aggregate(sb, (current, fish) => current.Append($"\n\t- {fish}"));

        Log.I(sb.ToString());
    }
}
