namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugRenderedHudEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedHudEvent(EventManager? manager = null)
    : RenderedHudEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        State.FpsCounter?.Draw(Game1.currentGameTime);
    }
}
