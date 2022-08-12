namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxDayStartedEvent : DayStartedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayStartedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (ModEntry.LatestAmountDebited.Value > 0)
            Game1.addHUDMessage(
                new(ModEntry.i18n.Get("debt.debit", new { amount = ModEntry.LatestAmountDebited }), HUDMessage.newQuest_type)
                    { timeLeft = HUDMessage.defaultTime * 2 });
    }
}