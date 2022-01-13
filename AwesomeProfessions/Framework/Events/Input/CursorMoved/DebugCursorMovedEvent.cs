using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.Input;

[UsedImplicitly]
internal class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object sender, CursorMovedEventArgs e)
    {
        ModEntry.State.Value.CursorPosition = e.NewPosition;
    }
}