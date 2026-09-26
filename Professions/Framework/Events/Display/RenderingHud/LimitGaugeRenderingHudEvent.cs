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
    : RenderingHudEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        if (!Game1.game1.takingMapScreenshot && !Game1.game1.ScreenshotBusy &&
            !Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack)
        {
            try
            {
                State.LimitBreak!.Gauge.Draw(e.SpriteBatch);
            }
            catch (NullReferenceException)
            {
                Log.E($"Tried rendering Limit Gauge for {Game1.player.Name} who doesn't have a Limit Break. The even will forcefully shut-down.");
                this.Disable();
            }
        }
    }
}
