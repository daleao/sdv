using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;
using TheLion.Stardew.Professions.Framework.Events.Multiplayer.ModMessageReceived;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.PeerConnected;

[UsedImplicitly]
internal class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    public override void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        ModEntry.Subscriber.SubscribeTo(new ToggledSuperModeModMessageReceivedEvent(),
            new RequestDataUpdateModMessageReceivedEvent(), new RequestEventSubscriptionModMessageReceivedEvent());

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession("Conservationist"))
            ModEntry.Subscriber.SubscribeTo(new GlobalConservationistDayEndingEvent());
    }
}