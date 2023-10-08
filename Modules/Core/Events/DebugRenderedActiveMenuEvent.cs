namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedActiveMenuEvent : RenderedActiveMenuEvent
{
    private readonly Texture2D _pixel;

    /// <summary>Initializes a new instance of the <see cref="DebugRenderedActiveMenuEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugRenderedActiveMenuEvent(EventManager manager)
        : base(manager)
    {
        this._pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        this._pixel.SetData(new[] { Color.White });
    }

    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var component in DebugMenuChangedEvent.ClickableComponents)
        {
            component.bounds.DrawBorder(this._pixel, Color.Red, e.SpriteBatch);
        }
    }
}
