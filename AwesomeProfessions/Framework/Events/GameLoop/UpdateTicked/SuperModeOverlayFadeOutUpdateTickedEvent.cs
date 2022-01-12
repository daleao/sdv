using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal class SuperModeOverlayFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperMode.Overlay.FadeOut();
    }
}