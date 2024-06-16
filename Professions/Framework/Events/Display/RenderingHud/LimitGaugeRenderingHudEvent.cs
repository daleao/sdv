namespace DaLion.Professions.Framework.Events.Display.RenderingHud;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitGaugeRenderingHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class LimitGaugeRenderingHudEvent(EventManager? manager = null)
    : RenderingHudEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        if (!Game1.game1.takingMapScreenshot && !Game1.game1.ScreenshotBusy)
        {
            State.LimitBreak!.Gauge.Draw(e.SpriteBatch);
        }
    }
}
