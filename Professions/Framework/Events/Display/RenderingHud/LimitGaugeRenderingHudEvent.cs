namespace DaLion.Professions.Framework.Events.Display.RenderingHud;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[LimitEvent]
[UsedImplicitly]
internal sealed class LimitGaugeRenderingHudEvent : RenderingHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="LimitGaugeRenderingHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LimitGaugeRenderingHudEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        State.LimitBreak!.Gauge.Draw(e.SpriteBatch);
    }
}
