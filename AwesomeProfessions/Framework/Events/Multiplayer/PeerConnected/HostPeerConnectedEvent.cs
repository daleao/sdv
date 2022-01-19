namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e)
    {
        EventManager.Enable(typeof(ToggledSuperModeModMessageReceivedEvent),
            typeof(RequestDataUpdateModMessageReceivedEvent), typeof(RequestGlobalEventEnableModMessageReceivedEvent));

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession("Conservationist"))
            EventManager.Enable(typeof(GlobalConservationistDayEndingEvent));
    }
}