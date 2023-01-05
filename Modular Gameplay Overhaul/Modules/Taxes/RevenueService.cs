namespace DaLion.Overhaul.Modules.Taxes;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

internal static class RevenueService
{
    internal static float[] Brackets { get; } = { 0.1f, 0.12f, 0.22f, 0.24f, 0.32f, 0.35f, 0.37f };

    internal static IReadOnlyDictionary<float, int> Thresholds { get; } = new Dictionary<float, int>()
    {
        { 0.1f, 9950 },
        { 0.12f, 40525 },
        { 0.22f, 86375 },
        { 0.24f, 164925 },
        { 0.32f, 209425 },
        { 0.35f, 523600 },
        { 0.37f, int.MaxValue },
    };

    /// <summary>Calculates due income tax for the <paramref name="who"/>.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <returns>The amount of income tax due in gold.</returns>
    internal static int CalculateTaxes(Farmer who)
    {
        var income = who.Read<int>(DataFields.SeasonIncome);
        var expenses = Math.Min(who.Read<int>(DataFields.BusinessExpenses), income);
        var deductions = who.Read<float>(DataFields.PercentDeductions);
        var taxable = (int)((income - expenses) * (1f - deductions));

        var dueF = 0f;
        var bracket = 0f;
        var temp = taxable;
        for (var i = 0; i < 7; i++)
        {
            bracket = Brackets[i];
            var threshold = Thresholds[bracket];
            if (temp > threshold)
            {
                dueF += threshold * bracket;
                temp -= threshold;
            }
            else
            {
                dueF += temp * bracket;
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
            CurrentCulture($"\n\t- Tax bracket: {bracket:0%}") +
            $"\n\t- Due amount: {dueI}g.");
        return dueI;
    }
}
