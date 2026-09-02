namespace DaLion.Professions.Framework.Events.Multiplayer;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayEnding;
using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.World.ObjectListChanged;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionsPeerConnectedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProfessionsPeerConnectedEvent(EventManager? manager = null)
    : PeerConnectedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />s
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        var peer = Game1.GetPlayer(e.Peer.PlayerID, onlyOnline: true);
        if (peer is null)
        {
            return;
        }

        if (peer.HasProfession(Profession.Rancher))
        {
            this.Manager.Enable(
                typeof(NutritionDayStartedEvent),
                typeof(NutritionDayEndingEvent));
        }

        if (peer.HasProfession(Profession.Breeder) || peer.HasProfession(Profession.Producer) ||
            peer.HasProfession(Profession.Aquarist) || peer.HasProfession(Profession.Piper))
        {
            this.Manager.Enable<RevalidateBuildingsDayStartedEvent>();
        }

        if (peer.HasProfession(Profession.Luremaster))
        {
            this.Manager.Enable(
                typeof(LuremasterDayStartedEvent),
                typeof(LuremasterTimeChangedEvent));
        }

        if (peer.HasProfession(Profession.Piper))
        {
            this.Manager.Enable<ChromaBallObjectListChangedEvent>();
        }
    }
}
