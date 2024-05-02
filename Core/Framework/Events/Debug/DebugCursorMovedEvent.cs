namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugCursorMovedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugCursorMovedEvent(EventManager? manager = null)
        : base(manager ?? CoreMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    internal static ICursorPosition? CursorPosition { get; set; }

    internal static ClickableComponent? FocusedComponent { get; set; }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        CursorPosition = e.NewPosition;
        if (Game1.activeClickableMenu is null)
        {
            return;
        }

        foreach (var component in DebugMenuChangedEvent.ClickableComponents)
        {
            var (cursorX, cursorY) = e.NewPosition.GetScaledScreenPixels();
            if (component.containsPoint((int)cursorX, (int)cursorY))
            {
                FocusedComponent = component;
            }
        }
    }
}
