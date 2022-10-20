namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using DaLion.Common.Enums;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Taxes.Framework;
using static System.FormattableString;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Calculates due income tax for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The amount of income tax due in gold.</returns>
    public static int DoTaxes(this Farmer farmer)
    {
        var income = farmer.Read<int>(DataFields.SeasonIncome);
        var expenses = farmer.Read<int>(DataFields.BusinessExpenses);
        var deductions = farmer.Read<float>(DataFields.PercentDeductions);
        var taxable = (int)((income - expenses) * (1f - deductions));

        var dueF = 0f;
        var bracket = 0f;
        for (var i = 0; i < 7; ++i)
        {
            bracket = Utils.Brackets[i];
            var threshold = Utils.Thresholds[bracket];
            if (taxable > threshold)
            {
                dueF += threshold * bracket;
                taxable -= threshold;
            }
            else
            {
                dueF += taxable * bracket;
                break;
            }
        }

        var dueI = (int)Math.Round(dueF);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {SeasonExtensions.Previous()} season, year {Game1.year}:" +
            $"\n\t- Season income: {income}g" +
            $"\n\t- Business expenses: {expenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductions:p0}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Due amount: {dueI}g.");
        return dueI;
    }
}
