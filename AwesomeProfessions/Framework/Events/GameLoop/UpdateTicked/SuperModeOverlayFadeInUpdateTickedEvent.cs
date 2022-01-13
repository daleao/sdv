using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

internal class SuperModeOverlayFadeInUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperMode.Overlay.FadeIn();
    }
}