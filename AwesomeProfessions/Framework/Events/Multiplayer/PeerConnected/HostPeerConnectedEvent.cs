using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.GameLoop;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer;

[UsedImplicitly]
internal class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e)
    {
        ModEntry.EventManager.Enable(typeof(ToggledSuperModeModMessageReceivedEvent),
            typeof(RequestDataUpdateModMessageReceivedEvent), typeof(RequestGlobalEventEnableModMessageReceivedEvent));

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession("Conservationist"))
            ModEntry.EventManager.Enable(typeof(GlobalConservationistDayEndingEvent));
    }
}