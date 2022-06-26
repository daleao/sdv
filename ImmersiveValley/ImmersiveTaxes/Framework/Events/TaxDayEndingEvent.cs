namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxDayEndingEvent : DayEndingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayEndingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1)
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        var dayIncome = Game1.getFarm().getShippingBin(Game1.player).Sum(item =>
            item is SObject @object ? @object.sellToStorePrice() * @object.Stack : item.salePrice() / 2);
        if (dayIncome > 0 && !Game1.player.hasOrWillReceiveMail($"{ModEntry.Manifest.UniqueID}/TaxIntro"))
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        var debtOutstanding = ModDataIO.ReadDataAs<int>(Game1.player, ModData.DebtOutstanding.ToString());
        if (debtOutstanding > 0)
        {
            if (dayIncome >= debtOutstanding)
            {
                dayIncome -= debtOutstanding;
                debtOutstanding = 0;
                Log.I(
                    $"{Game1.player.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
            }
            else
            {
                debtOutstanding -= dayIncome;
                debtOutstanding += (int)Math.Round(debtOutstanding * ModEntry.Config.AnnualInterest / 112f);
                Log.I(
                    $"{Game1.player.Name}'s outstanding debt has accrued interest and is now worth {debtOutstanding}g.");
                dayIncome = 0;
            }

            Game1.player.Money = dayIncome;
            ModDataIO.WriteData(Game1.player, ModData.DebtOutstanding.ToString(), debtOutstanding.ToString());
        }

        switch (Game1.dayOfMonth)
        {
            case 28 when ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.mariner):
                {
                    var deductible = ModEntry.ProfessionsAPI.GetConservationistEffectiveTaxBonus(Game1.player);
                    if (deductible > 0f)
                    {
                        ModDataIO.WriteData(Game1.player, ModData.DeductionPct.ToString(),
                            deductible.ToString(CultureInfo.InvariantCulture));
                        ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxDeduction");
                        Log.I(
                            FormattableString.CurrentCulture(
                                $"Farmer {Game1.player.Name} is eligible for tax deductions of {deductible:p0}.") +
                            (deductible >= 1f
                                ? $" No taxes will be charged for {Game1.game1.GetPrecedingSeason()}."
                                : string.Empty) +
                            " An FRS deduction notice has been posted for tomorrow.");
                    }

                    break;
                }
            case 1 when ModDataIO.ReadDataAs<float>(Game1.player, ModData.DeductionPct.ToString()) < 1f:
                {
                    if (Game1.currentSeason == "spring" && Game1.year == 1) return;

                    var amountDue = Game1.player.DoTaxes();
                    ModEntry.LatestAmountDue.Value = amountDue;
                    if (amountDue > 0)
                    {
                        int amountPaid;
                        if (Game1.player.Money >= amountDue)
                        {
                            Game1.player.Money -= amountDue;
                            amountPaid = amountDue;
                            amountDue = 0;
                            ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxNotice");
                            Log.I("Amount due has been paid in full." +
                                  " An FRS taxation notice has been posted for tomorrow.");
                        }
                        else
                        {
                            Game1.player.Money = 0;
                            amountPaid = Game1.player.Money;
                            amountDue -= amountPaid;
                            ModDataIO.IncrementData(Game1.player, ModData.DebtOutstanding.ToString(), amountDue);
                            ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxOutstanding");
                            Log.I(
                                $"{Game1.player.Name} did not carry enough funds to cover the amount due." +
                                $"\n\t- Amount charged: {amountPaid}g" +
                                $"\n\t- Outstanding debt: {amountDue}g." +
                                " An FRS collection notice has been posted for tomorrow.");
                        }

                        ModDataIO.WriteData(Game1.player, ModData.SeasonIncome.ToString(), "0");
                    }

                    break;
                }
        }

        ModDataIO.IncrementData(Game1.player, ModData.SeasonIncome.ToString(), dayIncome);
    }
}