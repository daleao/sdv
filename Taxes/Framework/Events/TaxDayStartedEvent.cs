namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayStartedEvent(EventManager? manager = null)
        : base(manager ?? TaxesMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var player = Game1.player;
        var toDebit = Data.ReadAs<int>(player, DataKeys.LatestAmountWithheld);
        if (toDebit <= 0)
        {
            return;
        }

        player.Money -= toDebit;
        Game1.addHUDMessage(
            new HUDMessage(
                I18n.Hud_Debt_Debit(toDebit.ToString()),
                HUDMessage.newQuest_type) { timeLeft = HUDMessage.defaultTime * 2 });
        Data.Write(player, DataKeys.LatestAmountWithheld, string.Empty);
        this.Disable();
    }
}
