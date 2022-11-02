namespace DaLion.Redux.Framework.Professions.Events.Display;

#region using directives

using DaLion.Redux.Framework.Professions.Ultimates;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[UltimateEvent]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateOverlayRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateOverlayRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        var ultimate = Game1.player.Get_Ultimate();
        if (ultimate is null)
        {
            this.Disable();
            return;
        }

        ultimate.Overlay.Draw(e.SpriteBatch);
    }
}
