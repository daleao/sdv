namespace DaLion.Ligo.Modules.Professions.Events.Multiplayer;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerDisconnectedEvent : PeerDisconnectedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostPeerDisconnectedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostPeerDisconnectedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _))
        {
            this.Manager.Disable<ConservationismDayEndingEvent>();
        }
    }
}
