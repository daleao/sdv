namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Attributes;
using Common.Events;
using Common.Extensions.Xna;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

#endregion using directives

[UsedImplicitly, DebugOnly]
internal sealed class DebugRenderedActiveMenuEvent : RenderedActiveMenuEvent
{
    private readonly Texture2D _pixel;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugRenderedActiveMenuEvent(ProfessionEventManager manager)
        : base(manager)
    {
        _pixel = new(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _pixel.SetData(new[] { Color.White });
    }

    internal static List<ClickableComponent> ClickableComponents { get; } = new();
    internal static ClickableComponent? FocusedComponent { get; set; }

    /// <inheritdoc />
    protected override void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e)
    {
        ClickableComponents.Clear();
        var activeMenu = Game1.activeClickableMenu;
        if (activeMenu.allClickableComponents is null) activeMenu.populateClickableComponentList();

        ClickableComponents.AddRange(Game1.activeClickableMenu.allClickableComponents);
        if (Game1.activeClickableMenu is GameMenu gameMenu)
            ClickableComponents.AddRange(gameMenu.GetCurrentPage().allClickableComponents);

        foreach (var component in ClickableComponents)
        {
            component.bounds.DrawBorder(_pixel, 3, Color.Red, e.SpriteBatch);
            if (ModEntry.DebugCursorPosition is null) continue;

            var (cursorX, cursorY) = ModEntry.DebugCursorPosition.GetScaledScreenPixels();
            if (component.containsPoint((int)cursorX, (int)cursorY)) FocusedComponent = component;
        }
    }
}