namespace DaLion.Redux.Professions.Events.Player;

#region using directives

using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BruteWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BruteWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BruteWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            this.Manager.Enable<BruteUpdateTickedEvent>();
        }
        else
        {
            ModEntry.State.Professions.BruteRageCounter = 0;
            this.Manager.Enable<BruteUpdateTickedEvent>();
        }
    }
}
