using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.Display;

internal class SuperModeGaugeRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object sender, RenderingHudEventArgs e)
    {
        if (!Game1.eventUp)
            ModEntry.State.Value.SuperMode.Gauge.Draw(e.SpriteBatch);
    }
}