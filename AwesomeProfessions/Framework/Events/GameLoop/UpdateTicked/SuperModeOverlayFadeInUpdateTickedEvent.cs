using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeOverlayFadeInUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModState.SuperModeOverlayAlpha += 0.01f;
        if (ModState.SuperModeOverlayAlpha >= 0.3f) ModEntry.Subscriber.Unsubscribe(GetType());
    }
}