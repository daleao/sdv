namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpelunkerUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class SpelunkerUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.currentLocation is MineShaft && State.SpelunkerLadderStreak > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.applyBuff(new SpelunkerStreakBuff());
    }
}
