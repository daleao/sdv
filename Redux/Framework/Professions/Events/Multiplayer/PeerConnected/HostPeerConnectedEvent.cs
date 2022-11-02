namespace DaLion.Redux.Framework.Professions.Events.Multiplayer;

#region using directives

using DaLion.Redux.Framework.Professions.Events.GameLoop;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostPeerConnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostPeerConnectedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Conservationist))
        {
            this.Manager.Enable<ConservationismDayEndingEvent>();
        }
    }
}
