namespace DaLion.Ligo.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Events;
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
        if (ModEntry.State.Taxes.LatestDebit <= 0)
        {
            return;
        }

        Game1.player.Money -= ModEntry.State.Taxes.LatestDebit;
        Game1.addHUDMessage(
            new HUDMessage(
                ModEntry.i18n.Get("debt.debit", new { amount = ModEntry.State.Taxes.LatestDebit }),
                HUDMessage.newQuest_type) { timeLeft = HUDMessage.defaultTime * 2 });
        ModEntry.State.Taxes.LatestDebit = 0;
        this.Disable();
    }
}
