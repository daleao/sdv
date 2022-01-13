using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

[UsedImplicitly]
internal class HostPeerDisconnectedEvent : PeerDisconnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object sender, PeerDisconnectedEventArgs e)
    {
        if (!Game1.game1.DoesAnyPlayerHaveProfession("Conservationist", out _))
            ModEntry.EventManager.Disable(typeof(GlobalConservationistDayEndingEvent));
    }
}