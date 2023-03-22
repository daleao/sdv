namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using System.Globalization;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using StardewModdingAPI.Events;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var player = Game1.player;
        if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
        {
            PostalService.Send(Mail.FrsIntro);
        }

        var amountSold = Game1.getFarm().getShippingBin(player).Sum(item =>
            item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
        Utility.ForAllLocations(location =>
        {
            amountSold += location.Objects.Values
                .OfType<Chest>()
                .Where(c => c.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
                .Sum(miniBin => miniBin
                    .GetItemsForPlayer(player.UniqueMultiplayerID)
                    .Sum(item => item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2));
        });

        if (amountSold > 0 && !PostalService.HasSent(Mail.FrsIntro))
        {
            PostalService.Send(Mail.FrsIntro);
        }

        Log.D(
            amountSold > 0
                ? $"[Taxes]: {Game1.player} sold items worth a total of {amountSold}g on day {Game1.dayOfMonth} of {Game1.currentSeason}."
                : $"[Taxes]: No items were sold on day {Game1.dayOfMonth} of {Game1.currentSeason}.");

        var dayIncome = amountSold;
        switch (Game1.dayOfMonth)
        {
            case 28 when ProfessionsModule.IsEnabled && player.professions.Contains(Farmer.mariner):
            {
                var deductible = player.GetConservationistPriceMultiplier() - 1;
                if (deductible <= 0f)
                {
                    break;
                }

                player.Write(DataKeys.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
                TaxesModule.State.LatestTaxDeductions = deductible;
                PostalService.Send(Mail.FrsDeduction);
                Log.I(
                    FormattableString.CurrentCulture(
                        $"Farmer {player.Name} is eligible for income tax deductions of {deductible:0%}.") +
                    (deductible >= 1f
                        ? $" No income taxes will be charged for {Game1.currentSeason}."
                        : string.Empty));

                goto default;
            }

            case 1:
            {
                if (Game1.currentSeason == "spring" && Game1.year == 1)
                {
                    break;
                }

                var debtOutstanding = player.Read<int>(DataKeys.DebtOutstanding);
                if (debtOutstanding > 0)
                {
                    var penalties = Math.Min((int)(debtOutstanding * 0.05f), 100);
                    Log.I(
                        $"Outstanding debt in the amount of {debtOutstanding}g has accrued additional penalties in the amount of {penalties}g.");
                    player.Write(DataKeys.DebtOutstanding, (debtOutstanding + penalties).ToString());
                }

                // calculate income tax
                var amountDue = RevenueService.CalculateTaxes(player);
                TaxesModule.State.LatestDueIncomeTax = amountDue;
                if (amountDue > 0)
                {
                    int amountPaid;
                    if (player.Money + dayIncome >= amountDue)
                    {
                        player.Money -= amountDue;
                        amountPaid = amountDue;
                        amountDue = 0;
                        PostalService.Send(Mail.FrsNotice);
                        Log.I("Income tax due was paid in full.");
                    }
                    else
                    {
                        amountPaid = player.Money + dayIncome;
                        amountDue -= amountPaid;
                        player.Money = 0;

                        var penalties = Math.Min((int)(amountDue * TaxesModule.Config.IncomeTaxLatenessFine), 100);
                        player.Increment(DataKeys.DebtOutstanding, amountDue + penalties);
                        TaxesModule.State.LatestOutstandingIncomeTax = amountDue + penalties;
                        PostalService.Send(Mail.FrsOutstanding);
                        Log.I(
                            $"{player.Name} did not carry enough funds to cover the income tax due." +
                            $"\n\t- Amount charged: {amountPaid}g" +
                            $"\n\t- Outstanding debt: {amountDue}g (+{penalties}g in penalties).");
                    }

                    player.Write(DataKeys.SeasonIncome, "0");
                    player.Write(DataKeys.BusinessExpenses, "0");
                }

                // calculate property tax
                if (!player.IsMainPlayer)
                {
                    return;
                }

                var farm = Game1.getFarm();
                var agricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
                var livestockValue = farm.Read<int>(DataKeys.LivestockValue);
                var buildingValue = farm.Read<int>(DataKeys.BuildingValue);
                var usableTiles = farm.Read(DataKeys.UsableTiles, -1);
                if (usableTiles < 0)
                {
                    var origin = farm.GetMainFarmHouseEntry();
                    usableTiles = origin.FloodFill(
                        farm.Map.DisplayWidth,
                        farm.Map.DisplayHeight,
                        p => farm.doesTileHaveProperty(p.X, p.Y, "Diggable", "Back") is not null).Count;
                    farm.Write(DataKeys.UsableTiles, usableTiles.ToString());
                }

                var currentUsePct = farm.Read<float>(DataKeys.UsedTiles) / usableTiles;
                amountDue = (int)(((agricultureValue + livestockValue) * currentUsePct * TaxesModule.Config.UsedTileTaxRate) +
                                  ((agricultureValue + livestockValue) * (1f - currentUsePct) * TaxesModule.Config.UnusedTileTaxRate) +
                                  (buildingValue * TaxesModule.Config.BuildingTaxRate));
                TaxesModule.State.LatestDuePropertyTax = amountDue;
                if (amountDue > 0)
                {
                    int amountPaid;
                    if (player.Money + dayIncome >= amountDue)
                    {
                        player.Money -= amountDue;
                        amountPaid = amountDue;
                        amountDue = 0;
                        PostalService.Send(Mail.LewisNotice);
                        Log.I("Property tax due was paid in full.");
                    }
                    else
                    {
                        amountPaid = player.Money + dayIncome;
                        amountDue -= amountPaid;
                        player.Money = 0;

                        var penalties = Math.Min((int)(amountDue * TaxesModule.Config.PropertyTaxLatenessFine), 100);
                        player.Increment(DataKeys.DebtOutstanding, amountDue + penalties);
                        TaxesModule.State.LatestOutstandingPropertyTax = amountDue + penalties;
                        PostalService.Send(Mail.LewisOutstanding);
                        Log.I(
                            $"{player.Name} did not carry enough funds to cover the property tax due." +
                            $"\n\t- Amount charged: {amountPaid}g" +
                            $"\n\t- Outstanding debt: {amountDue}g (+{penalties}g in penalties).");
                    }

                    farm.Write(DataKeys.AgricultureValue, "0");
                    farm.Write(DataKeys.LivestockValue, "0");
                    farm.Write(DataKeys.BuildingValue, "0");
                }

                goto default;
            }

            case 8:
            case 22:
            {
                if (player.IsMainPlayer)
                {
                    Game1.getFarm().Appraise();
                }

                goto default;
            }

            default:
            {
                var debtOutstanding = player.Read<int>(DataKeys.DebtOutstanding);
                if (debtOutstanding <= 0)
                {
                    break;
                }

                if (dayIncome >= debtOutstanding)
                {
                    dayIncome -= debtOutstanding;
                    debtOutstanding = 0;
                    Log.I(
                        $"{player.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
                }
                else
                {
                    debtOutstanding -= dayIncome;
                    var interest = (int)Math.Round(debtOutstanding * TaxesModule.Config.AnnualInterest / 112f);
                    debtOutstanding += interest;
                    Log.I(
                        $"{player.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
                    dayIncome = 0;
                }

                var toDebit = amountSold - dayIncome;
                TaxesModule.State.LatestAmountWithheld = toDebit;
                player.Write(DataKeys.DebtOutstanding, debtOutstanding.ToString());
                this.Manager.Enable<TaxDayStartedEvent>();
            }

            break;
        }

        player.Increment(DataKeys.SeasonIncome, dayIncome);
        Log.T(
            $"[Taxes]: Actual income was increased by {dayIncome}g after debts.");
    }

    private static void CheckDeduction()
    {

    }
}
