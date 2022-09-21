namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using System;
using DaLion.Common;
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
        var income = farmer.Read<int>("SeasonIncome");
        var deductible = farmer.Read<float>("DeductionPct");
        var taxable = (int)(income * (1f - deductible));

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
            $"\n\t- Total income: {income}g" +
            CurrentCulture($"\n\t- Tax deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Total due income tax: {dueI}g.");
        return dueI;
    }
}
