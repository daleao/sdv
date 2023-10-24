namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var toDebit = Game1.player.Read<int>(DataKeys.LatestAmountWithheld);
        if (toDebit <= 0)
        {
            return;
        }

        Game1.player.Money -= toDebit;
        Game1.addHUDMessage(
            new HUDMessage(
                I18n.Hud_Debt_Debit(toDebit.ToString()),
                HUDMessage.newQuest_type) { timeLeft = HUDMessage.defaultTime * 2 });
        Game1.player.Write(DataKeys.LatestAmountWithheld, string.Empty);
        this.Disable();
    }
}
