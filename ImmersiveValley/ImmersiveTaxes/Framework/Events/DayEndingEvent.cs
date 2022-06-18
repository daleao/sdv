namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using System;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayEnding"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal sealed class DayEndingEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayEnding += OnDayEnding;
        Log.D("[Taxes] Hooked DayEnding event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayEnding -= OnDayEnding;
        Log.D("[Taxes] Unhooked DayEnding event.");
    }

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth == 1 && Game1.currentSeason == "spring" && Game1.year == 1)
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        var dayIncome = Game1.getFarm().getShippingBin(Game1.player).OfType<SObject>()
            .Sum(@object => @object.sellToStorePrice() * @object.Stack);
        if (dayIncome > 0 && !Game1.player.hasOrWillReceiveMail($"{ModEntry.Manifest.UniqueID}/TaxIntro"))
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        var debtOutstanding = Game1.player.ReadDataAs<int>(ModData.DebtOutstanding);
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
                debtOutstanding += (int) Math.Round(debtOutstanding * ModEntry.Config.AnnualInterest / 112f);
                Log.I($"{Game1.player.Name}'s outstanding debt has accrued interest and is now worth {debtOutstanding}g.");
                dayIncome = 0;
            }

            Game1.player.Money = dayIncome;
            Game1.player.WriteData(ModData.DebtOutstanding, debtOutstanding.ToString());
        }

        switch (Game1.dayOfMonth)
        {
            case 28 when ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.mariner):
            {
                var deductible = ModEntry.ProfessionsAPI.GetConservationistEffectiveTaxBonus(Game1.player);
                if (deductible > 0f)
                {
                    Game1.player.WriteData(ModData.DeductionPct, deductible.ToString(CultureInfo.InvariantCulture));
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                    Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxDeduction");
                    Log.I(
                        FormattableString.CurrentCulture($"Farmer {Game1.player.Name} is eligible for tax deductions of {deductible:p0}.") +
                        (deductible >= 1f ? $" No taxes will be charged for {Game1.game1.GetPrecedingSeason()}." : string.Empty) +
                        " An FRS deduction notice has been posted for tomorrow.");
                }

                break;
            }
            case 1 when Game1.player.ReadDataAs<float>(ModData.DeductionPct) < 1f:
            {
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
                        Game1.player.IncrementData(ModData.DebtOutstanding, amountDue);
                        ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxOutstanding");
                        Log.I(
                            $"{Game1.player.Name} did not carry enough funds to cover the amount due." +
                            $"\n\t- Amount charged: {amountPaid}g" +
                            $"\n\t- Outstanding debt: {amountDue}g." +
                            " An FRS collection notice has been posted for tomorrow.");
                    }

                    Game1.player.WriteData(ModData.SeasonIncome, "0");
                }

                break;
            }
        }

        Game1.player.IncrementData(ModData.SeasonIncome, dayIncome);
    }
}