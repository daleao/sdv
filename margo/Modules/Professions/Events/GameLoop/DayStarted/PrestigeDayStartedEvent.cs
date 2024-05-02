namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        ProfessionsModule.State.UsedStatueToday = false;
        this.Disable();
    }
}
