namespace DaLion.Professions.Framework.Events.Display.RenderedWorld;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitOverlayRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class LimitOverlayRenderedWorldEvent(EventManager? manager = null)
    : RenderedWorldEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.LimitBreak?.IsActive ?? false;

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        State.LimitBreak!.Overlay.Draw(e.SpriteBatch);
    }
}
