namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[UltimateEvent]
internal sealed class UltimateOverlayRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateOverlayRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateOverlayRenderedWorldEvent(ProfessionEventManager manager)
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
