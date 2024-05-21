namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitActiveUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class LimitActiveUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.LimitBreak?.IsActive == true;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.startGlowing(State.LimitBreak!.Color, false, 0.05f);
        if (Game1.game1.ShouldTimePass())
        {
            State.LimitBreak.Countdown();
        }
    }
}
