namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PathfinderDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PathfinderDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Pathfinder is not null || PathfinderAsync is not null;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        this.Manager.Reset();
        PerScreenState.ResetAllScreens();
        Pathfinder = null;
        PathfinderAsync = null;
    }
}
