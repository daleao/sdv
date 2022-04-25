namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Disable();
            return;
        }

        ModEntry.PlayerState.RegisteredUltimate.Overlay.Draw(e.SpriteBatch);
    }
}