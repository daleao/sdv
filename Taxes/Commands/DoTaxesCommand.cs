namespace DaLion.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DoTaxesCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class DoTaxesCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["calculate", "check", "do", "report"];

    /// <inheritdoc />
    public override string Documentation =>
        "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W(
                "You must specify a value for the type of report. Accepted values are \"income\" or \"property\" for the corresponding tax report, or \"debt\" for outstanding liabilities report.");
            return true;
        }

        var player = Game1.player;
        switch (args[0].ToLowerInvariant())
        {
            case "income":
            {
                RevenueService.CalculateTaxes(player);
                break;
            }

            case "property":
            case "estate":
            {
                if (args.Length > 1 && args[1].ToLowerInvariant() is "--force" or "-f")
                {
                    var farm = Game1.getFarm();
                    var (agricultureValue, livestockValue, artisanValue, buildingValue, usedTiles, treeCount) =
                        farm.Appraise();
                    Data.Write(farm, DataKeys.AgricultureValue, agricultureValue.ToString());
                    Data.Write(farm, DataKeys.LivestockValue, livestockValue.ToString());
                    Data.Write(farm, DataKeys.ArtisanValue, artisanValue.ToString());
                    Data.Write(farm, DataKeys.BuildingValue, buildingValue.ToString());
                    Data.Write(farm, DataKeys.TreeCount, treeCount.ToString());
                    Data.Write(farm, DataKeys.UsedTiles, usedTiles.ToString());
                }

                CountyService.CalculateTaxes();
                break;
            }

            case "debt":
                var debt = Data.ReadAs<int>(player, DataKeys.DebtOutstanding);
                Log.I(
                    $"Outstanding debt on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}: {debt}g");
                break;

            default:
                Log.W(
                    "You must specify either \"income\" or \"property\" for the tax report type, or \"debt\" for outstanding liabilities.");
                return false;
        }

        return true;
    }
}
