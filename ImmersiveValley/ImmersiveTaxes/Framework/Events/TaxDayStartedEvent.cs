namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using DaLion.Common.Events;
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
        if (ModEntry.State.ToDebit > 0)
        {
            Game1.player.Money -= ModEntry.State.ToDebit;
            Game1.addHUDMessage(
                new HUDMessage(
                    ModEntry.i18n.Get("debt.debit", new { amount = ModEntry.State.ToDebit }),
                    HUDMessage.newQuest_type) { timeLeft = HUDMessage.defaultTime * 2 });
            ModEntry.State.ToDebit = 0;
        }
    }
}
