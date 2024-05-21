namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugMenuChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugMenuChangedEvent(EventManager? manager = null)
    : MenuChangedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    internal static List<ClickableComponent> ClickableComponents { get; } = [];

    /// <inheritdoc />
    protected override void OnMenuChangedImpl(object? sender, MenuChangedEventArgs e)
    {
        ClickableComponents.Clear();
        if (e.NewMenu is null)
        {
            DebugCursorMovedEvent.FocusedComponent = null;
            return;
        }

        var activeMenu = e.NewMenu;
        if (activeMenu.allClickableComponents is null)
        {
            activeMenu.populateClickableComponentList();
        }

        ClickableComponents.AddRange(Game1.activeClickableMenu.allClickableComponents);
        if (Game1.activeClickableMenu is GameMenu gameMenu)
        {
            ClickableComponents.AddRange(gameMenu.GetCurrentPage().allClickableComponents);
        }
    }
}
