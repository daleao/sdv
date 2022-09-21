namespace DaLion.Stardew.Taxes.Commands;

#region using directives

using System;
using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Taxes.Framework;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class DoTaxesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DoTaxesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DoTaxesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "do_taxes", "do" };

    /// <inheritdoc />
    public override string Documentation =>
        "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        var forClosingSeason = Game1.dayOfMonth == 1;
        var income = player.Read<int>("SeasonIncome");
        var deductible = ModEntry.ProfessionsApi is not null && player.professions.Contains(Farmer.mariner)
            ? forClosingSeason
                ? player.Read<float>("DeductionPct")
                : ModEntry.ProfessionsApi.GetConservationistProjectedTaxBonus(player)
            : 0f;
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
        var debt = player.Read<int>("DebtOutstanding");
        Log.I(
            "Accounting " + (forClosingSeason ? "report" : "projections") + " for the " +
            (forClosingSeason ? "closing" : "current") + " season:" +
            $"\n\t- Income (season-to-date): {income}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductible:0%}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Current tax bracket: {bracket:0%}") +
            $"\n\t- Due income tax: {dueI}g." +
            $"\n\t- Outstanding debt: {debt}g." +
            $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}.");
    }
}
