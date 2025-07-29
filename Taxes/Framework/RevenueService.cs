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
        var taxable = (int)(Math.Max(income - expenses, 0) * (1f - deductions));
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
        var season = Game1.dayOfMonth <= Config.IncomeTaxDay ? Game1.season.Previous() : Game1.season;
        Log.I($@"
            Income Tax Report for {farmer.Name}
            ===============================================
            Season Summary:
                - Season:                    {season} (Year {Game1.year})
                - Total Income:              {income}g
                - Business Expenses:         {expenses}g
                - Eligible Deductions:       {deductions:0.0%}

            Tax Details:
                - Taxable Amount:            {taxable}g
                - Tax Bracket:               {tax:0.0%}
                - Total Tax Due:             {dueI}g

            Generated on {Game1.currentSeason} {Game1.dayOfMonth}, Year {Game1.year}
            ===============================================
        ");
        return (dueI, income, expenses, deductions, taxable);
    }
}
