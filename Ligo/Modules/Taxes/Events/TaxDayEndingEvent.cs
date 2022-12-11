namespace DaLion.Ligo.Modules.Taxes.Events;

#region using directives

using System.Globalization;
using System.Linq;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Taxes.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

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
            player.mailForTomorrow.Add($"{Manifest.UniqueID}/TaxIntro");
        }

        var amountSold = Game1.getFarm().getShippingBin(player).Sum(item =>
            item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
        if (amountSold > 0 && !player.hasOrWillReceiveMail($"{Manifest.UniqueID}/TaxIntro"))
        {
            player.mailForTomorrow.Add($"{Manifest.UniqueID}/TaxIntro");
        }

        var dayIncome = amountSold;
        switch (Game1.dayOfMonth)
        {
            case 28 when Config.EnableProfessions && player.professions.Contains(Farmer.mariner):
            {
                var deductible = player.GetConservationistPriceMultiplier() - 1;
                if (deductible <= 0f)
                {
                    break;
                }

                player.Write(DataFields.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
                ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                player.mailForTomorrow.Add($"{Manifest.UniqueID}/TaxDeduction");
                Log.I(
                    FormattableString.CurrentCulture(
                        $"Farmer {player.Name} is eligible for tax deductions of {deductible:0%}.") +
                    (deductible >= 1f
                        ? $" No taxes will be charged for {Game1.currentSeason}."
                        : string.Empty) +
                    " An FRS deduction notice has been posted for tomorrow.");
                break;
            }

            case 1 when player.Read<float>(DataFields.PercentDeductions) < 1f:
            {
                if (Game1.currentSeason == "spring" && Game1.year == 1)
                {
                    break;
                }

                var amountDue = RevenueService.CalculateTaxes(player);
                player.Set_LatestDue(amountDue);
                if (amountDue <= 0)
                {
                    break;
                }

                int amountPaid;
                if (player.Money + dayIncome >= amountDue)
                {
                    player.Money -= amountDue;
                    amountPaid = amountDue;
                    amountDue = 0;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                    player.mailForTomorrow.Add($"{Manifest.UniqueID}/TaxNotice");
                    Log.I("Amount due has been paid in full." +
                          " An FRS taxation notice has been posted for tomorrow.");
                }
                else
                {
                    player.Money = 0;
                    amountPaid = player.Money + dayIncome;
                    amountDue -= amountPaid;
                    player.Increment(DataFields.DebtOutstanding, amountDue);
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                    player.mailForTomorrow.Add($"{Manifest.UniqueID}/TaxOutstanding");
                    Log.I(
                        $"{player.Name} did not carry enough funds to cover the amount due." +
                        $"\n\t- Amount charged: {amountPaid}g" +
                        $"\n\t- Outstanding debt: {amountDue}g." +
                        " An FRS collection notice has been posted for tomorrow.");
                }

                player.Write(DataFields.SeasonIncome, "0");
                break;
            }
        }

        var debtOutstanding = player.Read<int>(DataFields.DebtOutstanding);
        if (debtOutstanding > 0)
        {
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
            player.Set_LatestCharge(toDebit);
            player.Write(DataFields.DebtOutstanding, debtOutstanding.ToString());
            this.Manager.Enable<TaxDayStartedEvent>();
        }

        player.Increment(DataFields.SeasonIncome, dayIncome);
    }
}
