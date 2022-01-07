using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.ModMessageReceived;

internal class RequestEventSubscriptionModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestEventSubscription")) return;

        var which = e.ReadAs<string>();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            ModEntry.Log($"Unknown player {e.FromPlayerID} requested {which} event subscription.", LogLevel.Warn);
            return;
        }

        switch (which)
        {
            case "Conservationist":
                ModEntry.Log($"Player {e.FromPlayerID} requested {which} event subscription.", LogLevel.Trace);
                ModEntry.Subscriber.SubscribeTo(new GlobalConservationistDayEndingEvent());
                break;
        }
    }
}