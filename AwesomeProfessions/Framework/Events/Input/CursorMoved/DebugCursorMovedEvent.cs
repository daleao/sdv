using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class DebugCursorMovedEvent : CursorMovedEvent
{
    internal static ICursorPosition CursorPosition { get; set; }

    /// <inheritdoc />
    public override void OnCursorMoved(object sender, CursorMovedEventArgs e)
    {
        CursorPosition = e.NewPosition;
    }
}