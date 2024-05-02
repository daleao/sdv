namespace DaLion.Professions.Framework.Events.Multiplayer.PeerConnected;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LuremasterPeerConnectedEvent : PeerConnectedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LuremasterPeerConnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LuremasterPeerConnectedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (!Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Luremaster))
        {
            return;
        }

        this.Manager.Enable(
            typeof(LuremasterDayStartedEvent),
            typeof(LuremasterOneSecondUpdateTickedEvent),
            typeof(LuremasterTimeChangedEvent));
        this.Disable();
    }
}
