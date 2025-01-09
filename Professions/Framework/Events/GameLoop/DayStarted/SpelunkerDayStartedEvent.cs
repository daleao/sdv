namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpelunkerDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class SpelunkerDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnEnabled() => Game1.player.HasProfession(Profession.Spelunker, true);

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        State.HasSpelunkerUsedCheckpointToday = false;
    }
}
