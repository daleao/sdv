namespace DaLion.Professions.Framework.Events.Display.RenderedActiveMenu;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="MasteryWarningRenderedActiveMenuEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class MasteryWarningRenderedActiveMenuEvent(EventManager? manager = null)
    : RenderedActiveMenuEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.WarningBox is not null;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        State.WarningBox!.draw(e.SpriteBatch);
    }
}
