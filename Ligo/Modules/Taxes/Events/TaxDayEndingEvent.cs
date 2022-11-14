namespace DaLion.Ligo.Modules.Taxes.Events;

#region using directives

using System.Globalization;
using System.Linq;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Taxes.Extensions;
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
        if (!Context.IsMainPlayer)
        {
            return;
        }

        foreach (var farmer in Game1.getAllFarmers())
        {
            if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
            {
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");
            }

            var amountSold = Game1.getFarm().getShippingBin(farmer).Sum(item =>
                item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
            if (amountSold > 0 && !farmer.hasOrWillReceiveMail($"{ModEntry.Manifest.UniqueID}/TaxIntro"))
            {
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");
            }

            var dayIncome = amountSold;
            switch (Game1.dayOfMonth)
            {
                case 28 when ModEntry.Config.EnableProfessions && farmer.professions.Contains(Farmer.mariner):
                    {
                        var deductible = farmer.GetConservationistPriceMultiplier() - 1;
                        if (deductible <= 0f)
                        {
                            break;
                        }

                        farmer.Write(DataFields.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
                        ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                        farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxDeduction");
                        Log.I(
                            FormattableString.CurrentCulture(
                                $"Farmer {farmer.Name} is eligible for tax deductions of {deductible:0%}.") +
                            (deductible >= 1f
                                ? $" No taxes will be charged for {Game1.currentSeason}."
                                : string.Empty) +
                            " An FRS deduction notice has been posted for tomorrow.");
                        break;
                    }

                case 1 when farmer.Read<float>(DataFields.PercentDeductions) < 1f:
                    {
                        if (Game1.currentSeason == "spring" && Game1.year == 1)
                        {
                            break;
                        }

                        var amountDue = farmer.DoTaxes();
                        ModEntry.State.Taxes.LatestAmountDue = amountDue;
                        if (amountDue <= 0)
                        {
                            break;
                        }

                        int amountPaid;
                        if (farmer.Money + dayIncome >= amountDue)
                        {
                            farmer.Money -= amountDue;
                            amountPaid = amountDue;
                            amountDue = 0;
                            ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                            farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxNotice");
                            Log.I("Amount due has been paid in full." +
                                  " An FRS taxation notice has been posted for tomorrow.");
                        }
                        else
                        {
                            farmer.Money = 0;
                            amountPaid = farmer.Money + dayIncome;
                            amountDue -= amountPaid;
                            farmer.Increment(DataFields.DebtOutstanding, amountDue);
                            ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
                            farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxOutstanding");
                            Log.I(
                                $"{farmer.Name} did not carry enough funds to cover the amount due." +
                                $"\n\t- Amount charged: {amountPaid}g" +
                                $"\n\t- Outstanding debt: {amountDue}g." +
                                " An FRS collection notice has been posted for tomorrow.");
                        }

                        farmer.Write(DataFields.SeasonIncome, "0");
                        break;
                    }
            }

            var debtOutstanding = farmer.Read<int>(DataFields.DebtOutstanding);
            if (debtOutstanding > 0)
            {
                if (dayIncome >= debtOutstanding)
                {
                    dayIncome -= debtOutstanding;
                    debtOutstanding = 0;
                    Log.I(
                        $"{farmer.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
                }
                else
                {
                    debtOutstanding -= dayIncome;
                    var interest = (int)Math.Round(debtOutstanding * ModEntry.Config.Taxes.AnnualInterest / 112f);
                    debtOutstanding += interest;
                    Log.I(
                        $"{farmer.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
                    dayIncome = 0;
                }

                var debit = amountSold - dayIncome;
                ModEntry.State.Taxes.LatestDebit = debit;
                farmer.Write(DataFields.DebtOutstanding, debtOutstanding.ToString());
                ModEntry.Events.Enable<TaxDayStartedEvent>();
            }

            farmer.Increment(DataFields.SeasonIncome, dayIncome);
        }
    }
}
