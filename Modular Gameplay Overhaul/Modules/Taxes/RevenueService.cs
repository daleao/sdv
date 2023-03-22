namespace DaLion.Overhaul.Modules.Taxes;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

/// <summary>Responsible for collecting federal taxes and administering the Ferngill Revenue Code.</summary>
internal static class RevenueService
{
    internal static ImmutableDictionary<int, float> TaxByIncomeBrackets { get; } = TaxesModule.Config.IncomeBrackets
        .Zip(TaxesModule.Config.IncomeTaxPerBracket, (key, value) => new { key, value })
        .ToImmutableDictionary(p => p.key, p => p.value);

    /// <summary>Calculates due income tax for the <paramref name="who"/>.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <returns>The amount of income tax due in gold.</returns>
    internal static int CalculateTaxes(Farmer who)
    {
        var income = who.Read<int>(DataKeys.SeasonIncome);
        var expenses = Math.Min(who.Read<int>(DataKeys.BusinessExpenses), income);
        var deductions = who.Read<float>(DataKeys.PercentDeductions);
        var taxable = (int)((income - expenses) * (1f - deductions));

        var dueF = 0f;
        var tax = 0f;
        var temp = taxable;
        foreach (var bracket in TaxByIncomeBrackets.Keys)
        {
            tax = TaxByIncomeBrackets[bracket];
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
            $"Accounting results for {who.Name} over the closing {SeasonExtensions.Previous()} season, year {Game1.year}:" +
            $"\n\t- Season income: {income}g" +
            $"\n\t- Business expenses: {expenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductions:0%}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {tax:0%}") +
            $"\n\t- Due amount: {dueI}g.");
        return dueI;
    }
}
