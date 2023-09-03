namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateOverlayFadeInUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateOverlayFadeInUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateOverlayFadeInUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.Get_Ultimate()!.Overlay.FadeIn();
    }
}
