namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class DoTaxesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DoTaxesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DoTaxesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "assess", "calculate", "check", "evaluate", "do", "report" };

    /// <inheritdoc />
    public override string Documentation =>
        "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || args[0].ToLowerInvariant() is not ("income" or "property" or "debt"))
        {
            Log.W("You must specify either \"income\" or \"property\" for the tax report type, or \"debt\" for outstanding liabilities.");
        }

        if (!SeasonExtensions.TryParse(Game1.currentSeason, true, out var currentSeason))
        {
            Log.E($"Failed to parse the current season {Game1.currentSeason}");
            return;
        }

        var player = Game1.player;
        switch (args[0].ToLowerInvariant())
        {
            case "income":
            {
                var forClosingSeason = Game1.dayOfMonth == 1;
                var seasonIncome = player.Read<int>(DataKeys.SeasonIncome);
                var businessExpenses = player.Read<int>(DataKeys.BusinessExpenses);
                var deductiblePct = ProfessionsModule.ShouldEnable && player.professions.Contains(Farmer.mariner)
                    ? forClosingSeason
                        ? player.Read<float>(DataKeys.PercentDeductions)
                        // ReSharper disable once PossibleLossOfFraction
                        : player.Read<int>(Professions.DataKeys.ConservationistTrashCollectedThisSeason) / ProfessionsModule.Config.TrashNeededPerTaxDeductionPct / 100f
                    : 0f;
                var taxable = (int)((seasonIncome - businessExpenses) * (1f - deductiblePct));

                var dueF = 0f;
                var tax = 0f;
                var temp = taxable;
                foreach (var bracket in RevenueService.TaxByIncomeBrackets.Keys)
                {
                    tax = RevenueService.TaxByIncomeBrackets[bracket];
                    if (temp > bracket)
                    {
                        dueF += bracket * tax;
                        temp -= bracket;
                    }
                    else
                    {
                        dueF += temp * tax;
                        break;
                    }
                }

                var dueI = (int)Math.Round(dueF);
                Log.I(
                    "Accounting " + (forClosingSeason ? "report" : "projection") + " for the " +
                    (forClosingSeason ? $"closing {currentSeason.Previous()}" : $"current {currentSeason}") + " season:" +
                    $"\n\t- Income (season-to-date): {seasonIncome}g" +
                    $"\n\t- Business expenses: {businessExpenses}g" +
                    CurrentCulture($"\n\t- Eligible deductions: {deductiblePct:0.0%}") +
                    $"\n\t- Taxable amount: {taxable}g" +
                    CurrentCulture($"\n\t- Bracket: {tax:0.0%}") +
                    $"\n\t- Income tax due: {dueI}g." +
                    $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}.");

                break;
            }

            case "property":
            {
                var farm = Game1.getFarm();
                var (agricultureValue, livestockValue, buildingValue, usedTiles) = farm.Appraise(false);
                var usableTiles = farm.Read<int>(DataKeys.UsableTiles);
                var usedPct = (float)usedTiles / usableTiles;
                var owedOverUsedLand = (int)((agricultureValue + livestockValue) * usedPct * TaxesModule.Config.UsedTileTaxRate);
                var owedOverUnusedLand = (int)((agricultureValue + livestockValue) * (1f - usedPct) * TaxesModule.Config.UnusedTileTaxRate);
                var owedOverBuildings = (int)(buildingValue * TaxesModule.Config.BuildingTaxRate);
                Log.I(
                    $"Use-value assessment for {farm.Name} (year-to-date):" +
                    $"\n\t- Agricultural value: {agricultureValue}g" +
                    $"\n\t- Livestock value: {livestockValue}g" +
                    $"\n\t- Building value: {buildingValue}g" +
                    $"\n\t\t- Total property value: {agricultureValue + livestockValue + buildingValue}g" +
                    CurrentCulture($"\n\t- Used Tiles: {usedTiles} ({usedPct:0.0%})") +
                    $"\n\t- Tax owed over used land: {owedOverUsedLand}g" +
                    $"\n\t- Tax owed over real-estate: {owedOverBuildings}g" +
                    $"\n\t- Tax owed over unused land: {owedOverUnusedLand}g" +
                    $"\n\t\t- Total property tax owed: {owedOverUsedLand + owedOverUnusedLand + owedOverBuildings}g" +
                    $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}.");

                break;
            }

            case "debt":
                var debt = player.Read<int>(DataKeys.DebtOutstanding);
                Log.I($"Outstanding debt on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}: {debt}g");
                break;
        }
    }
}
