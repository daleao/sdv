namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;
using Content;
using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e)
    {
        ModEntry.EventManager.Hook<ToggledUltimateModMessageReceivedEvent>();
        ModEntry.EventManager.Hook<RequestGlobalEventModMessageReceivedEvent>();
        ModEntry.EventManager.Hook<RequestUpdateDataModMessageReceivedEvent>();
        ModEntry.EventManager.Hook<RequestUpdateHostStateModMessageReceivedEvent>();

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Conservationist))
            ModEntry.EventManager.Hook<HostConservationismDayEndingEvent>();
    }
}