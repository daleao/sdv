using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeBarRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        HUD.DrawSuperModeBar();
    }
}