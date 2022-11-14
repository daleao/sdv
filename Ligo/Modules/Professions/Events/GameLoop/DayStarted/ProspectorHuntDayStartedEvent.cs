namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        ModEntry.State.Professions.ProspectorHunt.Value.ResetChanceAccumulator();
    }
}
