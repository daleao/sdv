namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
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
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (!e.Peer.IsSplitScreen || !e.Peer.ScreenID.HasValue)
        {
            return;
        }

        ModEntry.Events.EnableForScreen<ArsenalSaveLoadedEvent>(e.Peer.ScreenID.Value);
        if (ModEntry.Config.FaceMouseCursor || ModEntry.Config.SlickMoves)
        {
            ModEntry.Events.EnableForScreen<DriftButtonPressedEvent>(e.Peer.ScreenID.Value);
        }

        if (ModEntry.Config.ComboHits)
        {
            ModEntry.Events.EnableForScreen<ComboButtonPressedEvent>(e.Peer.ScreenID.Value);
        }
    }
}
