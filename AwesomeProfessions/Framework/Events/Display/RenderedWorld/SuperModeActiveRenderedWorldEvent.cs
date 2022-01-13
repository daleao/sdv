using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Display;

internal class SuperModeActiveRenderedWorldEvent : RenderedWorldEvent
{
    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e)
    {
        ModEntry.State.Value.SuperMode.Overlay.Draw(e.SpriteBatch);
    }
}