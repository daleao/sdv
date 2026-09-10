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
    /// <inheritdoc />
    protected override void OnEnabled()
    {
        State.StepsTakenUntilPreviousTimeChange = Game1.player.stats.StepsTaken;
        State.ItemsForagedUntilPreviousTimeChange = Game1.player.stats.ItemsForaged;
        State.TreesChoppedUntilPreviousTimeChange = Game1.player.stats.Get("treesChopped");
        State.RocksCrushedUntilPreviousTimeChange = Game1.player.stats.RocksCrushed;
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        var stepsTakenSincePreviousTimeChange = (int)(Game1.player.stats.StepsTaken - State.StepsTakenUntilPreviousTimeChange);
        if (State.ScavengerHunt is not null && Game1.currentLocation.IsOutdoors)
        {
            var itemsForagedSincePreviousTimeChange = (int)(Game1.player.stats.ItemsForaged - State.ItemsForagedUntilPreviousTimeChange);
            var treesChoppedSincePreviousTimeChange = (int)(Game1.player.stats.Get("treesChopped") - State.TreesChoppedUntilPreviousTimeChange);
            if (stepsTakenSincePreviousTimeChange > 0 || itemsForagedSincePreviousTimeChange > 0 || treesChoppedSincePreviousTimeChange > 0)
            {
                State.ScavengerHunt.UpdateTriggerPool(
                    stepsTakenSincePreviousTimeChange,
                    itemsForagedSincePreviousTimeChange,
                    treesChoppedSincePreviousTimeChange);
            }

            State.ItemsForagedUntilPreviousTimeChange = Game1.player.stats.ItemsForaged;
            State.TreesChoppedUntilPreviousTimeChange = Game1.player.stats.Get("treesChopped");
        }
        else if (State.ProspectorHunt is not null && Game1.currentLocation is MineShaft or VolcanoDungeon)
        {
            var rocksCrushedSincePreviousTimeChange = (int)(Game1.player.stats.RocksCrushed - State.RocksCrushedUntilPreviousTimeChange);
            if (stepsTakenSincePreviousTimeChange > 0 || rocksCrushedSincePreviousTimeChange > 0)
            {
                State.ProspectorHunt.UpdateTriggerPool(
                    stepsTakenSincePreviousTimeChange,
                    rocksCrushedSincePreviousTimeChange,
                    0);
            }

            State.RocksCrushedUntilPreviousTimeChange = Game1.player.stats.RocksCrushed;
        }

        State.StepsTakenUntilPreviousTimeChange = Game1.player.stats.StepsTaken;
    }
}
