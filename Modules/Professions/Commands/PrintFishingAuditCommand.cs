namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintFishingAuditCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintFishingAuditCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintFishingAuditCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_fishdex", "fishdex" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Check how many fish have been caught at max-size. Relevant for {VanillaProfession.Angler.Name}s.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!Game1.player.fishCaught.Pairs.Any())
        {
            Log.W("You haven't caught any fish.");
            return;
        }

        var fishData = ModHelper.GameContent.Load<Dictionary<int, string>>("Data/Fish");
        int numLegendaryCaught = 0, numMaxSizedCaught = 0, numCaught = 0;
        var caughtFishNames = new List<string>();
        var nonMaxSizedCaught = new Dictionary<string, Tuple<int, int>>();
        var result = new StringBuilder();
        foreach (var (key, value) in Game1.player.fishCaught.Pairs)
        {
            if (key is 152 or 153 or 157 || !fishData.TryGetValue(key, out var specificFishData) ||
                specificFishData.Contains("trap"))
            {
                continue;
            }

            var dataFields = specificFishData.SplitWithoutAllocation('/');
            var name = dataFields[0].ToString();
            if (Lookups.LegendaryFishes.Contains(name))
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

        var priceMultiplier = Game1.player.HasProfession(VanillaProfession.Angler)
            ? Game1.player.HasProfession(VanillaProfession.Angler, true)
                ? CurrentCulture($"{Math.Min((numMaxSizedCaught * 0.01f) + ((numCaught - numMaxSizedCaught) * 0.005f) + (numLegendaryCaught * 0.025f), ProfessionsModule.Config.AnglerPriceBonusCeiling):0%}")
                : CurrentCulture($"{Math.Min((numCaught * 0.005f) + (numLegendaryCaught * 0.025f), ProfessionsModule.Config.AnglerPriceBonusCeiling):0%}")
            : "zero. You're not an Angler..";
        result.Append(
            $"You've caught {Game1.player.fishCaught.Count()} out of {fishData.Count} fishes. Of those, {numMaxSizedCaught} are max-sized, and {numLegendaryCaught} are legendary. You're total Angler price bonus is {priceMultiplier}." +
            "\nThe following caught fish are not max-sized:");
        nonMaxSizedCaught.Keys.Aggregate(
            result,
            (current, fish) =>
                current.Append($"\n\t- {fish} (current: {nonMaxSizedCaught[fish].Item1}, max: {nonMaxSizedCaught[fish].Item2})"));

        var seasonFish = from specificFishData in fishData.Values
            where specificFishData.SplitWithoutAllocation('/')[6].Contains(Game1.currentSeason, StringComparison.Ordinal)
            select specificFishData.SplitWithoutAllocation('/')[0].ToString();

        result.Append("\nThe following fish can be caught this season:");
        result = seasonFish.Except(caughtFishNames).Aggregate(result, (current, fish) => current.Append($"\n\t- {fish}"));

        Log.I(result.ToString());
    }
}
