namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TaxDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TaxDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var player = Game1.player;
        if (Data.ReadAs<int>(player, DataKeys.OvernightDebit) is var debited and > 0)
        {
            Game1.addHUDMessage(
                new HUDMessage(
                        I18n.Hud_Tax_Debit(debited.ToString()),
                        HUDMessage.newQuest_type)
                    { timeLeft = HUDMessage.defaultTime * 2 });
            Data.Write(player, DataKeys.OvernightDebit, string.Empty);
        }

        if (Data.ReadAs<int>(player, DataKeys.Withheld) is var withheld and > 0)
        {
            player.Money -= withheld;
            Game1.addHUDMessage(
                new HUDMessage(
                        I18n.Hud_Debt_Debit(withheld.ToString()),
                        HUDMessage.newQuest_type)
                    { timeLeft = HUDMessage.defaultTime * 2 });
            Data.Write(player, DataKeys.Withheld, string.Empty);
        }

        this.Disable();
    }
}
