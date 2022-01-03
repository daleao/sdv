using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class DebugRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        // show FPS counter
        ModEntry.FpsCounter?.Draw(Game1.currentGameTime);
    }
}