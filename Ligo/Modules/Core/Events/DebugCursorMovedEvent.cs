namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugCursorMovedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugCursorMovedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        Globals.DebugCursorPosition = e.NewPosition;
    }
}
