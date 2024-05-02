namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProspectorHuntOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    private static uint _previousHuntRocksCrushed;

    /// <summary>Initializes a new instance of the <see cref="ProspectorHuntOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProspectorHuntOneSecondUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        _previousHuntRocksCrushed = 0;
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Game1.currentLocation.IsSuitablePropsectorHuntLocation())
        {
            return;
        }

        var delta = Game1.player.stats.RocksCrushed - _previousHuntRocksCrushed;
        State.ProspectorHunt ??= new ProspectorHunt();
        if (State.ProspectorHunt.TryStart(Math.Pow(1.0016, delta) - 1d))
        {
            _previousHuntRocksCrushed += delta;
        }
    }
}
