namespace DaLion.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Taxes.Framework.Integrations;
using static System.FormattableString;

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
            this.Handler.Log.W(
                "You must specify a value for the type of report. Accepted values are \"income\" or \"property\" for the corresponding tax report, or \"debt\" for outstanding liabilities report.");
            return true;
        }

        var player = Game1.player;
        switch (args[0].ToLowerInvariant())
        {
            case "income":
            {
                var forClosingSeason = Game1.dayOfMonth == 1;
                var seasonIncome = Data.ReadAs<int>(player, DataKeys.SeasonIncome);
                var businessExpenses = Data.ReadAs<int>(player, DataKeys.BusinessExpenses);
                var deductible = ProfessionsIntegration.IsValueCreated && player.professions.Contains(Farmer.mariner)
                    ? forClosingSeason
                        ? Data.ReadAs<float>(player, DataKeys.PercentDeductions)
                        // ReSharper disable once PossibleLossOfFraction
                        : Data.ReadAs<int>(player, "ConservationistTrashCollectedThisSeason") /
                          ProfessionsIntegration.Instance.ModApi!.GetConfig()
                              .ConservationistTrashNeededPerTaxDeduction / 100f
                    : 0f;
                if (deductible > 0f)
                {
                    deductible = Math.Min(
                        deductible,
                        ProfessionsIntegration.Instance!.ModApi!.GetConfig().ConservationistTaxDeductionCeiling);
                }

                var taxable = (int)((seasonIncome - businessExpenses) * (1f - deductible));

                var dueF = 0f;
                var tax = 0f;
                var temp = taxable;
                foreach (var bracket in RevenueService.TaxByIncomeBracket.Keys)
                {
                    tax = RevenueService.TaxByIncomeBracket[bracket];
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
                this.Handler.Log.I(
                    "Accounting " + (forClosingSeason ? "report" : "projection") + " for the " +
                    (forClosingSeason ? $"closing {Game1.season.Previous()}" : $"current {Game1.season}") + " season:" +
                    $"\n\t- Income (season-to-date): {seasonIncome}g" +
                    $"\n\t- Business expenses: {businessExpenses}g" +
                    CurrentCulture($"\n\t- Eligible deductions: {deductible:0.0%}") +
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
                var usableTiles = Data.ReadAs<int>(farm, DataKeys.UsableTiles);
                var usedPct = (float)usedTiles / usableTiles;
                var owedOverUsedLand = (int)((agricultureValue + livestockValue) * usedPct * Config.UsedTileTaxRate);
                var owedOverUnusedLand =
                    (int)((agricultureValue + livestockValue) * (1f - usedPct) * Config.UnusedTileTaxRate);
                var owedOverBuildings = (int)(buildingValue * Config.BuildingTaxRate);
                this.Handler.Log.I(
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
                var debt = Data.ReadAs<int>(player, DataKeys.DebtOutstanding);
                this.Handler.Log.I(
                    $"Outstanding debt on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}: {debt}g");
                break;

            default:
                this.Handler.Log.W(
                    "You must specify either \"income\" or \"property\" for the tax report type, or \"debt\" for outstanding liabilities.");
                return false;
        }

        return true;
    }
}
