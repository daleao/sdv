namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateOverlayRenderedWorldEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (ModEntry.Player.RegisteredUltimate is null)
        {
            Unhook();
            return;
        }

        ModEntry.Player.RegisteredUltimate.Overlay.Draw(e.SpriteBatch);
    }
}