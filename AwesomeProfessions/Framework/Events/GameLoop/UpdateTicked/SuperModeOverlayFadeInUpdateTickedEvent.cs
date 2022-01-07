using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class SuperModeOverlayFadeInUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperModeOverlayAlpha += 0.01f;
        if (ModEntry.State.Value.SuperModeOverlayAlpha >= 0.3f) ModEntry.Subscriber.UnsubscribeFrom(GetType());
    }
}