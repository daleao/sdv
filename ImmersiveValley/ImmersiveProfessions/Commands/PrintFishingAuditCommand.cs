namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions;
using Extensions;
using Framework;
using Framework.Utility;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintFishingAuditCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintFishingAuditCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_fishdex", "fishdex" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Check how many fish have been caught at max-size. Relevant for {Profession.Angler.Name}s.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!Game1.player.fishCaught.Pairs.Any())
        {
            Log.W("You haven't caught any fish.");
            return;
        }

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsIn(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);
        int numLegendariesCaught = 0, numMaxSizedCaught = 0;
        var caughtFishNames = new List<string>();
        var nonMaxSizedCaught = new Dictionary<string, Tuple<int, int>>();
        var result = "";
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
                if (value[1] > Convert.ToInt32(dataFields[4]))
                    ++numMaxSizedCaught;
                else
                    nonMaxSizedCaught.Add(dataFields[0],
                        new(value[1], Convert.ToInt32(dataFields[4])));
            }

            caughtFishNames.Add(dataFields[0]);
        }

        var priceMultiplier = Game1.player.HasProfession(Profession.Angler)
            ? CurrentCulture($"{numMaxSizedCaught * 0.01f + numLegendariesCaught * 0.05f:p0}")
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
}