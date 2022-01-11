using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Input.CursorMoved;

[UsedImplicitly]
internal class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object sender, CursorMovedEventArgs e)
    {
        ModEntry.State.Value.CursorPosition = e.NewPosition;
    }
}