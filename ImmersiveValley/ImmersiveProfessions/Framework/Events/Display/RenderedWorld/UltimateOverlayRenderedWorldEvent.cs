namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, UltimateEvent]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateOverlayRenderedWorldEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        var ultimate = Game1.player.get_Ultimate();
        if (ultimate is null)
        {
            Disable();
            return;
        }

        ultimate.Overlay.Draw(e.SpriteBatch);
    }
}