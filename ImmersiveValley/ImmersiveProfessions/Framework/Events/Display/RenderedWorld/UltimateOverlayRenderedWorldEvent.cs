namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
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