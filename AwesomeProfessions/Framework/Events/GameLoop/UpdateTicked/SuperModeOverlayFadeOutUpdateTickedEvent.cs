using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderedWorld;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class SuperModeOverlayFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperModeOverlayAlpha -= 0.01f;
        if (ModEntry.State.Value.SuperModeOverlayAlpha <= 0)
            ModEntry.Subscriber.UnsubscribeFrom(typeof(SuperModeRenderedWorldEvent), GetType());
    }
}