namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.TimeChanged;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class MonitorLuremastersDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="MonitorLuremastersDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal MonitorLuremastersDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster, out _))
        {
            this.Manager.Enable<LuremasterTimeChangedEvent>();
        }
    }
}
