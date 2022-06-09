namespace DaLion.Stardew.Alchemy.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class HostPeerDisconnectedEvent : PeerDisconnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object sender, PeerDisconnectedEventArgs e)
    {
        
    }
}