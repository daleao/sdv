namespace DaLion.Professions.Commands;

#region using directives

using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;
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
        if (args.Length == 0)
        {
            this.PrintProfessionsList([]);
            return true;
        }

        switch (args[0].ToLower())
        {
            case "data":
                this.PrintModData();
                break;

            case "limit":
            case "ulti":
                this.PrintLimitBreak();
                break;

            case "fishdex":
            case "fish":
                this.PrintFishPokedex();
                break;

            default:
                this.PrintProfessionsList(args);
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

    private void PrintProfessionsList(string[] args)
    {
        if (args.Length == 0 && Game1.player.professions.Count == 0)
        {
            this.Handler.Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        var sb = new StringBuilder("Query result:");
        if (args.Length > 0)
        {
            var q = new Queue<ISkill>();
            if (args[0] == "all")
            {
                Skill.List.ForEach(q.Enqueue);
                CustomSkill.Loaded.Values.ForEach(q.Enqueue);
            }
            else
            {
                foreach (var arg in args)
                {
                    if (Skill.TryFromName(arg, true, out var vanillaSkill))
                    {
                        q.Enqueue(vanillaSkill);
                    }
                    else
                    {
                        var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                            s.StringId.ToLower().Contains(arg.ToLowerInvariant()) ||
                            s.DisplayName.ToLower().Contains(arg.ToLowerInvariant()));
                        if (customSkill is not null)
                        {
                            q.Enqueue(customSkill);
                        }
                        else
                        {
                            this.Handler.Log.W($"{arg} is not a valid skill name.");
                        }
                    }
                }
            }

            while (q.TryDequeue(out var skill))
            {
                sb.Append($"\n{skill.StringId} " +
                          $"LV{skill.CurrentLevel} " +
                          $"(EXP: {skill.CurrentExp}" +
                          (skill.CurrentLevel < skill.MaxLevel
                              ? $" / {ISkill.ExperienceCurve[skill.CurrentLevel + 1]})"
                              : ')'));
                var professionsInSkill = Game1.player.GetProfessionsForSkill(skill);
                Array.Sort(professionsInSkill);
                var list = professionsInSkill.Aggregate(
                    Empty,
                    (current, next) =>
                        current +
                        $"{(next.Level == 5 ? "\n>" : "\n - ")} {next.StringId} {(Game1.player.professions.Contains(next.Id + 100) ? "(P) " : string.Empty)}(LV{next.Level} / ID: {next.Id})");
                sb.Append(list);
            }
        }
        else
        {
            List<IProfession> professions = [];
            List<int> unknown = [];
            foreach (var pid in Game1.player.professions)
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
                if (Game1.player.professions.Contains(profession.Id + 100))
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

        this.Handler.Log.I(sb.ToString());
    }

    private void PrintModData()
    {
        var player = Game1.player;
        var sb = new StringBuilder($"Farmer {player.Name}'s mod data:");
        var value = Data.Read(player, DataKeys.EcologistVarietiesForaged);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? $"Ecologist Varieties Foraged: {value}\n\t\tExpected quality: {(ObjectQuality)player.GetEcologistForageQuality()}" +
                  (int.Parse(value) < Config.ForagesNeededForBestQuality
                      ? $" ({Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                      : Empty)
                : "Mod data does not contain an entry for EcologistVarietiesForaged.");

        value = Data.Read(player, DataKeys.GemologistMineralsStudied);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? $"Gemologist Minerals Studied: {value}\n\t\tExpected quality: {(ObjectQuality)player.GetGemologistMineralQuality()}" +
                  (int.Parse(value) < Config.MineralsNeededForBestQuality
                      ? $" ({Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                      : Empty)
                : "Mod data does not contain an entry for GemologistMineralsStudied.");

        value = Data.Read(player, DataKeys.ProspectorHuntStreak);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? $"Prospector Hunt Streak: {value} (affects Prospector Hunt treasure quality)"
                : "Mod data does not contain an entry for ProspectorHuntStreak.");

        value = Data.Read(player, DataKeys.ScavengerHuntStreak);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? $"Scavenger Hunt Streak: {value} (affects Scavenger Hunt treasure quality)"
                : "Mod data does not contain an entry for ScavengerHuntStreak.");

        value = Data.Read(player, DataKeys.ConservationistTrashCollectedThisSeason);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? $"Conservationist Trash Collected ({Game1.season}): {value}\n\t\tExpected tax deduction for {Game1.season.Next()}: " +
                  // ReSharper disable once PossibleLossOfFraction
                  $"{Math.Min((int)float.Parse(value) / Config.ConservationistTrashNeededPerTaxDeduction / 100f, Config.ConservationistTaxDeductionCeiling):0%}"
                : "Mod data does not contain an entry for ConservationistTrashCollectedThisSeason.");

        value = Data.Read(player, DataKeys.ConservationistActiveTaxDeduction);
        sb.Append("\n\t- ").Append(
            !IsNullOrEmpty(value)
                ? CurrentCulture($"Conservationist Active Tax Deduction: {float.Parse(value):0%}")
                : "Mod data does not contain an entry for ConservationistActiveTaxDeduction.");

        if (player.HasProfession(Profession.Angler) && player.CurrentTool is FishingRod rod)
        {
            value = Data.Read(rod, DataKeys.FirstMemorizedTackle);
            sb.Append("\n\t- ").Append(
                !IsNullOrEmpty(value)
                    ? CurrentCulture($"Fishing Rod Tackle Memory Slot 1: {value}")
                    : "Mod data does not contain an entry for FirstMemorizedTackle.");

            value = Data.Read(rod, DataKeys.SecondMemorizedTackle);
            sb.Append("\n\t- ").Append(
                !IsNullOrEmpty(value)
                    ? CurrentCulture($"Fishing Rod Tackle Memory Slot 2: {value}")
                    : "Mod data does not contain an entry for SecondMemorizedTackle.");

            value = Data.Read(rod, DataKeys.FirstMemorizedTackleUses);
            sb.Append("\n\t- ").Append(
                !IsNullOrEmpty(value)
                    ? CurrentCulture($"Fishing Rod Tackle Memory Uses Slot 1: {value}")
                    : "Mod data does not contain an entry for FirstMemorizedTackleprfsUses.");

            value = Data.Read(rod, DataKeys.SecondMemorizedTackleUses);
            sb.Append("\n\t- ").Append(
                !IsNullOrEmpty(value)
                    ? CurrentCulture($"Fishing Rod Tackle Memory Uses Slot 2: {value}")
                    : "Mod data does not contain an entry for SecondMemorizedTackleUses.");
        }

        this.Handler.Log.I(sb.ToString());
    }

    private void PrintLimitBreak()
    {
        var limit = State.LimitBreak;
        if (limit is null)
        {
            Log.I($"Farmer {Game1.player.Name} does not have a Limit Break.");
            return;
        }

        Log.I(
            $"{Game1.player.Name} has broken the limits of the {limit.ParentProfession.Title} profession and acquired the {limit.DisplayName} Limit Break.");
    }

    private void PrintFishPokedex()
    {
        if (!Game1.player.fishCaught.Pairs.Any())
        {
            this.Handler.Log.W("You haven't caught any fish.");
            return;
        }

        var fishData = DataLoader.Fish(Game1.content);
        int numLegendaryCaught = 0, numMaxSizedCaught = 0, numCaught = 0;
        var caughtFishNames = new List<string>();
        var nonMaxSizedCaught = new Dictionary<string, Tuple<int, int>>();
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

        this.Handler.Log.I(sb.ToString());
    }
}
