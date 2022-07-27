namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using Common;
using Common.ModData;
using System;
using static System.FormattableString;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Calculate due income tax for the player.</summary>
    public static int DoTaxes(this Farmer farmer)
    {
        var income = ModDataIO.Read<int>(farmer, "SeasonIncome");
        var deductible = ModDataIO.Read<float>(farmer, "DeductionPct");
        var taxable = (int)(income * (1f - deductible));
        var bracket = Framework.Utils.GetTaxBracket(taxable);
        var due = (int)Math.Round(taxable * bracket);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {Game1.game1.GetPrecedingSeason()} season, year {Game1.year}:" +
            $"\n\t- Total income: {income}g" +
            CurrentCulture($"\n\t- Tax deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Total due income tax: {due}g."
        );
        return due;
    }
}