namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Menus;

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

    internal static List<ClickableComponent> ClickableComponents { get; } = new();

    internal static ClickableComponent? FocusedComponent { get; set; }

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        ClickableComponents.Clear();
        var activeMenu = Game1.activeClickableMenu;
        if (activeMenu.allClickableComponents is null)
        {
            activeMenu.populateClickableComponentList();
        }

        ClickableComponents.AddRange(Game1.activeClickableMenu.allClickableComponents);
        if (Game1.activeClickableMenu is GameMenu gameMenu)
        {
            ClickableComponents.AddRange(gameMenu.GetCurrentPage().allClickableComponents);
        }

        foreach (var component in ClickableComponents)
        {
            component.bounds.DrawBorder(this._pixel, 3, Color.Red, e.SpriteBatch);
            if (Globals.DebugCursorPosition is null)
            {
                continue;
            }

            var (cursorX, cursorY) = Globals.DebugCursorPosition.GetScaledScreenPixels();
            if (component.containsPoint((int)cursorX, (int)cursorY))
            {
                FocusedComponent = component;
            }
        }
    }
}
