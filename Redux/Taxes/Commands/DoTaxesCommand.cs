namespace DaLion.Redux.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
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
    public override string[] Triggers { get; } = { "do" };

    /// <inheritdoc />
    public override string Documentation =>
        "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        var forClosingSeason = Game1.dayOfMonth == 1;
        var seasonIncome = player.Read<int>(DataFields.SeasonIncome);
        var deductibleExpenses = player.Read<int>(DataFields.BusinessExpenses);
        var deductiblePct = ModEntry.Config.EnableProfessions && player.professions.Contains(Farmer.mariner)
            ? forClosingSeason
                ? player.Read<float>(DataFields.PercentDeductions)
                : player.Read<int>(DataFields.ConservationistTrashCollectedThisSeason) / ModEntry.Config.Professions.TrashNeededPerTaxBonusPct / 100f
            : 0f;
        var taxable = (int)((seasonIncome - deductibleExpenses) * (1f - deductiblePct));

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
        var debt = player.Read<int>(DataFields.DebtOutstanding);
        Log.I(
            "Accounting " + (forClosingSeason ? "report" : "projections") + " for the " +
            (forClosingSeason ? "closing" : "current") + " season:" +
            $"\n\t- Income (season-to-date): {seasonIncome}g" +
            $"\n\t- Business expenses: {deductibleExpenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductiblePct:0%}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Current tax bracket: {bracket:0%}") +
            $"\n\t- Due amount: {dueI}g." +
            $"\n\t- Outstanding debt: {debt}g." +
            $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}.");
    }
}
