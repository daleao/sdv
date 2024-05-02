namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;

#region using directives

using System.Threading.Tasks;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LuremastersDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LuremastersDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LuremastersDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.IterateAllWithLocation<CrabPot>(), pair =>
        {
            pair.Instance.ResetSuccesses();
        });
    }
}
