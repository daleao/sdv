namespace DaLion.Stardew.Alchemy.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Content;
using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e)
    {
       
    }
}