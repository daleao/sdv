namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal class SuperModeActiveOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e)
    {
        if (ModEntry.State.Value.SuperMode is null)
        {
            Disable();
            return;
        }

        ModEntry.State.Value.SuperMode.Overlay.Draw(e.SpriteBatch);
    }
}