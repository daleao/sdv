namespace DaLion.Overhaul.Modules.Professions.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpelunkerWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(VanillaProfession.Spelunker);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is MineShaft || e.OldLocation is not MineShaft)
        {
            return;
        }

        ProfessionsModule.State.SpelunkerLadderStreak = 0;
        this.Manager.Disable<SpelunkerUpdateTickedEvent>();
    }
}
