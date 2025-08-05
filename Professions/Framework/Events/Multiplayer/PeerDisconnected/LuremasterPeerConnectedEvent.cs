namespace DaLion.Professions.Framework.Events.Multiplayer.PeerConnected;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterPeerConnectedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LuremasterPeerConnectedEvent(EventManager? manager = null)
    : PeerConnectedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (Game1.GetPlayer(e.Peer.PlayerID, onlyOnline: true)?.HasProfession(Profession.Luremaster) != true)
        {
            return;
        }

        this.Manager.Enable(typeof(LuremasterTimeChangedEvent));
        this.Disable();
    }
}
