namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ScavengerHunt != null;

    private static PerScreen<int> MovingFrames = new();

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        AddPointsForMoving();

        if (State.ScavengerHunt!.IsActive)
        {
            State.ScavengerHunt!.TimeUpdate(e.Ticks);
            if (Game1.player.HasProfession(Profession.Scavenger, true))
            {
                Game1.gameTimeInterval = 0;
            }
        }
    }

    /// <summary>Add scavenger hunt points for moving around. Using this method instead of Game1.player.stats.StepsTaken
    /// allows points to increase while not walking, i.e. being on a horse</summary>
    private void AddPointsForMoving()
    {
        if (Game1.player.isMoving())
        {
            MovingFrames.Value++;
            if (MovingFrames.Value >= 100)
            {
                // Character takes one step every 20 frames by default
                State.ScavengerHunt!.UpdateTriggerPool(MovingFrames.Value / 20, 0, 0, 0);
                MovingFrames.Value = MovingFrames.Value % 20;
            }
        }
    }
}
