namespace DaLion.Overhaul.Modules.Professions.Events.Multiplayer.PeerDisconnected;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeTreasureHuntPeerDisconnectedEvent : PeerDisconnectedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeTreasureHuntPeerDisconnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeTreasureHuntPeerDisconnectedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e)
    {
        var who = Game1.getFarmerMaybeOffline(e.Peer.PlayerID);
        if (who is null)
        {
            Log.W($"[PRFS]: Unknown player {e.Peer.PlayerID} has disconnected.");
            return;
        }

        Farmer_TreasureHunt.HuntingState.Remove(who);
    }
}
