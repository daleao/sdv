namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[DebugOnly]
internal sealed class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugCursorMovedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugCursorMovedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        ModEntry.DebugCursorPosition = e.NewPosition;
    }
}
