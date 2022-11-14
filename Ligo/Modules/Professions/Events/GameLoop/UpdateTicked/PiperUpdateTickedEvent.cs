namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PiperUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PiperUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        // countdown contact timer
        if (ModEntry.State.Professions.SlimeContactTimer > 0 &&
            (Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass())
        {
            --ModEntry.State.Professions.SlimeContactTimer;
        }
    }
}
