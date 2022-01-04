using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events.Display.RenderingHud;

internal class SuperModeBarRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        if (!Game1.eventUp) HUD.DrawSuperModeBar();
    }
}