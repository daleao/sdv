using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace TheLion.Stardew.Professions.Framework.Events.Display;

internal class DebugRenderedActiveMenuEvent : RenderedActiveMenuEvent
{
    private readonly Texture2D _pixel;

    /// <summary>Construct an instance.</summary>
    internal DebugRenderedActiveMenuEvent()
    {
        _pixel = new(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _pixel.SetData(new[] {Color.White});
    }

    internal static List<ClickableComponent> ClickableComponents { get; } = new();
    internal static ClickableComponent FocusedComponent { get; set; }

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object sender, RenderedActiveMenuEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown()) return;

        ClickableComponents.Clear();
        var activeMenu = Game1.activeClickableMenu;
        if (activeMenu.allClickableComponents is null) activeMenu.populateClickableComponentList();

        ClickableComponents.AddRange(Game1.activeClickableMenu.allClickableComponents);
        if (Game1.activeClickableMenu is GameMenu gameMenu)
            ClickableComponents.AddRange(gameMenu.GetCurrentPage().allClickableComponents);

        foreach (var component in ClickableComponents)
        {
            DrawBorder(component.bounds, 3, Color.Red, e.SpriteBatch);
            if (ModEntry.State.Value.CursorPosition is null) continue;

            var (cursorX, cursorY) = ModEntry.State.Value.CursorPosition.GetScaledScreenPixels();
            if (component.containsPoint((int) cursorX, (int) cursorY)) FocusedComponent = component;
        }
    }

    /// <summary>Draw a border around a rectangle object.</summary>
    /// <param name="r">The rectangle.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    private void DrawBorder(Rectangle r, int thickness, Color color, SpriteBatch b)
    {
        b.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, thickness), color); // top line
        b.Draw(_pixel, new Rectangle(r.X, r.Y, thickness, r.Height), color); // left line
        b.Draw(_pixel, new Rectangle(r.X + r.Width - thickness, r.Y, thickness, r.Height), color); // right line
        b.Draw(_pixel, new Rectangle(r.X, r.Y + r.Height - thickness, r.Width, thickness), color); // bottom line
    }
}