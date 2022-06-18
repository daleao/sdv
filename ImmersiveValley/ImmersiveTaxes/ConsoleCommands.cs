namespace DaLion.Stardew.Taxes;

#region using directives

using static System.FormattableString;

using System;
using StardewModdingAPI;
using StardewValley;

using Extensions;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(this ICommandHelper helper)
    {
        helper.Add("do_taxes", "Check accounting stats for the current season-to-date.", DoAccounting);
    }

    #region command handlers

    public static void DoAccounting(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        var income = Game1.player.ReadDataAs<int>(ModData.SeasonIncome);
        var deductible = ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.mariner)
            ? ModEntry.ProfessionsAPI.GetConservationistProjectedTaxBonus(Game1.player)
            : 0f;
        var taxable = (int) (income * (1f - deductible));
        var bracket = Framework.Utility.GetTaxBracket(taxable);
        var due = (int) Math.Round(income * bracket);
        Log.I(
            "Accounting projections for the current season:" +
            $"\n\t- Income (season-to-date): {income}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Current tax bracket: {bracket:p0}") +
            $"\n\t- Due income tax: {due}g." +
            $"\n\t- Total projected income tax: {due * 28 / Game1.dayOfMonth}g."
        );
    }

    #endregion command handlers
}