namespace DaLion.Taxes.Framework.Events;

#region using directives

using System.Globalization;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Taxes.Framework.Integrations;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TaxDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var taxpayer = Game1.player;
        if (!taxpayer.ShouldPayTaxes())
        {
            // clear any outdated data just in case
            Data.Write(taxpayer, DataKeys.SeasonIncome, null);
            Data.Write(taxpayer, DataKeys.BusinessExpenses, null);
            Data.Write(taxpayer, DataKeys.PercentDeductions, null);
            Data.Write(taxpayer, DataKeys.DebtOutstanding, null);
            return;
        }

        if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
        {
            PostalService.Send(Mail.LewisIntro);
            PostalService.Send(Mail.FrsIntro);
        }

        var amountSold = Game1.game1.GetTotalSoldByPlayer(taxpayer);
        if (amountSold > 0 && !PostalService.HasSent(Mail.FrsIntro))
        {
            PostalService.Send(Mail.FrsIntro);
        }

        Log.D(
            amountSold > 0
                ? $"{Game1.player} sold items worth a total of {amountSold}g on day {Game1.dayOfMonth} of {Game1.currentSeason}."
                : $"No items were sold on day {Game1.dayOfMonth} of {Game1.currentSeason}.");

        var dayIncome = amountSold;
        switch (Game1.dayOfMonth)
        {
            // handle Conservationist profession
            case 28 when ProfessionsIntegration.IsValueCreated && taxpayer.professions.Contains(Farmer.mariner):
            {
                CheckDeductions(taxpayer);
                goto default;
            }

            case 1:
            {
                if (Game1.currentSeason == "spring" && Game1.year == 1)
                {
                    break;
                }

                CheckIncomeStatement(taxpayer, ref dayIncome);
                if (taxpayer.IsMainPlayer && Game1.currentSeason == "spring")
                {
                    CheckPropertyStatement(taxpayer, ref dayIncome);
                }

                goto default;
            }

            case 8:
            case 22:
            {
                if (taxpayer.IsMainPlayer)
                {
                    Game1.getFarm().Appraise();
                }

                goto default;
            }

            default:
            {
                CheckOutstanding(taxpayer, ref dayIncome);
                break;
            }
        }

        if (dayIncome < amountSold)
        {
            Data.Write(taxpayer, DataKeys.LatestAmountWithheld, (amountSold - dayIncome).ToString());
            TaxesMod.EventManager.Enable<TaxDayStartedEvent>();
            Log.T(dayIncome > 0
                ? $"Actual income was decreased by {amountSold - dayIncome}g after debts and payments."
                : "Day's income was entirely consumed by debts and payments.");
        }

        Data.Increment(taxpayer, DataKeys.SeasonIncome, dayIncome);
    }

    private static void CheckDeductions(Farmer taxpayer)
    {
        var professionsApi = ProfessionsIntegration.Instance?.ModApi;
        if (professionsApi is null)
        {
            return;
        }

        var deductible = professionsApi.GetConservationistTaxDeduction();
        if (deductible <= 0f)
        {
            return;
        }

        deductible = Math.Min(
            deductible,
            professionsApi.GetConfig().ConservationistTaxDeductionCeiling);

        Data.Write(taxpayer, DataKeys.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
        Data.Write(taxpayer, DataKeys.LatestTaxDeductions, deductible.ToString(CultureInfo.InvariantCulture));
        PostalService.Send(Mail.FrsDeduction);
        Log.I(
            FormattableString.CurrentCulture(
                $"Farmer {taxpayer.Name} is eligible for income tax deductions of {deductible:0.0%}.") +
            (deductible >= 1f
                ? $" No income taxes will be charged for {Game1.currentSeason}."
                : string.Empty));
    }

    private static void CheckIncomeStatement(Farmer taxpayer, ref int dayIncome)
    {
        var (amountDue, _, _, _, _) = RevenueService.CalculateTaxes(taxpayer);
        Data.Write(taxpayer, DataKeys.LatestDueIncomeTax, amountDue.ToString());
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            PostalService.Send(Mail.FrsNotice);
            Log.I("Income tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue >= dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * Config.IncomeTaxLatenessFine), 100);
                Data.Increment(taxpayer, DataKeys.DebtOutstanding, outstanding + penalties);
                Data.Write(taxpayer, DataKeys.LatestOutstandingIncomeTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.FrsOutstanding);
                Log.I(
                    $"{taxpayer.Name} did not carry enough funds to cover the income tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        Data.Write(taxpayer, DataKeys.SeasonIncome, "0");
        Data.Write(taxpayer, DataKeys.BusinessExpenses, "0");
    }

    private static void CheckPropertyStatement(Farmer taxpayer, ref int dayIncome)
    {
        var farm = Game1.getFarm();
        var agricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var livestockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var buildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        var usableTiles = Data.ReadAs(farm, DataKeys.UsableTiles, -1);
        if (usableTiles < 0)
        {
            var origin = farm.GetMainFarmHouseEntry();
            usableTiles = origin.FloodFill(
                farm.Map.Layers[0].TileWidth,
                farm.Map.Layers[0].TileHeight,
                p => farm.doesTileHaveProperty(p.X, p.Y, "Diggable", "Back") is not null).Count;
            Data.Write(farm, DataKeys.UsableTiles, usableTiles.ToString());
        }

        var currentUsePct = Data.ReadAs<float>(farm, DataKeys.UsedTiles) / usableTiles;
        var amountDue = (int)(((agricultureValue + livestockValue) * currentUsePct * Config.UsedTileTaxRate) +
                              ((agricultureValue + livestockValue) * (1f - currentUsePct) * Config.UnusedTileTaxRate) +
                              (buildingValue * Config.BuildingTaxRate));
        Data.Write(taxpayer, DataKeys.LatestDuePropertyTax, amountDue.ToString());
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            PostalService.Send(Mail.LewisNotice);
            Log.I("Property tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue > dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * Config.PropertyTaxLatenessFine), 500);
                Data.Increment(taxpayer, DataKeys.DebtOutstanding, outstanding + penalties);
                Data.Write(taxpayer, DataKeys.LatestOutstandingPropertyTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.LewisOutstanding);
                Log.I(
                    $"{taxpayer.Name} did not carry enough funds to cover the property tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        Data.Write(farm, DataKeys.AgricultureValue, "0");
        Data.Write(farm, DataKeys.LivestockValue, "0");
        Data.Write(farm, DataKeys.BuildingValue, "0");
        Data.Write(farm, DataKeys.UsedTiles, "0");
    }

    private static void CheckOutstanding(Farmer taxpayer, ref int dayIncome)
    {
        var debtOutstanding = Data.ReadAs<int>(taxpayer, DataKeys.DebtOutstanding);
        if (debtOutstanding <= 0)
        {
            return;
        }

        // only seize shipping bin income
        int withheld;
        if (dayIncome >= debtOutstanding)
        {
            withheld = debtOutstanding;
            debtOutstanding = 0;
            Log.I(
                $"{taxpayer.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
        }
        else
        {
            withheld = dayIncome;
            debtOutstanding -= withheld;
            var interest = (int)Math.Round(debtOutstanding * Config.AnnualInterest / 112f);
            debtOutstanding += interest;
            Log.I(
                $"{taxpayer.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
        }

        dayIncome -= withheld;
        Data.Write(taxpayer, DataKeys.DebtOutstanding, debtOutstanding.ToString());
    }
}
