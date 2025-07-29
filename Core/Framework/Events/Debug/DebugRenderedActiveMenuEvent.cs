namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugRenderedActiveMenuEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedActiveMenuEvent(EventManager? manager = null)
    : RenderedActiveMenuEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var component in DebugMenuChangedEvent.ClickableComponents)
        {
            component.bounds.BorderHighlight(Color.Red, e.SpriteBatch);
        }
    }
}
