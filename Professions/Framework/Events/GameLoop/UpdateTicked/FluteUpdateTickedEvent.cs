namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FluteUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class FluteUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.SlimeFluteCooldown > 0 || State.SlimeFluteAddedScale > 0f;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (State.SlimeFluteCooldown > 0 && e.IsOneSecond)
        {
            State.SlimeFluteCooldown--;
            if (State.SlimeFluteCooldown == 0)
            {
                State.SlimeFluteAddedScale = 0.5f;
                Game1.playSound("objectiveComplete");
            }
        }

        if (State.SlimeFluteAddedScale > 0f)
        {
            State.SlimeFluteAddedScale -= 0.01f;
        }
    }
}
