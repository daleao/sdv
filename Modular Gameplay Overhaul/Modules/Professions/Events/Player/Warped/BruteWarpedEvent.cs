namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
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
    public override bool IsEnabled => ProfessionsModule.State.BruteRageCounter > 0;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.NewLocation.IsDungeon() && !e.NewLocation.HasMonsters())
        {
            ProfessionsModule.State.BruteRageCounter = 0;
        }
    }
}
