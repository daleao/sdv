namespace DaLion.Professions.Framework.Events.Multiplayer.PeerDisconnected;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PrestigeTreasureHuntPeerDisconnectedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PrestigeTreasureHuntPeerDisconnectedEvent(EventManager? manager = null)
    : PeerDisconnectedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e)
    {
        var who = Game1.GetPlayer(e.Peer.PlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.Peer.PlayerID} has disconnected.");
            return;
        }

        Farmer_TreasureHunt.Values.Remove(who);
    }
}
