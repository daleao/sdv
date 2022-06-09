namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class UltimateMeterRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object sender, RenderingHudEventArgs e)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Disable();
            return;
        }

        if (!Game1.eventUp) ModEntry.PlayerState.RegisteredUltimate.Hud.Draw(e.SpriteBatch);
    }
}