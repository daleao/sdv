namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TreasureHuntPoolTrackerTimeChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TreasureHuntPoolTrackerTimeChangedEvent(EventManager? manager = null)
    : TimeChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static uint _stepsTakenUntilPreviousTimeChange;
    private static uint _itemsForagedUntilPreviousTimeChange;
    private static uint _treesChoppedUntilPreviousTimeChange;
    private static uint _rocksCrushedUntilPreviousTimeChange;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        _stepsTakenUntilPreviousTimeChange = Game1.player.stats.StepsTaken;
        _itemsForagedUntilPreviousTimeChange = Game1.player.stats.ItemsForaged;
        _treesChoppedUntilPreviousTimeChange = Game1.player.stats.Get("treesChopped");
        _rocksCrushedUntilPreviousTimeChange = Game1.player.stats.RocksCrushed;
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        var stepsTakenSincePreviousTimeChange = (int)(Game1.player.stats.StepsTaken - _stepsTakenUntilPreviousTimeChange);
        if (State.ScavengerHunt is not null && Game1.currentLocation.IsOutdoors)
        {
            var itemsForagedSincePreviousTimeChange = (int)(Game1.player.stats.ItemsForaged - _itemsForagedUntilPreviousTimeChange);
            var treesChoppedSincePreviousTimeChange = (int)(Game1.player.stats.Get("treesChopped") - _treesChoppedUntilPreviousTimeChange);
            if (stepsTakenSincePreviousTimeChange > 0 || itemsForagedSincePreviousTimeChange > 0 || treesChoppedSincePreviousTimeChange > 0)
            {
                State.ScavengerHunt.UpdateTriggerPool(
                    stepsTakenSincePreviousTimeChange,
                    itemsForagedSincePreviousTimeChange,
                    treesChoppedSincePreviousTimeChange);
            }

            _itemsForagedUntilPreviousTimeChange = Game1.player.stats.ItemsForaged;
            _treesChoppedUntilPreviousTimeChange = Game1.player.stats.Get("treesChopped");
        }
        else if (State.ProspectorHunt is not null && Game1.currentLocation is MineShaft or VolcanoDungeon)
        {
            var rocksCrushedSincePreviousTimeChange = (int)(Game1.player.stats.RocksCrushed - _rocksCrushedUntilPreviousTimeChange);
            if (stepsTakenSincePreviousTimeChange > 0 || rocksCrushedSincePreviousTimeChange > 0)
            {
                State.ProspectorHunt.UpdateTriggerPool(
                    stepsTakenSincePreviousTimeChange,
                    rocksCrushedSincePreviousTimeChange,
                    0);
            }

            _rocksCrushedUntilPreviousTimeChange = Game1.player.stats.RocksCrushed;
        }

        _stepsTakenUntilPreviousTimeChange = Game1.player.stats.StepsTaken;
    }
}
