namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ScavengerHuntTriggerTimeChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ScavengerHuntTriggerTimeChangedEvent(EventManager? manager = null)
    : TimeChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        Log.D("Attempting to start Scavenger Hunt.");
        if (State.ScavengerHunt!.TryStart(Game1.currentLocation))
        {
            this.Disable();
        }
    }
}
