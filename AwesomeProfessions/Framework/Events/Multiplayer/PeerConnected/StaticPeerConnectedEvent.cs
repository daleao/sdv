using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.Multiplayer.ModMessageReceived;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.PeerConnected;

[UsedImplicitly]
internal class StaticPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    public override void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        if (Context.IsMainPlayer) ModEntry.Subscriber.Subscribe(new DataModMessageReceivedEvent());
    }
}