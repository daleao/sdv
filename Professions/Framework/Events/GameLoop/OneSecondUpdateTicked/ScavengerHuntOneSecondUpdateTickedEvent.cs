namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static uint _previousHuntStepsTaken;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (_previousHuntStepsTaken == 0)
        {
            _previousHuntStepsTaken = Game1.player.stats.StepsTaken;
        }
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Game1.currentLocation.IsSuitableScavengerHuntLocation())
        {
            return;
        }

        var delta = Game1.player.stats.StepsTaken - _previousHuntStepsTaken;
        State.ScavengerHunt ??= new ScavengerHunt();
        if (State.ScavengerHunt.TryStart(Math.Pow(1.0016, delta) - 1d))
        {
            _previousHuntStepsTaken += delta;
        }
    }
}
