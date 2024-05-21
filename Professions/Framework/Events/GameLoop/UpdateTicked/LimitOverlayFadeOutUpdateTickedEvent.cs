namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitOverlayFadeOutUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class LimitOverlayFadeOutUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        State.LimitBreak!.Overlay.FadeOut();
    }
}
