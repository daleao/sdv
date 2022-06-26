namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using Common;
using Common.Data;
using StardewValley;
using System;
using static System.FormattableString;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Calculate due income tax for the player.</summary>
    public static int DoTaxes(this Farmer farmer)
    {
        var income = ModDataIO.ReadDataAs<int>(farmer, ModData.SeasonIncome.ToString());
        var deductible = ModDataIO.ReadDataAs<float>(farmer, ModData.DeductionPct.ToString());
        var taxable = (int)(income * (1f - deductible));
        var bracket = Framework.Utils.GetTaxBracket(taxable);
        var due = (int)Math.Round(income * bracket);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {Game1.game1.GetPrecedingSeason()} season:" +
            $"\n\t- Total income: {income}g" +
            CurrentCulture($"\n\t- Tax deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Total due income tax: {due}g."
        );
        return due;
    }
}