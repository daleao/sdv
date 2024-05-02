namespace DaLion.Taxes.Framework;

#region using directives

using System.Collections.Immutable;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

/// <summary>Collects federal taxes.</summary>
internal static class RevenueService
{
    internal static ImmutableDictionary<int, float> TaxByIncomeBracket { get; set; } = Config.TaxRatePerIncomeBracket.ToImmutableDictionary();

    /// <summary>Calculates due income tax for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The amount of income tax due in gold, along with other relevant stats.</returns>
    internal static (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateTaxes(Farmer farmer)
    {
        var income = Data.ReadAs<int>(farmer, DataKeys.SeasonIncome);
        var expenses = Math.Min(Data.ReadAs<int>(farmer, DataKeys.BusinessExpenses), income);
        var deductions = Data.ReadAs<float>(farmer, DataKeys.PercentDeductions);
        var taxable = (int)((income - expenses) * (1f - deductions));

        var dueF = 0f;
        var tax = 0f;
        var temp = taxable;
        foreach (var bracket in TaxByIncomeBracket.Keys)
        {
            tax = TaxByIncomeBracket[bracket];
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
            $"Accounting results for {farmer.Name} over the closing {Game1.season.Previous()} season, year {Game1.year}:" +
            $"\n\t- Season income: {income}g" +
            $"\n\t- Business expenses: {expenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductions:0.0%}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {tax:0.0%}") +
            $"\n\t- Due amount: {dueI}g.");
        return (dueI, income, expenses, deductions, taxable);
    }
}
