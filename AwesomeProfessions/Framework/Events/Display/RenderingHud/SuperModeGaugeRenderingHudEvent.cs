namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class SuperModeGaugeRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object sender, RenderingHudEventArgs e)
    {
        if (!Game1.eventUp)
            ModEntry.State.Value.SuperMode.Gauge.Draw(e.SpriteBatch);
    }
}