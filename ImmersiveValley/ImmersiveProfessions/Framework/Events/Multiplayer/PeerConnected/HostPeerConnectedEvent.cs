namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;
using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostPeerConnectedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        Manager.Hook<ToggledUltimateModMessageReceivedEvent>();
        Manager.Hook<RequestGlobalEventModMessageReceivedEvent>();
        Manager.Hook<RequestUpdateDataModMessageReceivedEvent>();
        Manager.Hook<RequestUpdateHostStateModMessageReceivedEvent>();

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Conservationist))
            Manager.Hook<HostConservationismDayEndingEvent>();
    }
}